using Infrastructure.Data;
using Infrastructure.Data.DbContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace API.ContextFactory;

public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .AddJsonFile("appsettings.Development.json", reloadOnChange: true, optional: true)
            .AddEnvironmentVariables()
            .Build();

        var builder = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql(config.GetConnectionString("DefaultConnection"), 
                b => b.MigrationsAssembly("API"));
        return new AppDbContext(builder.Options);
    }
}