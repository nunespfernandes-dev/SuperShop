using System.ComponentModel.DataAnnotations;

namespace SuperShop.Web.Models
{
    // Não deriva de nada — não tem a ver com nenhuma tabela, é só o que a
    // view do Login precisa de mostrar/receber.
    public class LoginViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Utilizador (email)")]
        public string Username { get; set; } = string.Empty;

        [Required]
        [MinLength(6, ErrorMessage = "O campo {0} tem de ter no mínimo {1} caracteres.")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; } = string.Empty;

        // Para a pessoa não ter de fazer login sempre que fecha o browser.
        [Display(Name = "Lembrar-me")]
        public bool RememberMe { get; set; }
    }
}
