namespace Meus_Contatos.Models
{
    public class Estado
    {
        public int Id { get; set; }

        public string? Nome { get; set; }

        public int? IdRegiao { get; set; }

        public virtual Regiao Regiao { get; set; }

        public virtual ICollection<DDD> DDDs { get; set; }
    }
}
