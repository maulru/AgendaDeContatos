using System.ComponentModel.DataAnnotations;

namespace Meus_Contatos.Models
{
    public class LoginViewModel
    {
        [Required]
        [Display(Name = "Usuário")]
        public string Username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Senha")]
        public string Password { get; set; }
    }
}
