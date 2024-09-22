using Core.Entity;

namespace Core.Repository
{
    public interface IDDDRepository
    {
        /// <summary>
        /// Contrato responsável por validar a existência do DDD
        /// </summary>
        /// <param name="codigoDDD"></param>
        /// <returns></returns>
        Task<bool> ValidarExistenciaDDD(string codigoDDD);

        Task<DDD> RetornarDDDPorCodigo(string codigoDDD);
    }
}
