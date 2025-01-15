using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserManagement.Domain.Entities;
using UserManagement.Infrastructure.Data;

namespace UserManagement.Infrastructure.Seed
{
    public static class DbContextSeeder
    {
        public static void SeedData(ApplicationDbContext context)
        {
            // Ensure the database exists
            context.Database.EnsureCreated();

            // Seed Countries
            if (!context.Countries.Any())
            {
                context.Countries.AddRange(
                    new Country { Id = 1, Code = "US", Name = "United States" },
                    new Country { Id = 2, Code = "IN", Name = "India" },
                    new Country { Id = 3, Code = "FR", Name = "France" }
                );
            }

            // Seed Users
            if (!context.Users.Any())
            {
                context.Users.AddRange(
                    new User
                    {
                        Id = 1,
                        FirstName = "John",
                        LastName = "Doe",
                        Email = "john.doe@example.com",
                        DateOfBirth = new DateTime(1990, 1, 1),
                        CountryId = 1
                    },
                    new User
                    {
                        Id = 2,
                        FirstName = "Jane",
                        LastName = "Smith",
                        Email = "jane.smith@example.com",
                        DateOfBirth = new DateTime(1985, 5, 20),
                        CountryId = 2
                    }
                );
            }

            // Save changes to database
            context.SaveChanges();
        }
    }

}
