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
            // Nota de segurança: nunca devolver o objeto User completo aqui.
            // O IdentityUser (de onde o nosso User herda) tem campos
            // sensíveis — PasswordHash, SecurityStamp, ConcurrencyStamp,
            // etc. — que NUNCA devem sair numa resposta JSON, mesmo que a
            // password já esteja encriptada. Por isso projeta-se só os
            // campos seguros do utilizador.
            var products = _productRepository.GetAllWithUsers()
                .Select(p => new
                {
                    p.Id,
                    p.Name,
                    p.Price,
                    p.ImageUrl,
                    p.LastPurchase,
                    p.LastSale,
                    p.IsAvailable,
                    p.Stock,
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
