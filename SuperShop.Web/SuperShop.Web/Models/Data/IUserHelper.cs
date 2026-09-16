using Microsoft.AspNetCore.Identity;
using SuperShop.Web.Models;

namespace SuperShop.Web.Data
{
    // Tudo o que seja gestão de utilizadores e autenticação passa por aqui,
    // em vez de se injetar o UserManager<User>/SignInManager<User>
    // diretamente em cada controlador.
    public interface IUserHelper
    {
        Task<User?> GetUserByEmailAsync(string email);

        Task<IdentityResult> CreateUserAsync(User user, string password);

        // Faz login com o SignInManager (gestão de sessões/cookies) — não
        // confundir com o UserManager (gestão dos dados do utilizador).
        Task<SignInResult> LoginAsync(LoginViewModel model);

        Task LogoutAsync();
    }
}
