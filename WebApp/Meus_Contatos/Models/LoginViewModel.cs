using System.ComponentModel.DataAnnotations;

namespace Meus_Contatos.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "O campo usuário é obrigatório.")]
        [Display(Name = "Usuário")]
        public required string Username { get; set; }

        [Required(ErrorMessage = "O campo senha é obrigatório")]
        [DataType(DataType.Password)]
        [Display(Name = "Senha")]
        public required string Password { get; set; }
    }
}
