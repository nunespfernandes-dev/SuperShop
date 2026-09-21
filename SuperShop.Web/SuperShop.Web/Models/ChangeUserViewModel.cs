using System.ComponentModel.DataAnnotations;

namespace SuperShop.Web.Models
{
    // ViewModel para o utilizador logado poder alterar o primeiro e o
    // último nome. A password fica sempre à parte (ver ChangePasswordViewModel),
    // por questões de segurança.
    public class ChangeUserViewModel
    {
        [Required]
        [Display(Name = "Primeiro nome")]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Último nome")]
        public string LastName { get; set; } = string.Empty;
    }
}
