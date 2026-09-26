using SuperShop.Web.Models;

namespace SuperShop.Web.Data
{
    // Aula 22 — Repositório da ÁREA das encomendas. Herda o CRUD básico do
    // genérico (para Order) e acrescenta os métodos específicos.
    public interface IOrderRepository : IGenericRepository<Order>
    {
        // Admin -> todas as encomendas; cliente -> só as dele.
        Task<IQueryable<Order>> GetOrderAsync(string userName);

        // Aula 23 — Linhas temporárias (carrinho) de um utilizador.
        Task<IQueryable<OrderDetailTemp>> GetDetailTempsAsync(string userName);

        // Aula 24 — Adiciona um produto ao carrinho (ou soma à quantidade
        // se esse produto já lá estiver).
        Task AddItemToOrderAsync(AddItemViewModel model, string userName);

        // Aula 24 — Soma (ou subtrai, se for negativa) uma quantidade a uma
        // linha temporária. Usado pelos botões + e -.
        Task ModifyOrderDetailTempQuantityAsync(int id, double quantity);

        // Aula 25 — Apaga uma linha temporária.
        Task DeleteDetailTempAsync(int id);
    }
}
