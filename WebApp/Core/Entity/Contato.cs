namespace Core.Entity
{
    public class Contato : EntityBase
    {
        public required string Nome { get; set; }
        public string Email { get; set; }
        public ICollection<Telefone> Telefones { get; set; }
    }
}
