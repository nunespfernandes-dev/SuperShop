using System;
using System.Linq;
using System.Threading.Tasks;
using SuperShop.Web.Models;

namespace SuperShop.Web.Data
{
    // "Seed" = semente: cria a base de dados (se não existir) e mete lá dentro
    // alguns produtos de exemplo, para nunca teres de andar a escrever tudo à
    // mão sempre que apagas a base de dados durante os testes.
    public class Seed
    {
        private readonly DataContext _context;
        private readonly Random _random;

        public Seed(DataContext context)
        {
            _context = context;
            _random = new Random();
        }

        public async Task SeedAsync()
        {
            // Garante que a base de dados existe (cria-a se ainda não existir).
            await _context.Database.EnsureCreatedAsync();

            // Só cria produtos de exemplo se a tabela ainda estiver vazia.
            if (!_context.Products.Any())
            {
                AddProduct("iPhone X");
                AddProduct("Magic Mouse");
                AddProduct("iWatch Series 4");
                AddProduct("iPad Mini");

                await _context.SaveChangesAsync();
            }
        }

        private void AddProduct(string name)
        {
            _context.Products.Add(new Product
            {
                Name = name,
                Price = _random.Next(1000),
                IsAvailable = true,
                Stock = _random.Next(100),
            });
        }
    }
}
