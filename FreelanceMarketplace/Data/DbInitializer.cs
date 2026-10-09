using FreelanceMarketplace.Models;

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
                        new Skill { Name = "C#", CategoryId = itCat.Id },
                        new Skill { Name = ".NET", CategoryId = itCat.Id },
                        new Skill { Name = "PostgreSQL", CategoryId = itCat.Id },
                        new Skill { Name = "React", CategoryId = itCat.Id },
                        new Skill { Name = "Figma", CategoryId = designCat.Id },
                        new Skill { Name = "Adobe Photoshop", CategoryId = designCat.Id }
                    };

                    await context.Skills.AddRangeAsync(skills);
                    await context.SaveChangesAsync();
                }
            }
        }
    }
}
