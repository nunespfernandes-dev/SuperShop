using Microsoft.AspNetCore.Identity;
using SuperShop.Web.Models;

namespace SuperShop.Web.Data
{
    // "Seed" = semente: cria a base de dados (se não existir), cria o
    // utilizador admin e mete lá dentro alguns produtos de exemplo, para
    // nunca teres de andar a escrever tudo à mão sempre que apagas a base de
    // dados durante os testes.
    public class Seed
    {
        private readonly DataContext _context;
        private readonly IUserHelper _userHelper;
        private readonly Random _random;

        // Utilizador criado automaticamente na primeira execução. Serve de
        // "dono" temporário de tudo o que for criado, até existir login.
        public const string AdminEmail = "admin@supershop.com";
        private const string AdminPassword = "123456";

        public Seed(DataContext context, IUserHelper userHelper)
        {
            _context = context;
            _userHelper = userHelper;
            _random = new Random();
        }

        public async Task SeedAsync()
        {
            // Garante que a base de dados existe (cria-a se ainda não existir).
            await _context.Database.EnsureCreatedAsync();

            var admin = await _userHelper.GetUserByEmailAsync(AdminEmail);
            if (admin == null)
            {
                admin = new User
                {
                    FirstName = "Admin",
                    LastName = "SuperShop",
                    Email = AdminEmail,
                    UserName = AdminEmail,
                };

                var result = await _userHelper.CreateUserAsync(admin, AdminPassword);
                if (result != IdentityResult.Success)
                {
                    throw new InvalidOperationException("Não foi possível criar o utilizador admin (seed).");
                }
            }

            // Só cria produtos de exemplo se a tabela ainda estiver vazia.
            if (!_context.Products.Any())
            {
                AddProduct("iPhone X", admin);
                AddProduct("Magic Mouse", admin);
                AddProduct("iWatch Series 4", admin);
                AddProduct("iPad Mini", admin);

                await _context.SaveChangesAsync();
            }
        }

        private void AddProduct(string name, User user)
        {
            _context.Products.Add(new Product
            {
                Name = name,
                Price = _random.Next(1000),
                ImageUrl = string.Empty,
                IsAvailable = true,
                Stock = _random.Next(100),
                User = user,
            });
        }
    }
}
