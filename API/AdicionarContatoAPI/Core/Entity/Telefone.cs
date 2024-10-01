using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Core.Entity
{
    public class Telefone : EntityBase
    {
        public int ContatoId { get; set; }
        public int DDDId { get; set; }
        public string NumeroTelefone { get; set; }

        public string NumeroCompleto => $"({DDD.Codigo}) {NumeroTelefone}";
        [JsonIgnore]
        public Contato Contato { get; set; }
        public DDD DDD { get; set; }

    }
}
