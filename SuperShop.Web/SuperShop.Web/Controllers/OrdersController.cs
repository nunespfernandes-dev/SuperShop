using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SuperShop.Web.Data;
using SuperShop.Web.Models;

namespace SuperShop.Web.Controllers
{
    // Aulas 22-25 — Encomendas. Só utilizadores com login podem entrar.
    [Authorize]
    public class OrdersController : Controller
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IProductRepository _productRepository;

        public OrdersController(IOrderRepository orderRepository, IProductRepository productRepository)
        {
            _orderRepository = orderRepository;
            _productRepository = productRepository;
        }

        // GET: Orders — lista de encomendas (Admin vê todas, cliente só as dele)
        public async Task<IActionResult> Index()
        {
            var model = await _orderRepository.GetOrderAsync(User.Identity!.Name!);
            return View(model);
        }

        // GET: Orders/Create — mostra o carrinho temporário (não há POST:
        // esta view só serve para mostrar as linhas temporárias).
        public async Task<IActionResult> Create()
        {
            var model = await _orderRepository.GetDetailTempsAsync(User.Identity!.Name!);
            return View(model);
        }

        // GET: Orders/AddProduct — formulário com a combobox de produtos
        public IActionResult AddProduct()
        {
            var model = new AddItemViewModel
            {
                Quantity = 1,
                Products = _productRepository.GetComboProducts()
            };

            return View(model);
        }

        // POST: Orders/AddProduct
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddProduct(AddItemViewModel model)
        {
            if (ModelState.IsValid)
            {
                await _orderRepository.AddItemToOrderAsync(model, User.Identity!.Name!);
                return RedirectToAction("Create");
            }

            // A lista da combobox não vem no POST: tem de ser recarregada,
            // senão a view volta com o <select> vazio.
            model.Products = _productRepository.GetComboProducts();
            return View(model);
        }

        // GET: Orders/DeleteItem/5 — botão "apagar" de uma linha do carrinho
        public async Task<IActionResult> DeleteItem(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            await _orderRepository.DeleteDetailTempAsync(id.Value);
            return RedirectToAction("Create");
        }

        // GET: Orders/Increase/5 — botão "+"
        public async Task<IActionResult> Increase(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            await _orderRepository.ModifyOrderDetailTempQuantityAsync(id.Value, 1);
            return RedirectToAction("Create");
        }

        // GET: Orders/Decrease/5 — botão "-"
        public async Task<IActionResult> Decrease(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            await _orderRepository.ModifyOrderDetailTempQuantityAsync(id.Value, -1);
            return RedirectToAction("Create");
        }
    }
}
