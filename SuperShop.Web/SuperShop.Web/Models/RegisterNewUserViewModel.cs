using System.ComponentModel.DataAnnotations;

namespace SuperShop.Web.Models
{
    // ViewModel do registo de um novo utilizador. Tal como o LoginViewModel,
    // não deriva de nada — não tem a ver com nenhuma tabela, é só o que a
    // view do Register precisa de mostrar/receber.
    public class RegisterNewUserViewModel
    {
        [Required]
        [Display(Name = "Primeiro nome")]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Último nome")]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [Display(Name = "Utilizador (email)")]
        public string UserName { get; set; } = string.Empty;

        [Required]
        [MinLength(6, ErrorMessage = "O campo {0} tem de ter no mínimo {1} caracteres.")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Confirmar password")]
        [Compare("Password", ErrorMessage = "As passwords não coincidem.")]
        public string Confirm { get; set; } = string.Empty;
    }
}
