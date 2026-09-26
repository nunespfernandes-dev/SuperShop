using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace SuperShop.Web.Models
{
    // A nossa classe de utilizador: estende a IdentityUser (que já traz
    // Email, UserName, PasswordHash, PhoneNumber, EmailConfirmed, etc.) e
    // acrescenta só o que nos interessa a mais.
    public class User : IdentityUser
    {
        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        // Aula 22/23 — Nome completo (calculado, não vai para a tabela).
        // Usado na lista de encomendas quando quem está logado é o Admin.
        [Display(Name = "Nome completo")]
        public string FullName => $"{FirstName} {LastName}";
    }
}
