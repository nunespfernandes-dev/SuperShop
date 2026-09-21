using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SuperShop.Web.Data;
using SuperShop.Web.Models;

namespace SuperShop.Web.Controllers
{
    // Tudo o que tenha a ver com login, logout, registo e gestão da conta
    // do utilizador fica aqui, à parte dos outros controladores.
    public class AccountController : Controller
    {
        private readonly IUserHelper _userHelper;

        public AccountController(IUserHelper userHelper)
        {
            _userHelper = userHelper;
        }

        public IActionResult Login()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _userHelper.LoginAsync(model);
                if (result.Succeeded)
                {
                    // Agora que o LoginPath do cookie aponta sempre para
                    // NotAuthorized (ver Program.cs), este "ReturnUrl" já não
                    // costuma vir preenchido — fica só por segurança/histórico.
                    if (Request.Query.Keys.Contains("ReturnUrl"))
                    {
                        return Redirect(Request.Query["ReturnUrl"].First()!);
                    }

                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError(string.Empty, "Não foi possível fazer login. Confirma o email e a password.");
            }

            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await _userHelper.LogoutAsync();
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Register()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterNewUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userHelper.GetUserByEmailAsync(model.UserName);
                if (user == null)
                {
                    user = new User
                    {
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        Email = model.UserName,
                        UserName = model.UserName,
                    };

                    var result = await _userHelper.CreateUserAsync(user, model.Password);
                    if (result != IdentityResult.Success)
                    {
                        ModelState.AddModelError(string.Empty, "Não foi possível criar o utilizador.");
                        return View(model);
                    }

                    // Um utilizador que se regista sozinho no site fica
                    // sempre como "Customer" — nunca como Admin.
                    await _userHelper.AddUserToRoleAsync(user, Seed.CustomerRole);

                    // Registo imediato: assim que a conta é criada, o
                    // utilizador entra logo automaticamente como logado.
                    var loginViewModel = new LoginViewModel
                    {
                        Username = model.UserName,
                        Password = model.Password,
                        RememberMe = false,
                    };

                    var loginResult = await _userHelper.LoginAsync(loginViewModel);
                    if (loginResult.Succeeded)
                    {
                        return RedirectToAction("Index", "Home");
                    }

                    ModelState.AddModelError(string.Empty, "Não foi possível iniciar sessão.");
                    return View(model);
                }

                ModelState.AddModelError(string.Empty, "Este utilizador já existe.");
            }

            return View(model);
        }

        [Authorize]
        public async Task<IActionResult> ChangeUser()
        {
            var model = new ChangeUserViewModel();

            var user = await _userHelper.GetUserByEmailAsync(User.Identity!.Name!);
            if (user != null)
            {
                model.FirstName = user.FirstName;
                model.LastName = user.LastName;
            }

            return View(model);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeUser(ChangeUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userHelper.GetUserByEmailAsync(User.Identity!.Name!);
                if (user != null)
                {
                    user.FirstName = model.FirstName;
                    user.LastName = model.LastName;

                    var response = await _userHelper.UpdateUserAsync(user);
                    if (response.Succeeded)
                    {
                        ViewBag.UserMessage = "Utilizador atualizado.";
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, response.Errors.FirstOrDefault()?.Description ?? "Não foi possível atualizar o utilizador.");
                    }
                }
            }

            return View(model);
        }

        [Authorize]
        public IActionResult ChangePassword()
        {
            // Ao contrário do ChangeUser, esta view nunca vem preenchida —
            // não faz sentido mostrar a password antiga.
            return View();
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userHelper.GetUserByEmailAsync(User.Identity!.Name!);
                if (user != null)
                {
                    var response = await _userHelper.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
                    if (response.Succeeded)
                    {
                        return RedirectToAction(nameof(ChangeUser));
                    }

                    ModelState.AddModelError(string.Empty, response.Errors.FirstOrDefault()?.Description ?? "Não foi possível alterar a password.");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Utilizador não encontrado.");
                }
            }

            return View(model);
        }

        // Página apresentada sempre que o Identity bloqueia o acesso a
        // alguma zona da aplicação — quer seja por não estar autenticado
        // (LoginPath), quer seja por estar autenticado mas sem o role
        // necessário (AccessDeniedPath). Ver a configuração em Program.cs.
        public IActionResult NotAuthorized()
        {
            return View();
        }
    }
}
