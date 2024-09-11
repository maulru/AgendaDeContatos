using System.ComponentModel.DataAnnotations;

namespace Meus_Contatos.Models
{
    public class RegistrarViewModel
    {
        [Required(ErrorMessage = "O campo usuário é obrigatório.")]
        [Display(Name = "Usuário")]
        public required string Username { get; set; }

        [Required(ErrorMessage = "O campo senha é obrigatório.")]
        [DataType(DataType.Password)]
        [Display(Name = "Senha")]
        public required string Password { get; set; }

        [Required(ErrorMessage = "É necessário realizar a confirmação da senha.")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirmar Senha")]
        [Compare("Password", ErrorMessage = "As senhas não coincidem.")]
        public required string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "O campo e-mail é obrigatório.")]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "E-mail")]
        public required string Email { get; set; }

        [Required(ErrorMessage = "O campo data de nascimento é obrigatório.")]
        [Display(Name = "Data de Nascimento")]
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }
    }
}
