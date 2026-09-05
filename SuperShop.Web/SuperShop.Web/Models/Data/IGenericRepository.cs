using System.Linq;
using System.Threading.Tasks;

namespace SuperShop.Web.Data
{
    // Repositório genérico: define o CRUD uma única vez, para qualquer
    // entidade T que implemente IEntity. Amanhã, se criares Category,
    // Supplier, Customer, etc., não voltas a escrever isto — só herdas.
    public interface IGenericRepository<T> where T : class, IEntity
    {
        IQueryable<T> GetAll();
        Task<T> GetByIdAsync(int id);
        Task CreateAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(T entity);
        Task<bool> ExistAsync(int id);
        Task<bool> SaveAllAsync();
    }
}
