using PM.Core;

namespace PM.Interfaces;

public interface IDataRepository<T> where T : class
{
    Task<PaginatedList<T>> SearchAsync(int page, int pageSize, string query = "");
    Task<List<T>> GetAsync();
    Task<bool> DeleteAsync(int entityId);
    Task<bool> UpdateAsync(T entity);
    Task<T> InsertAsync(T entity);
    Task<T> DetailsAsync(int entityId);
    Task<bool> IsAlive();
}