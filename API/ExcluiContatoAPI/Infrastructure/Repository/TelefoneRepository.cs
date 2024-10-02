using Core.Entity;
using Core.Repository;

namespace Infrastructure.Repository
{
    public class TelefoneRepository : EFRepository<Telefone>, ITelefoneRepository
    {
        public TelefoneRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
