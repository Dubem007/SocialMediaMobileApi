using Infrastructure.Data.DbContext;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Infrastructure.Data.DbSeeds
{
    public static class DbInitializer
    {
        public static async Task SeedRoleData(this IHost host)
        {
            var serviceProvider = host.Services.CreateScope().ServiceProvider;
            var context = serviceProvider.GetRequiredService<AppDbContext>();
            var roles = InitialMembersData.GetRoles();

            if (!context.Roles.Any())
            {
                await context.Roles.AddRangeAsync(roles);
                await context.SaveChangesAsync();
            }
        }

        public static async Task MembersConnData(this IHost host)
        {
            var serviceProvider = host.Services.CreateScope().ServiceProvider;
            var context = serviceProvider.GetRequiredService<AppDbContext>();
            var membersconn = MembersConnectionsData.GetMembersConnections();

            if (!context.UserConnections.Any())
            {
                await context.UserConnections.AddRangeAsync(membersconn);
                await context.SaveChangesAsync();
            }
        }

    }
}
