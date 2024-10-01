namespace Core.Entity
{
    public class Estado : EntityBase
    {
        public string Nome { get; set; }
        public ICollection<Regiao> Regioes { get; set; }
    }
}
