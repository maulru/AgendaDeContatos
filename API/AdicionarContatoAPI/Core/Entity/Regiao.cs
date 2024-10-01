using System.Text.Json.Serialization;

namespace Core.Entity
{
    public class Regiao : EntityBase
    {
        public string Nome { get; set; }
        public int EstadoId { get; set; }
        public Estado Estado { get; set; }
        [JsonIgnore]
        public ICollection<DDD> DDDs { get; set; }
    }
}
