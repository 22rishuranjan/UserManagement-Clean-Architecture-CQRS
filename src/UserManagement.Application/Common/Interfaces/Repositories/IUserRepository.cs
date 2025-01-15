

using Microsoft.EntityFrameworkCore;

using UserManagement.Domain.Entities;

namespace UserManagement.Application.Common.Interfaces
{
    public interface IUserRepository
    {
        Task<User> GetByIdAsync(int id, CancellationToken cancellationToken);
        IQueryable<User> GetAllAsync(int pageNumber, int pageSize);
        Task<User> CreateAsync(User user, CancellationToken cancellationToken);
        Task UpdateAsync(User user, CancellationToken cancellationToken);
        Task DeleteAsync(int id, CancellationToken cancellationToken);
    }
}
