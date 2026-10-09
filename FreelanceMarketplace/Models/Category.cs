namespace FreelanceMarketplace.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public int DisplayOrder { get; set; } = 0;
        public int? ParentId { get; set; }
        public Category? Parent { get; set; }
        public List<Category> SubCategories { get; set; } = new();
        public List<Skill> Skills { get; set; } = new();
        public List<Job> Jobs { get; set; } = new();
    }
}
