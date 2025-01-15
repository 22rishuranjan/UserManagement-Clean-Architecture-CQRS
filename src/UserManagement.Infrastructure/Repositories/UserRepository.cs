
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using UserManagement.Application.Common.Interfaces;
using UserManagement.Domain.Entities;
using UserManagement.Infrastructure.Data;

namespace UserManagement.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;
 

    public UserRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<User> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        return await _context.Users
            .Include(u => u.Country) // Include the Country in the query
            .AsNoTracking() 
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    public IQueryable<User> GetAllAsync(int pageNumber, int pageSize)
    {

        return _context.Users
            .Include(u => u.Country) // Include the Country in the query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize);

    }

    public async Task<User> CreateAsync(User user, CancellationToken cancellationToken)
    {
        await _context.Users.AddAsync(user, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return user;
    }

    public async Task UpdateAsync(User user, CancellationToken cancellationToken)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var user = await GetByIdAsync(id, cancellationToken);
        if (user != null)
        {
            _context.Users.Remove(user);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
