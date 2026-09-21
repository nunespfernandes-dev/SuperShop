using System.ComponentModel.DataAnnotations;

namespace SuperShop.Web.Models
{
    // ViewModel para o utilizador logado poder mudar a password. Repara que,
    // ao contrário do Register, aqui NÃO metemos [MinLength] nem outras
    // pistas sobre as regras da password: quando alguém já está autenticado
    // e algo corre mal aqui, quanto menos informação dermos sobre o motivo,
    // melhor (não queremos ajudar um eventual atacante a "afinar" a password).
    public class ChangePasswordViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password atual")]
        public string OldPassword { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Nova password")]
        public string NewPassword { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Confirmar nova password")]
        [Compare("NewPassword", ErrorMessage = "As passwords não coincidem.")]
        public string Confirm { get; set; } = string.Empty;
    }
}
