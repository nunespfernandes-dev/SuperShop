using Microsoft.AspNetCore.Mvc;
using SuperShop.Web.Models;
using System.Diagnostics;

namespace SuperShop.Web.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        // Página apresentada quando se tenta aceder a um controlador/action
        // ou a um caminho qualquer que não existe (404 "genérico" do site,
        // ao contrário do ProductNotFound, que é específico dos produtos).
        // É chamada através de app.UseStatusCodePagesWithReExecute em
        // Program.cs, que reexecuta o pedido para "/Error/404".
        public IActionResult Error404()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
