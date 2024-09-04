using Core.Entity;

namespace Core.Repository
{
    public interface IRepository<T> where T : EntityBase
    {
        /// <summary>
        /// Contrato genérico para obter uma lista de registros.
        /// </summary>
        /// <returns></returns>
        IList<T> ObterTodos();

        /// <summary>
        /// Contrato genérico para obter um registro por ID.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        T ObterPorId(int id);
        /// <summary>
        /// Contrato genérico para realizar o cadastro de um registro.
        /// </summary>
        /// <param name="entidade"></param>
        void Cadastrar(T entidade);

        /// <summary>
        /// Contrato genérico para realizar a alteração de um registro.
        /// </summary>
        /// <param name="entidade"></param>
        void Alterar(T entidade);

        /// <summary>
        /// Contrato genérico para realizar a exclusão de um registro.
        /// </summary>
        /// <param name="id"></param>
        void Deletar(int id);
    }
}
