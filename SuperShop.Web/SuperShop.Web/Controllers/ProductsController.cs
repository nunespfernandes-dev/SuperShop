using Microsoft.AspNetCore.Mvc;
using SuperShop.Web.Data;
using SuperShop.Web.Models;

namespace SuperShop.Web.Controllers
{
    public class ProductsController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly IUserHelper _userHelper;
        private readonly IImageHelper _imageHelper;
        private readonly IConverterHelper _converterHelper;

        public ProductsController(
            IProductRepository productRepository,
            IUserHelper userHelper,
            IImageHelper imageHelper,
            IConverterHelper converterHelper)
        {
            _productRepository = productRepository;
            _userHelper = userHelper;
            _imageHelper = imageHelper;
            _converterHelper = converterHelper;
        }

        // GET: Products
        public IActionResult Index()
        {
            return View(_productRepository.GetAll());
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var product = await _productRepository.GetByIdAsync(id.Value);
            if (product == null) return NotFound();

            return View(product);
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Products/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductViewModel model)
        {
            // Ao criar, ainda não existe imagem nenhuma — é este método que a
            // vai gerar a partir do ficheiro enviado, mais abaixo. O campo
            // oculto ImageUrl chega sempre vazio neste ponto, e por ser uma
            // string não-anulável o ASP.NET Core marca-o como obrigatório
            // por omissão. Sem esta linha, o ModelState fica sempre inválido
            // e o produto nunca chega a ser criado.
            ModelState.Remove(nameof(model.ImageUrl));

            if (ModelState.IsValid)
            {
                var path = string.Empty;

                if (model.ImagesFile != null && model.ImagesFile.Length > 0)
                {
                    path = await _imageHelper.UploadImageAsync(model.ImagesFile, "products");
                }

                var product = _converterHelper.ToProduct(model, path, true);

                // TODO: substituir por User.Identity.Name quando o login existir.
                // Por agora, todos os produtos criados ficam associados ao admin.
                product.User = await _userHelper.GetUserByEmailAsync(Seed.AdminEmail);

                await _productRepository.CreateAsync(product);
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var product = await _productRepository.GetByIdAsync(id.Value);
            if (product == null) return NotFound();

            var model = _converterHelper.ToProductViewModel(product);
            return View(model);
        }

        // POST: Products/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProductViewModel model)
        {
            if (id != model.Id) return NotFound();

            // Mesmo motivo do Create: ImageUrl é string não-anulável, por
            // isso é implicitamente obrigatória. Se o produto ainda não
            // tinha imagem (campo oculto chega vazio), a validação falhava
            // sempre, mesmo sem se enviar ficheiro novo nenhum. O valor
            // certo é sempre calculado a seguir (mantém o antigo ou usa o
            // novo upload), por isso não faz sentido validá-lo aqui.
            ModelState.Remove(nameof(model.ImageUrl));

            if (ModelState.IsValid)
            {
                if (!await _productRepository.ExistAsync(model.Id)) return NotFound();

                // Por omissão mantém o caminho que já lá estava (campo oculto
                // na view). Só se vier um ficheiro novo é que se substitui.
                var path = model.ImageUrl;

                if (model.ImagesFile != null && model.ImagesFile.Length > 0)
                {
                    path = await _imageHelper.UploadImageAsync(model.ImagesFile, "products");
                }

                var product = _converterHelper.ToProduct(model, path, false);

                // A View de Edit não envia o User (não há nenhum campo, nem
                // oculto, para isso). Sem esta linha, o UPDATE gravava o
                // User a null, porque o model binder nunca o preenche.
                product.User = await _userHelper.GetUserByEmailAsync(Seed.AdminEmail);

                await _productRepository.UpdateAsync(product);
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var product = await _productRepository.GetByIdAsync(id.Value);
            if (product == null) return NotFound();

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product != null)
            {
                await _productRepository.DeleteAsync(product);
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
