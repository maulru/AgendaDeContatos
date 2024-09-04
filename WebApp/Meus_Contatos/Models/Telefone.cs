namespace Meus_Contatos.Models
{
    public class Telefone
    {
        public int Id { get; set; }

        public int? ContatoId { get; set; }

        public virtual Contato Contato { get; set; }

        public int DDD { get; set; }

        public string NumeroTelefone { get; set; }

        public int? UsuarioId { get; set; }

        public virtual Usuario Usuario { get; set; }


    }
}
