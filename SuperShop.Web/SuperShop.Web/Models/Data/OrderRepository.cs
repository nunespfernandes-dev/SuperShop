using Microsoft.EntityFrameworkCore;
using SuperShop.Web.Models;

namespace SuperShop.Web.Data
{
    // Porque é que herda do GenericRepository<Order> E implementa o
    // IOrderRepository? Porque na Dependency Injection registamos sempre
    // "interface -> classe" (AddScoped<IOrderRepository, OrderRepository>).
    // O genérico nunca é injetado diretamente (é como uma classe abstrata),
    // por isso a classe tem de declarar explicitamente o seu interface.
    public class OrderRepository : GenericRepository<Order>, IOrderRepository
    {
        private readonly IUserHelper _userHelper;

        public OrderRepository(DataContext context, IUserHelper userHelper) : base(context)
        {
            _userHelper = userHelper;
        }

        public async Task<IQueryable<Order>> GetOrderAsync(string userName)
        {
            // Nunca confiar cegamente no userName: confirmar sempre na tabela.
            var user = await _userHelper.GetUserByEmailAsync(userName);
            if (user == null)
            {
                return Enumerable.Empty<Order>().AsQueryable();
            }

            if (await _userHelper.IsUserInRoleAsync(user, "Admin"))
            {
                // Include     -> tabela ligada DIRETAMENTE (Orders -> OrderDetails)
                // ThenInclude -> tabela ligada através da anterior
                //                (Orders -> OrderDetails -> Products)
                return _context.Orders
                    .Include(o => o.User)   // para mostrar o nome do cliente ao Admin
                    .Include(o => o.Items!)
                    .ThenInclude(i => i.Product)
                    .OrderByDescending(o => o.OrderDate);
            }

            return _context.Orders
                .Include(o => o.Items!)
                .ThenInclude(i => i.Product)
                .Where(o => o.User.Id == user.Id)
                .OrderByDescending(o => o.OrderDate);
        }

        public async Task<IQueryable<OrderDetailTemp>> GetDetailTempsAsync(string userName)
        {
            var user = await _userHelper.GetUserByEmailAsync(userName);
            if (user == null)
            {
                return Enumerable.Empty<OrderDetailTemp>().AsQueryable();
            }

            // Aqui é só Include (sem ThenInclude): o OrderDetailTemp está
            // ligado DIRETAMENTE ao Product.
            return _context.OrderDetailsTemp
                .Include(o => o.Product)
                .Where(o => o.User.Id == user.Id)
                .OrderBy(o => o.Product.Name);
        }

        public async Task AddItemToOrderAsync(AddItemViewModel model, string userName)
        {
            var user = await _userHelper.GetUserByEmailAsync(userName);
            if (user == null)
            {
                return;
            }

            // Também não confiamos na combobox: o produto pode ter sido
            // apagado entretanto por um admin.
            var product = await _context.Products.FindAsync(model.ProductId);
            if (product == null)
            {
                return;
            }

            // Já existe uma linha deste produto no carrinho deste user?
            var orderDetailTemp = await _context.OrderDetailsTemp
                .Where(odt => odt.User.Id == user.Id && odt.Product.Id == product.Id)
                .FirstOrDefaultAsync();

            if (orderDetailTemp == null)
            {
                // Primeira vez: cria uma linha nova.
                orderDetailTemp = new OrderDetailTemp
                {
                    Price = product.Price,
                    Product = product,
                    Quantity = model.Quantity,
                    User = user
                };

                _context.OrderDetailsTemp.Add(orderDetailTemp);
            }
            else
            {
                // Já existe: em vez de uma 2.ª linha, soma a quantidade
                // (30 copos + 5 copos = 1 linha com 35 copos).
                orderDetailTemp.Quantity += model.Quantity;
                _context.OrderDetailsTemp.Update(orderDetailTemp);
            }

            // Sem isto fica tudo só em memória (foi o erro da aula 24).
            await _context.SaveChangesAsync();
        }

        public async Task ModifyOrderDetailTempQuantityAsync(int id, double quantity)
        {
            var orderDetailTemp = await _context.OrderDetailsTemp.FindAsync(id);
            if (orderDetailTemp == null)
            {
                return;
            }

            orderDetailTemp.Quantity += quantity;

            // Segurança: só grava se a quantidade continuar acima de zero.
            if (orderDetailTemp.Quantity > 0)
            {
                _context.OrderDetailsTemp.Update(orderDetailTemp);
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteDetailTempAsync(int id)
        {
            var orderDetailTemp = await _context.OrderDetailsTemp.FindAsync(id);
            if (orderDetailTemp == null)
            {
                return;
            }

            _context.OrderDetailsTemp.Remove(orderDetailTemp);
            await _context.SaveChangesAsync();
        }
    }
}
