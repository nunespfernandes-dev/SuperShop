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
                    // Se o utilizador tentou entrar diretamente numa página
                    // que precisava de login (ex.: Products/Create), o
                    // [Authorize] mandou-o para cá com uma query string
                    // "ReturnUrl" (é o comportamento por omissão do
                    // Identity, cujo LoginPath já é "/Account/Login"). Feito
                    // o login, devolve-o exatamente a esse sítio.
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
    }
}
