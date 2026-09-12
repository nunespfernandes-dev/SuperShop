using Microsoft.AspNetCore.Identity;
using SuperShop.Web.Models;

namespace SuperShop.Web.Data
{
    // Tudo o que seja gestão de utilizadores passa por aqui, em vez de se
    // injetar o UserManager<User> diretamente em cada controlador.
    public interface IUserHelper
    {
        Task<User?> GetUserByEmailAsync(string email);

        Task<IdentityResult> CreateUserAsync(User user, string password);
    }
}
