using Microsoft.AspNetCore.Identity;
using SuperShop.Web.Models;

namespace SuperShop.Web.Data
{
    // Tudo o que seja gestão de utilizadores, papéis (roles) e autenticação
    // passa por aqui, em vez de se injetar o UserManager<User>,
    // SignInManager<User> e RoleManager<IdentityRole> diretamente em cada
    // controlador.
    public interface IUserHelper
    {
        Task<User?> GetUserByEmailAsync(string email);

        Task<IdentityResult> CreateUserAsync(User user, string password);

        Task<IdentityResult> UpdateUserAsync(User user);

        Task<IdentityResult> ChangePasswordAsync(User user, string oldPassword, string newPassword);

        // Faz login com o SignInManager (gestão de sessões/cookies) — não
        // confundir com o UserManager (gestão dos dados do utilizador).
        Task<SignInResult> LoginAsync(LoginViewModel model);

        Task LogoutAsync();

        // Garante que um determinado role (ex.: "Admin") existe na base de
        // dados — se não existir, cria-o.
        Task CheckRoleAsync(string roleName);

        Task AddUserToRoleAsync(User user, string roleName);

        Task<bool> IsUserInRoleAsync(User user, string roleName);
    }
}
