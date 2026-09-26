using Microsoft.AspNetCore.Mvc.Rendering;
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

        public IEnumerable<SelectListItem> GetComboProducts()
        {
            // Forma "funcional": o Select percorre os produtos um a um e,
            // para cada um, cria um SelectListItem (Text = nome, Value = Id).
            // Não é preciso nenhum foreach.
            var list = _context.Products
                .Select(p => new SelectListItem
                {
                    Text = p.Name,
                    Value = p.Id.ToString()
                })
                .OrderBy(l => l.Text)
                .ToList();

            // Opção inicial com valor 0 na posição 0: assim a combobox não
            // mostra logo o primeiro produto escolhido. Como o
            // AddItemViewModel tem [Range(1, ...)], o 0 não é aceite.
            list.Insert(0, new SelectListItem
            {
                Text = "(Selecione um produto...)",
                Value = "0"
            });

            return list;
        }
    }
}
