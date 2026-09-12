using SuperShop.Web.Models;

namespace SuperShop.Web.Data
{
    // Específico para Product, mas já vem com todo o CRUD de graça, herdado
    // do IGenericRepository<T>. Se amanhã precisares de um método só para
    // produtos (ex.: GetProductsByCategoryAsync), é aqui que o acrescentas.
    public interface IProductRepository : IGenericRepository<Product>
    {
        // O GetAll() do genérico só conhece Product — não traz o User
        // (é outra entidade/tabela à parte). Este método é que faz o
        // "eager loading" do utilizador associado a cada produto.
        IQueryable<Product> GetAllWithUsers();
    }
}
