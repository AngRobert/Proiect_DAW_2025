using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Proiect_DAW_2025.Data;

namespace Proiect_DAW_2025.Models {
    public static class SeedData {
        public static void Initialize(IServiceProvider serviceProvider) {
            using (var context = new ApplicationDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>())) {
                
                if (context.Roles.Any()) {
                    return;
                }

                context.Roles.AddRange(
                    new IdentityRole { Id = "9b4871f2-2ff2-4606-9c3b-2dd29cc86461", Name = "Admin", NormalizedName = "Admin".ToUpper() },
                    new IdentityRole { Id = "9b4871f2-2ff2-4606-9c3b-2dd29cc86462", Name = "Colaborator", NormalizedName = "Colaborator".ToUpper() },
                    new IdentityRole { Id = "9b4871f2-2ff2-4606-9c3b-2dd29cc86463", Name = "User", NormalizedName = "User".ToUpper() }
                    );

                var hasher = new PasswordHasher<ApplicationUser>();

                context.Users.AddRange(
                    new ApplicationUser {

                        Id = "e6a3ac89-2a22-44b1-bc90-c58469c34c85",
                        UserName = "admin@test.com",
                        EmailConfirmed = true,
                        NormalizedEmail = "ADMIN@TEST.COM",
                        Email = "admin@test.com",
                        NormalizedUserName = "ADMIN@TEST.COM",
                        PasswordHash = hasher.HashPassword(null, "Admin1!")
                    },
                    new ApplicationUser {
                        Id = "e6a3ac89-2a22-44b1-bc90-c58469c34c86",
                        UserName = "colaborator@test.com",
                        EmailConfirmed = true,
                        NormalizedEmail = "COLABORATOR@TEST.COM",
                        Email = "colaborator@test.com",
                        NormalizedUserName = "COLABORATOR@TEST.COM",
                        PasswordHash = hasher.HashPassword(null, "Colaborator1!")
                    },
                    new ApplicationUser {
                        Id = "e6a3ac89-2a22-44b1-bc90-c58469c34c87",
                        UserName = "user@test.com",
                        EmailConfirmed = true,
                        NormalizedEmail = "USER@TEST.COM",
                        Email = "user@test.com",
                        NormalizedUserName = "USER@TEST.COM",
                        PasswordHash = hasher.HashPassword(null, "User1!")
                    });

                context.UserRoles.AddRange(
                    new IdentityUserRole<string> {
                        RoleId = "9b4871f2-2ff2-4606-9c3b-2dd29cc86461",
                        UserId = "e6a3ac89-2a22-44b1-bc90-c58469c34c85"
                    },
                    new IdentityUserRole<string> {
                        RoleId = "9b4871f2-2ff2-4606-9c3b-2dd29cc86462",
                        UserId = "e6a3ac89-2a22-44b1-bc90-c58469c34c86"
                    },
                    new IdentityUserRole<string> {
                        RoleId = "9b4871f2-2ff2-4606-9c3b-2dd29cc86463",
                        UserId = "e6a3ac89-2a22-44b1-bc90-c58469c34c87"
                    });

                context.SaveChanges();
            }
        }
    }
}
