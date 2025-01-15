
using UserManagement.Application.Common.Interfaces;
using UserManagement.Domain.Entities;
using UserManagement.Infrastructure.Data;

namespace UserManagement.Infrastructure.Repositories;

public class CountryRepository : ICountryRepository
{
    private readonly ApplicationDbContext _context;

    public CountryRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Country> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        return await _context.Countries.FindAsync(id, CancellationToken.None);
    }

    public IQueryable<Country> GetAllAsync(int pageNumber, int pageSize)
    {
        return _context.Countries
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize);
    }

    public async Task<Country> CreateAsync(Country country, CancellationToken cancellationToken)
    {
        await _context.Countries.AddAsync(country, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return country;
    }

    public async Task UpdateAsync(Country country, CancellationToken cancellationToken)
    {
        _context.Countries.Update(country);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var country = await GetByIdAsync(id, cancellationToken);
        if (country != null)
        {
            _context.Countries.Remove(country);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
