using Core.Entity;
using Core.Repository;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository
{
    public class DDDRepository : IDDDRepository
    {
        #region Propriedades
        protected ApplicationDbContext _context;
        #endregion

        #region Construtores
        public DDDRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        #endregion

        #region Métodos
        public async Task<bool> ValidarExistenciaDDD(string codigoDDD)
        {
            return await _context.DDD.AnyAsync(d => d.Codigo == codigoDDD);
        }

        public async Task<DDD> RetornarDDDPorCodigo(string codigoDDD)
        {
            return await _context.DDD.FirstOrDefaultAsync(d => d.Codigo == codigoDDD);
        }
        #endregion
    }
}
