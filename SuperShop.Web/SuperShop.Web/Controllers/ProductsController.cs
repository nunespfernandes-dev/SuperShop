using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SuperShop.Web.Data;
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
            if (id == null) return NotFound();

            var product = await _productRepository.GetByIdAsync(id.Value);
            if (product == null) return NotFound();

            return View(product);
        }

        // GET: Products/Create
        [Authorize]
        public IActionResult Create()
        {
            // Sem isto, o Model chegava null a Create.cshtml e a
            // _ProductForm.cshtml rebentava com NullReferenceException ao
            // chamar Model.GetImageFullPath(...).
            return View(new ProductViewModel());
        }

        // POST: Products/Create
        [Authorize]
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

                // TODO: substituir por User.Identity.Name agora que já
                // existe login. Por agora, todos os produtos criados
                // continuam associados ao admin do seed.
                product.User = await _userHelper.GetUserByEmailAsync(Seed.AdminEmail);

                await _productRepository.CreateAsync(product);
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // GET: Products/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var product = await _productRepository.GetByIdAsync(id.Value);
            if (product == null) return NotFound();

            var model = _converterHelper.ToProductViewModel(product);
            return View(model);
        }

        // POST: Products/Edit/5
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProductViewModel model)
        {
            if (id != model.Id) return NotFound();

            if (ModelState.IsValid)
            {
                if (!await _productRepository.ExistAsync(model.Id)) return NotFound();

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
