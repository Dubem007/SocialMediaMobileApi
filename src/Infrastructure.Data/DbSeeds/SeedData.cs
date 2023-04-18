using Domain.Entities;
using Domain.Entities.Identity;

namespace Infrastructure.Data.DbSeeds
{
    public static class InitialMembersData
    {
        public static List<Role> GetRoles()
        {
            return new List<Role>
            {
                new Role
                {
                    Name = "Regular",
                    NormalizedName = "Regular".ToUpper(),
                    ConcurrencyStamp = Guid.NewGuid().ToString(),
                }               
            };
        }
    }


    public static class MembersConnectionsData
    {
        public static List<UserConnection> GetMembersConnections()
        {
            return new List<UserConnection>
            {
                new UserConnection
                {
                    MemberId = Guid.Parse("b810d742-ca57-4126-86fb-2c8d4b35494d"),
                    ConnectedMemberId = Guid.Parse("9b9ec59c-0948-4d69-a1ef-82ecaea64fad"),
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },

                 new UserConnection
                {
                    MemberId = Guid.Parse("b810d742-ca57-4126-86fb-2c8d4b35494d"),
                    ConnectedMemberId = Guid.Parse("0f261fd9-ae6e-41f6-a5bf-cd4bcc0f8027"),
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },

                  new UserConnection
                {
                    MemberId = Guid.Parse("9b9ec59c-0948-4d69-a1ef-82ecaea64fad"),
                    ConnectedMemberId = Guid.Parse("b810d742-ca57-4126-86fb-2c8d4b35494d"),
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }
            };
        }
    }
}
