using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Proiect_DAW_2025.Data;

namespace Proiect_DAW_2025.Models
{
    public static class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using var context = new ApplicationDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>());

            if (!context.Roles.Any())
            {
                context.Roles.AddRange(
                    new IdentityRole { Id = "9b4871f2-2ff2-4606-9c3b-2dd29cc86461", Name = "Admin", NormalizedName = "Admin".ToUpper() },
                    new IdentityRole { Id = "9b4871f2-2ff2-4606-9c3b-2dd29cc86462", Name = "Colaborator", NormalizedName = "Colaborator".ToUpper() },
                    new IdentityRole { Id = "9b4871f2-2ff2-4606-9c3b-2dd29cc86463", Name = "User", NormalizedName = "User".ToUpper() }
                    );

                var hasher = new PasswordHasher<ApplicationUser>();

                context.Users.AddRange(
                    new ApplicationUser
                    {

                        Id = "e6a3ac89-2a22-44b1-bc90-c58469c34c85",
                        UserName = "admin@test.com",
                        EmailConfirmed = true,
                        NormalizedEmail = "ADMIN@TEST.COM",
                        Email = "admin@test.com",
                        NormalizedUserName = "ADMIN@TEST.COM",
                        PasswordHash = hasher.HashPassword(null, "Admin1!")
                    },
                    new ApplicationUser
                    {
                        Id = "e6a3ac89-2a22-44b1-bc90-c58469c34c86",
                        UserName = "colaborator@test.com",
                        EmailConfirmed = true,
                        NormalizedEmail = "COLABORATOR@TEST.COM",
                        Email = "colaborator@test.com",
                        NormalizedUserName = "COLABORATOR@TEST.COM",
                        PasswordHash = hasher.HashPassword(null, "Colaborator1!")
                    },
                    new ApplicationUser
                    {
                        Id = "e6a3ac89-2a22-44b1-bc90-c58469c34c87",
                        UserName = "user@test.com",
                        EmailConfirmed = true,
                        NormalizedEmail = "USER@TEST.COM",
                        Email = "user@test.com",
                        NormalizedUserName = "USER@TEST.COM",
                        PasswordHash = hasher.HashPassword(null, "User1!")
                    });

                context.UserRoles.AddRange(
                    new IdentityUserRole<string>
                    {
                        RoleId = "9b4871f2-2ff2-4606-9c3b-2dd29cc86461",
                        UserId = "e6a3ac89-2a22-44b1-bc90-c58469c34c85"
                    },
                    new IdentityUserRole<string>
                    {
                        RoleId = "9b4871f2-2ff2-4606-9c3b-2dd29cc86462",
                        UserId = "e6a3ac89-2a22-44b1-bc90-c58469c34c86"
                    },
                    new IdentityUserRole<string>
                    {
                        RoleId = "9b4871f2-2ff2-4606-9c3b-2dd29cc86463",
                        UserId = "e6a3ac89-2a22-44b1-bc90-c58469c34c87"
                    });
            }

            if (!context.Categories.Any())
            {
                var catElectronics = new Category { CategoryName = "Electronice" };
                var catFashion = new Category { CategoryName = "Fashion" };
                var catHome = new Category { CategoryName = "Casă și grădină" };

                context.Categories.AddRange(catElectronics, catFashion, catHome);
                context.SaveChanges();

                var prod1 = new Product
                {
                    Title = "Laptop Gaming XZ500",
                    Description = "Un laptop performant pentru gaming și productivitate.",
                    Price = 4500.00m,
                    Stock = 10,
                    CategoryId = catElectronics.Id,
                    CollaboratorId = "e6a3ac89-2a22-44b1-bc90-c58469c34c86",
                };

                var prod2 = new Product
                {
                    Title = "Smartphone Ultra 2025",
                    Description = "Cea mai bună cameră foto de pe piață.",
                    Price = 3200.99m,
                    Stock = 25,
                    CategoryId = catElectronics.Id,
                    CollaboratorId = "e6a3ac89-2a22-44b1-bc90-c58469c34c86",
                };

                var prod3 = new Product
                {
                    Title = "Tricou bumbac organic",
                    Description = "Tricou confortabil, disponibil în mai multe culori.",
                    Price = 89.99m,
                    Stock = 100,
                    CategoryId = catFashion.Id,
                    CollaboratorId = "e6a3ac89-2a22-44b1-bc90-c58469c34c86",
                };

                var prod4 = new Product
                {
                    Title = "Rochie de vară florală",
                    Description = "Perfectă pentru zilele călduroase.",
                    Price = 150.00m,
                    Stock = 15,
                    CategoryId = catFashion.Id,
                    CollaboratorId = "e6a3ac89-2a22-44b1-bc90-c58469c34c86",
                };

                var prod5 = new Product
                {
                    Title = "Canapea extensibilă",
                    Description = "Confort maxim și design modern pentru livingul tău.",
                    Price = 1200.50m,
                    Stock = 5,
                    CategoryId = catHome.Id,
                    CollaboratorId = "e6a3ac89-2a22-44b1-bc90-c58469c34c86",
                };

                context.Products.AddRange(prod1, prod2, prod3, prod4, prod5);
                context.SaveChanges();

                context.Reviews.AddRange(
                    new Review
                    {
                        Text = "Un produs excelent, recomand!",
                        Rating = 5,
                        Date = new DateTime(2025, 11, 25),
                        ProductId = prod1.Id,
                        UserId = "e6a3ac89-2a22-44b1-bc90-c58469c34c87"
                    },
                    new Review
                    {
                        Text = "Bateria ține puțin, dar pozele sunt super.",
                        Rating = 4,
                        Date = new DateTime(2025, 11, 27),
                        ProductId = prod2.Id,
                        UserId = "e6a3ac89-2a22-44b1-bc90-c58469c34c87"
                    },
                    new Review
                    {
                        Text = "Material de calitate.",
                        Rating = 5,
                        Date = new DateTime(2025, 11, 5),
                        ProductId = prod3.Id,
                        UserId = "e6a3ac89-2a22-44b1-bc90-c58469c34c87"
                    },
                    new Review
                    {
                        Text = "Mărimea nu corespunde.",
                        Rating = 2,
                        Date = new DateTime(2025, 7, 11),
                        ProductId = prod4.Id,
                        UserId = "e6a3ac89-2a22-44b1-bc90-c58469c34c87"
                    },
                    new Review
                    {
                        Text = "Foarte comodă.",
                        Rating = 5,
                        Date = new DateTime(2025, 7, 25),
                        ProductId = prod5.Id,
                        UserId = "e6a3ac89-2a22-44b1-bc90-c58469c34c87"
                    }
                );

                context.SaveChanges();
            }
        }
    }
}