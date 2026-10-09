namespace FreelanceMarketplace.Models
{
    public class Skill
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public string Name { get; set; } = string.Empty; 
        public string Slug { get; set; } = string.Empty;
        public Category Category { get; set; } = null!;
        public List<Job> Jobs { get; set; } = new();
    }
}
