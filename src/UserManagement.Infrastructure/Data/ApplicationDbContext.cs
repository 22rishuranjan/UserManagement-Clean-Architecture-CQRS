using Microsoft.EntityFrameworkCore;
using UserManagement.Application.Common.Interfaces;
using UserManagement.Domain.Entities;

namespace UserManagement.Infrastructure.Data;

public class ApplicationDbContext : DbContext, IApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<Otp> Otps { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Country> Countries { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Apply Fluent API Configurations
        ConfigureUserEntity(modelBuilder);
        ConfigureCountryEntity(modelBuilder);

        // Set up the relationship between User and Country (One-to-Many)
        modelBuilder.Entity<User>()
            .HasOne(u => u.Country)
            .WithMany(c => c.Users)
            .HasForeignKey(u => u.CountryId)
            .OnDelete(DeleteBehavior.Cascade);

        base.OnModelCreating(modelBuilder);
    }

    private void ConfigureUserEntity(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("Users");

            // Key
            entity.HasKey(u => u.Id);

            // Properties
            entity.Property(u => u.FirstName)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(u => u.LastName)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(100); // Optional length constraint for email
            entity.HasIndex(u => u.Email) // Add unique index on email
                .IsUnique();

            entity.Property(u => u.DateOfBirth)
                .IsRequired();

            // Relationships
            entity.HasOne(u => u.Country)
                .WithMany(c => c.Users)
                .HasForeignKey(u => u.CountryId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }

    private void ConfigureCountryEntity(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Country>(entity =>
        {
            entity.ToTable("Countries");

            // Key
            entity.HasKey(c => c.Id);

            // Properties
            entity.Property(c => c.Code)
                .IsRequired()
                .HasMaxLength(3);

            entity.Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(100);

            // Relationships
            entity.HasMany(c => c.Users)
                .WithOne(u => u.Country)
                .HasForeignKey(u => u.CountryId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
