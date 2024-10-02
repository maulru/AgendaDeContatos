using Core.Entity;
using Core.Repository;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository
{
    public class RegiaoRepository : EFRepository<Regiao>, IRegiaoRepository
    {
        public RegiaoRepository(ApplicationDbContext context) : base(context)
        {
        }

    }
}