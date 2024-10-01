using Core.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Response
{
    public class ApiResponse<T>
    {
        public List<T> Value { get; set; }
        public List<string> Formatters { get; set; }
        public List<string> ContentTypes { get; set; }
        public string DeclaredType { get; set; }
        public int? StatusCode { get; set; }
    }

    public class Contato
    {
        public string Nome { get; set; }
        public string Email { get; set; }
        public List<Telefone> Telefones { get; set; }
        public int Id { get; set; }
    }

    public class Telefone
    {
        public int ContatoId { get; set; }
        public int DDDId { get; set; }
        public string NumeroTelefone { get; set; }
        public string NumeroCompleto { get; set; }
        public DDD DDD { get; set; }
        public int Id { get; set; }
    }

    public class DDD
    {
        public int Id { get; set; }
        public string Codigo { get; set; }
        public int RegiaoId { get; set; }
        public Regiao Regiao { get; set; }
    }

    public class Regiao
    {
        public string Nome { get; set; }
        public int EstadoId { get; set; }
    }


}
