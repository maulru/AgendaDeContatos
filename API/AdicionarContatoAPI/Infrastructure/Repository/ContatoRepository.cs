using Core.Entity;
using Core.Repository;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository
{
    public class ContatoRepository : EFRepository<Contato>, IContatoRepository
    {
        public ContatoRepository(ApplicationDbContext context) : base(context)
        {
        }

        /// <summary>
        /// Método que retorna todos os Contatos
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Contato> ObterTodos()
        {
            return _context.Contato
                .Include(c => c.Telefones)
                    .ThenInclude(t => t.DDD)
                        .ThenInclude(d => d.Regiao)
                .ToList();
        }

        /// <summary>
        /// Retorna Contato Por ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public Contato ObterPorId(int id)
        {
            return _context.Contato
                .Include(c => c.Telefones)
                    .ThenInclude(t => t.DDD)
                .FirstOrDefault(c => c.Id == id);
        }

        /// <summary>
        ///  Método responsável pelo cadastro e retorno do usuário cadastrado
        /// </summary>
        /// <param name="contato"></param>
        /// <returns></returns>
        public async Task<Contato> Cadastrar(Contato contato)
        {
            await _context.Contato.AddAsync(contato);
            await _context.SaveChangesAsync();
            return contato;
        }

        /// <summary>
        /// Busca contatos por DDD
        /// </summary>
        /// <param name="ddd"></param>
        /// <returns></returns>
        public IEnumerable<Contato> BuscarContatosPorDDD(string ddd)
        {
            return _context.Contato
                .Include(c => c.Telefones)
                .ThenInclude(t => t.DDD)
                .Where(c => c.Telefones.Any(t => t.DDD.Codigo == ddd))
                .ToList();
        }

        /// <summary>
        /// Busca contatos por Região
        /// </summary>
        /// <param name="regiao"></param>
        /// <returns></returns>
        public IEnumerable<Contato> BuscarContatosPorRegiao(string regiao)
        {
            return _context.Contato
                .Include(c => c.Telefones)
                .ThenInclude(t => t.DDD)
                .ThenInclude(d => d.Regiao)
                .Where(c => c.Telefones.Any(t => t.DDD.Regiao.Nome == regiao))
                .ToList();
        }

    }
}
