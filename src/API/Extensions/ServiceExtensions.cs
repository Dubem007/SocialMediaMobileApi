using Amazon.S3;
using API.Configurations;
using API.Middlewares;
using API.SignalR;
using Application.Contracts;
using Application.Helpers;
using Application.Services;
using Application.Settings;
using Application.Validations;
using Domain.Entities.Identity;
using FluentValidation.AspNetCore;
using Hangfire;
using Hangfire.PostgreSql;
using Infrastructure.Contracts;
using Infrastructure.Data.DbContext;
using Infrastructure.HttpHelper;
using Infrastructure.Logger;
using Infrastructure.Repositories;
using Infrastructure.Utils.AWS;
using Infrastructure.Utils.Email;
using Infrastructure.Utils.Notification;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using StackExchange.Redis;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Text;
using Role = Domain.Entities.Identity.Role;
using Domain.ConfigurationModels;
using CorePush.Google;
using AutoMapper;
using Domain.Entities;
using Infrastructure.Utils.EmailClient;

namespace API.Extensions;

public static class ServiceExtensions
{
    public static void ConfigureCors(this IServiceCollection services)
    {
        services.AddCors(options => options.AddPolicy("CorsPolicy",
            builder =>
            {
                builder.AllowAnyHeader()
                       .AllowAnyMethod()
                       .SetIsOriginAllowed((host) => true)
                       .AllowCredentials();
            }));
    }

    public static void ConfigureLoggerService(this IServiceCollection services)
    {
        services.AddSingleton<ILoggerManager, LoggerManager>();
    }

    public static void ConfigureServiceManager(this IServiceCollection services)
    {
        services.AddScoped<IServiceManager, ServiceManager>();
    }

    public static void ConfigureIisIntegration(this IServiceCollection serviceCollection) =>
        serviceCollection.Configure<IISOptions>(options => { });

    public static void ConfigurePostgresSqlContext(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        serviceCollection.AddDbContext<AppDbContext>(
          opts =>
          {
              opts.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));
          });
    }

    public static void ConfigureIOObjects(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<AwsConfiguration>(configuration.GetSection("AWS"));
        services.Configure<FileSettings>(configuration.GetSection(nameof(FileSettings)));
        services.Configure<SmtpSettings>(configuration.GetSection(nameof(SmtpSettings)));
    }

    public static void ConfigureJwt(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtSettings = configuration.GetSection("JwtSettings");
        var jwtUserSecret = jwtSettings.GetSection("Secret").Value;

        services.AddAuthentication(opt =>
        {
            opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            options.RequireHttpsMetadata = false;
            options.SaveToken = true;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSettings.GetSection("ValidIssuer").Value,
                ValidAudience = jwtSettings.GetSection("ValidAudience").Value,
                IssuerSigningKey = new
                    SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtUserSecret))
            };
            options.Events = new JwtBearerEvents
            {
                OnMessageReceived = context =>
                {
                    var accessToken = context.Request.Query["access_token"];

                    var path = context.Request.Path;
                    if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/notificationhub"))
                    {
                        context.Token = accessToken;
                    }

                    return Task.CompletedTask;
                }
            };
        });
    }

    public static void ConfigureSignalR(this IServiceCollection services)
    {
        services.AddCors();
        services.AddSignalR();
        services.AddSingleton<IUserIdProvider, UserIdProvider>();
    }
    public static void ConfigureRedis(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IConnectionMultiplexer>(sp =>
        {
            var configurationOptions = ConfigurationOptions.Parse(configuration.GetConnectionString("RedisConnection"), true);
            return ConnectionMultiplexer.Connect(configurationOptions); 
        });
    }

    public static void ConfigureMvc(this IServiceCollection services)
    {
        services.AddMvc()
            .ConfigureApiBehaviorOptions(o =>
            {
                o.InvalidModelStateResponseFactory = context => new ValidationFailedResult(context.ModelState);
            }).AddFluentValidation(x => x.RegisterValidatorsFromAssemblyContaining<InitialMemberValidator>());
        services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
    }

    public static void ConfigureHangFire(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHangfire(config =>
            config.UsePostgreSqlStorage(configuration.GetConnectionString("HangFireConnection")));
        services.AddHangfireServer();
    }

    public static void ConfigureAWSServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddTransient<IAwsS3Client, AwsS3Client>();
        services.AddTransient<IWebHelper, WebHelper>();
        services.AddTransient<INotificationService, NotificationService>();
        services.AddTransient<IHttpClientHelper, HttpClientHelper>();
        services.AddTransient<ICacheServices, CacheServices>();
    }

    public static void ConfigureRepositoryManager(this IServiceCollection services) =>
             services.AddScoped<IRepositoryManager, RepositoryManager>();

    public static void ConfigureAppServices(this IServiceCollection services)
    {
        services.AddScoped<IEmailManager, EmailManager>();
        services.AddScoped<INotificationManager, NotificationManager>();
        services.AddHttpClient<FcmSender>();
        services.AddScoped<IEmailClient, EmailClient>();
    }

    public static void ConfigureApiVersioning(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddApiVersioning(opt =>
        {
            opt.AssumeDefaultVersionWhenUnspecified = true;
            opt.DefaultApiVersion = new ApiVersion(1, 0);
            opt.ReportApiVersions = true;
        });
        services.AddVersionedApiExplorer(opt =>
        {
            opt.GroupNameFormat = "'v'VVV";
            opt.SubstituteApiVersionInUrl = true;
        });
        services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
        services.AddMvcCore().AddApiExplorer();
    }

    public static void ConfigureSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen(c =>
        {
            c.OperationFilter<RemoveVersionFromParameter>();
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "JWT Authorization header using the Bearer scheme."
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    new string[] {}
                }
            });
        });
    }

    public static void ConfigureIdentity(this IServiceCollection services)
    {
        var builder = services.AddIdentity<User, Role>(opt =>
            {
                opt.Password.RequireDigit = true;
                opt.Password.RequireLowercase = true;
                opt.Password.RequireUppercase = true;
                opt.Password.RequireNonAlphanumeric = false;
                opt.Password.RequiredLength = 8;
                opt.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();
    }
}