
using UserManagement.Domain.Entities;

namespace UserManagement.Application.Common.Interfaces
{
    public interface ICountryRepository
    {
        Task<Country> GetByIdAsync(int id, CancellationToken cancellationToken);
        IQueryable<Country> GetAllAsync(int pageNumber, int pageSize);
        Task<Country> CreateAsync(Country country, CancellationToken cancellationToken);
        Task UpdateAsync(Country country,CancellationToken cancellationToken);
        Task DeleteAsync(int id,CancellationToken cancellationToken);
    }
}
