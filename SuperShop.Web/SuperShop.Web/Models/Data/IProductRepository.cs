using SuperShop.Web.Models;

namespace SuperShop.Web.Data
{
    // Específico para Product, mas já vem com todo o CRUD de graça, herdado
    // do IGenericRepository<T>. Se amanhã precisares de um método só para
    // produtos (ex.: GetProductsByCategoryAsync), é aqui que o acrescentas.
    public interface IProductRepository : IGenericRepository<Product>
    {
    }
}
