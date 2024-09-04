using Core.Entity;
using Core.Repository;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository
{
    public class DDDRepository : EFRepository<DDD>, IDDDRepository
    {
        public DDDRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}