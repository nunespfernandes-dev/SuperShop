using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using SuperShop.Web.Data;

namespace SuperShop.Web.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductRepository _productRepository;
        private readonly IConfiguration _configuration;

        public ProductsController(IProductRepository productRepository, IConfiguration configuration)
        {
            _productRepository = productRepository;
            _configuration = configuration;
        }

        [HttpGet]
        public IActionResult GetProducts()
        {
            var blobBaseUrl = _configuration["Blob:BaseUrl"];

            // ToList(): materializa a query aqui (já com o Include do User
            // feito em GetAllWithUsers()). É preciso porque, a seguir,
            // vamos usar o Request (esquema + domínio do pedido atual) para
            // montar o caminho da imagem "sem imagem" — e isso o Entity
            // Framework não consegue traduzir para SQL, só existe do lado
            // do C#.
            var products = _productRepository.GetAllWithUsers()
                .ToList()
                .Select(p => new
                {
                    p.Id,
                    p.Name,
                    p.Price,
                    // Caminho da imagem — útil para quem consome a API (por
                    // exemplo uma app mobile): vai buscar a imagem
                    // diretamente ao Azure Blob Storage, ou à imagem
                    // "sem imagem" local, quando o produto não tem nenhuma.
                    ImageFullUrl = p.ImagesId == Guid.Empty
                        ? $"{Request.Scheme}://{Request.Host}/images/noimage.png"
                        : p.GetImageFullPath(blobBaseUrl),
                    p.LastPurchase,
                    p.LastSale,
                    p.IsAvailable,
                    p.Stock,
                    // Nota de segurança: nunca devolver o objeto User
                    // completo aqui. O IdentityUser (de onde o nosso User
                    // herda) tem campos sensíveis — PasswordHash,
                    // SecurityStamp, ConcurrencyStamp, etc. — que NUNCA
                    // devem sair numa resposta JSON. Por isso projeta-se só
                    // os campos seguros.
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
