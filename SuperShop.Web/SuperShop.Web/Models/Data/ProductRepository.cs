using Microsoft.EntityFrameworkCore;
using SuperShop.Web.Models;

namespace SuperShop.Web.Data
{
    public class ProductRepository : GenericRepository<Product>, IProductRepository
    {
        public ProductRepository(DataContext context) : base(context)
        {
        }

        public IQueryable<Product> GetAllWithUsers()
        {
            // Include(): diz ao EF Core para ir buscar também o User
            // relacionado, numa única query (join), em vez de deixar a
            // propriedade a null como acontece no GetAll() genérico.
            return _context.Products
                .Include(p => p.User)
                .AsNoTracking();
        }
    }
}
