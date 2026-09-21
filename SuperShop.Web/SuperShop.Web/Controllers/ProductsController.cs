using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SuperShop.Web.Data;
using SuperShop.Web.Helpers;
using SuperShop.Web.Models;

namespace SuperShop.Web.Controllers
{
    public class ProductsController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly IUserHelper _userHelper;
        private readonly IBlobHelper _blobHelper;
        private readonly IConverterHelper _converterHelper;

        public ProductsController(
            IProductRepository productRepository,
            IUserHelper userHelper,
            IBlobHelper blobHelper,
            IConverterHelper converterHelper)
        {
            _productRepository = productRepository;
            _userHelper = userHelper;
            _blobHelper = blobHelper;
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
            if (id == null)
            {
                return new NotFoundViewResult("ProductNotFound");
            }

            var product = await _productRepository.GetByIdAsync(id.Value);
            if (product == null)
            {
                return new NotFoundViewResult("ProductNotFound");
            }

            return View(product);
        }

        // GET: Products/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            // Sem isto, o Model chegava null a Create.cshtml e a
            // _ProductForm.cshtml rebentava com NullReferenceException ao
            // chamar Model.GetImageFullPath(...).
            return View(new ProductViewModel());
        }

        // POST: Products/Create
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductViewModel model)
        {
            if (ModelState.IsValid)
            {
                var imagesId = Guid.Empty;

                if (model.ImagesFile != null && model.ImagesFile.Length > 0)
                {
                    imagesId = await _blobHelper.UploadBlobAsync(model.ImagesFile, "products");
                }

                var product = _converterHelper.ToProduct(model, imagesId, true);

                // Já existe login: o produto fica associado ao utilizador
                // que está autenticado no momento, não a um admin fixo.
                product.User = await _userHelper.GetUserByEmailAsync(User.Identity!.Name!);

                await _productRepository.CreateAsync(product);
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // GET: Products/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new NotFoundViewResult("ProductNotFound");
            }

            var product = await _productRepository.GetByIdAsync(id.Value);
            if (product == null)
            {
                return new NotFoundViewResult("ProductNotFound");
            }

            var model = _converterHelper.ToProductViewModel(product);
            return View(model);
        }

        // POST: Products/Edit/5
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProductViewModel model)
        {
            if (id != model.Id)
            {
                return new NotFoundViewResult("ProductNotFound");
            }

            if (ModelState.IsValid)
            {
                if (!await _productRepository.ExistAsync(model.Id))
                {
                    return new NotFoundViewResult("ProductNotFound");
                }

                // Por omissão mantém o blob que já lá estava (campo oculto
                // na view). Só se vier um ficheiro novo é que se substitui.
                var imagesId = model.ImagesId;

                if (model.ImagesFile != null && model.ImagesFile.Length > 0)
                {
                    imagesId = await _blobHelper.UploadBlobAsync(model.ImagesFile, "products");
                }

                var product = _converterHelper.ToProduct(model, imagesId, false);

                // A View de Edit não envia o User (não há nenhum campo, nem
                // oculto, para isso). Sem esta linha, o UPDATE gravava o
                // User a null, porque o model binder nunca o preenche.
                product.User = await _userHelper.GetUserByEmailAsync(User.Identity!.Name!);

                await _productRepository.UpdateAsync(product);
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // GET: Products/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new NotFoundViewResult("ProductNotFound");
            }

            var product = await _productRepository.GetByIdAsync(id.Value);
            if (product == null)
            {
                return new NotFoundViewResult("ProductNotFound");
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [Authorize(Roles = "Admin")]
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

        // Página genérica apresentada quando se tenta aceder a um produto
        // que não existe (ID inválido, apagado entretanto, ou nenhum ID).
        public IActionResult ProductNotFound()
        {
            return View();
        }
    }
}
