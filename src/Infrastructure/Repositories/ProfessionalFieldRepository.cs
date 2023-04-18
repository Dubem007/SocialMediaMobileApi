using Domain.Entities;
using Infrastructure.Contracts;
using Infrastructure.Data.DbContext;

namespace Infrastructure.Repositories
{
    public class ProfessionalFieldRepository : Repository<Professions>, IProfessionalFieldRepository
    {
        public ProfessionalFieldRepository(AppDbContext appDbContext) : base(appDbContext)
        {
        }
    }
}
