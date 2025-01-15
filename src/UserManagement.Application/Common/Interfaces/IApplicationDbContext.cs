
using Microsoft.EntityFrameworkCore;
using UserManagement.Domain.Entities;

namespace UserManagement.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Otp> Otps { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
