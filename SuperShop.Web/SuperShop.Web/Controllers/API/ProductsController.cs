using Microsoft.AspNetCore.Mvc;
using SuperShop.Web.Data;

namespace SuperShop.Web.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductRepository _productRepository;

        public ProductsController(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        [HttpGet]
        public IActionResult GetProducts()
        {
            // ToList(): materializa a query aqui (já com o Include do User
            // feito em GetAllWithUsers()). É preciso porque, a seguir,
            // vamos usar o Request (esquema + domínio do pedido atual) para
            // montar o caminho absoluto da imagem — e isso o Entity
            // Framework não consegue traduzir para SQL, só existe do lado
            // do C#.
            var products = _productRepository.GetAllWithUsers()
                .ToList()
                .Select(p => new
                {
                    p.Id,
                    p.Name,
                    p.Price,
                    p.ImageUrl,
                    // Caminho absoluto da imagem — útil para quem consome a
                    // API (por exemplo uma app mobile) e não sabe de outra
                    // forma o endereço do servidor onde a imagem está.
                    ImageFullUrl = string.IsNullOrEmpty(p.ImageUrl)
                        ? null
                        : $"{Request.Scheme}://{Request.Host}{p.ImageUrl}",
                    p.LastPurchase,
                    p.LastSale,
                    p.IsAvailable,
                    p.Stock,
                    // Nota de segurança: nunca devolver o objeto User completo
                    // aqui. O IdentityUser (de onde o nosso User herda) tem
                    // campos sensíveis — PasswordHash, SecurityStamp,
                    // ConcurrencyStamp, etc. — que NUNCA devem sair numa
                    // resposta JSON. Por isso projeta-se só os campos seguros.
                    User = p.User == null ? null : new
                    {
                        p.User.Id,
                        p.User.FirstName,
                        p.User.LastName,
                        p.User.Email,
                    },
                });

            return Ok(products);
        }
    }
}
