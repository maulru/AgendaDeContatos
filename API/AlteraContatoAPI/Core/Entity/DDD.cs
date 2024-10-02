using System.Text.Json.Serialization;

namespace Core.Entity
{
    public class DDD : EntityBase
    {
        public int Id { get; set; }
        public string Codigo { get; set; }
        public int RegiaoId { get; set; }
        public Regiao Regiao { get; set; }
        [JsonIgnore]
        public ICollection<Telefone> Telefones { get; set; }
    }
}
