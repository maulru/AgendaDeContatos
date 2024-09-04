using Microsoft.AspNetCore.Identity;

namespace Meus_Contatos.Models
{
    public class Usuario : IdentityUser
    {
        public Usuario() : base() { }

        public int? ContatoId { get; set; }

        public virtual Contato Contato { get; set; }

    }
}
