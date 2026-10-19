using FreelanceMarketplace.Models;
using Microsoft.EntityFrameworkCore;
namespace FreelanceMarketplace.Data
{
    public class DbInitializer
    {
        public static async Task SeedAsync(AppDbContext context)
        {
            if (!context.Categories.Any())
            {
                var itCategory = new Category
                {
                    Name = "IT và lập trình",
                    SubCategories = new List<Category>
                    {
                        new Category { Name = "Lập trình Web" },
                        new Category { Name = "Lập trình Di động" },
                        new Category { Name = "DevOps & Hệ thống" }
                    }
                };

                var designCategory = new Category
                {
                    Name = "Thiết kế",
                    SubCategories = new List<Category>
                    {
                        new Category { Name = "Thiết kế UI/UX" },
                        new Category { Name = "2D & 3D Animation" }
                    }
                };

                await context.Categories.AddRangeAsync(itCategory, designCategory);
                await context.SaveChangesAsync();
            }
            if (!context.Skills.Any())
            {
                var itCat = context.Categories.FirstOrDefault(c => c.Name == "Lập trình Web")
                         ?? context.Categories.FirstOrDefault(c => c.Name == "IT và lập trình");
                var designCat = context.Categories.FirstOrDefault(c => c.Name == "Thiết kế UI/UX")
                             ?? context.Categories.FirstOrDefault(c => c.Name == "Thiết kế");

                if (itCat != null && designCat != null)
                {
                    var skills = new List<Skill>
                    {
                        new Skill { Name = "C#", Slug = "c-sharp" },
new Skill { Name = ".NET", Slug = "dotnet" },
new Skill { Name = "PostgreSQL", Slug = "postgresql" },
new Skill { Name = "React", Slug = "react" },
new Skill { Name = "Figma", Slug = "figma" },
new Skill { Name = "Adobe Photoshop", Slug = "adobe-photoshop" }
                    };

                    await context.Skills.AddRangeAsync(skills);
                    await context.SaveChangesAsync();
                }
            }
            if (!await context.Users.AnyAsync(u => u.Role == "Admin"))
            {
                var adminUser = new User
                {
                    Email = "admin@freelance.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                    Role = "Admin",
                    IsVerified = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                await context.Users.AddAsync(adminUser);
                await context.SaveChangesAsync();

                var adminProfile = new UserProfile
                {
                    UserId = adminUser.Id,
                    FullName = "Quản trị viên hệ thống",
                    Phone = "0121212121",
                    City = "Hà Nội",
                    Address = "Trụ sở chính",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                await context.UserProfiles.AddAsync(adminProfile);
                await context.SaveChangesAsync();
            }
        }
    }
}
