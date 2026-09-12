using Microsoft.AspNetCore.Identity;

namespace SuperShop.Web.Models
{
    // A nossa classe de utilizador: estende a IdentityUser (que já traz
    // Email, UserName, PasswordHash, PhoneNumber, EmailConfirmed, etc.) e
    // acrescenta só o que nos interessa a mais.
    public class User : IdentityUser
    {
        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;
    }
}
