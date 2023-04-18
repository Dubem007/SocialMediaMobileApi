using API.Extensions;
using API.Middlewares;
using API.SignalR;
using Application.Helpers;
using Hangfire;
using Infrastructure.Data.DbContext;
using Infrastructure.Data.DbSeeds;
using Infrastructure.SignalR;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.EntityFrameworkCore;
using NLog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.ConfigureSignalR();
builder.Services.ConfigureCors();
builder.Services.ConfigureIisIntegration();
builder.Services.ConfigureIdentity();
builder.Services.ConfigureLoggerService();
builder.Services.ConfigureIOObjects(builder.Configuration);
builder.Services.ConfigurePostgresSqlContext(builder.Configuration);
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddAuthentication();
builder.Services.ConfigureJwt(builder.Configuration);
builder.Services.AddHttpContextAccessor();
builder.Services.ConfigureAppServices();
builder.Services.ConfigureRepositoryManager();
builder.Services.ConfigureServiceManager();
builder.Services.ConfigureAWSServices(builder.Configuration);
builder.Services.AddControllers()
    .AddXmlDataContractSerializerFormatters();
builder.Services.ConfigureSwagger();
builder.Services.ConfigureApiVersioning(builder.Configuration);
builder.Services.ConfigureMvc();
builder.Services.ConfigureRedis(builder.Configuration);
builder.Services.ConfigureSignalR();

builder.WebHost.UseSentry(o =>
{
    o.Dsn = "https://e9f65ef1f9a44b0f9f9b6e6ee9d20e46@o373456.ingest.sentry.io/6573119";
    o.Debug = true;
    o.TracesSampleRate = 1.0;
});
builder.Services.ConfigureHangFire(builder.Configuration);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
app.SeedRoleData().Wait();
app.MembersConnData().Wait();

var apiVersionDescriptionProvider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
if (app.Environment.IsProduction())
{
    app.UseHsts();
}

app.UseSwagger();
app.UseSwaggerUI(c => 
{
    c.ConfigObject.AdditionalItems.Add("persistAuthorization", "true");
    foreach (var description in apiVersionDescriptionProvider.ApiVersionDescriptions.Reverse())
    {
        c.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json",
            description.GroupName.ToUpperInvariant());
    }
});
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseCors("CorsPolicy");

app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.UseHangfireDashboard();
}
else
{
    app.UseHangfireDashboard("/hangfire", new DashboardOptions
    {
        Authorization = new[] { new HangFireAuthorizationFilter(builder.Configuration) }
    });
}
app.UseErrorHandler();
app.MapControllers();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapHub<ChatHub>("notificationhub");
    endpoints.MapHub<NotificationHub>("notificationhub/notify");
});
WebHelper.Configure(app.Services.GetRequiredService<IHttpContextAccessor>());

// migrate any database changes on startup
using (var scope = app.Services.CreateScope())
{
    var dataContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dataContext.Database.Migrate();
}
app.Run();
