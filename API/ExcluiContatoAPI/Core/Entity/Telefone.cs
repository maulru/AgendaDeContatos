using System.Text.Json.Serialization;

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
