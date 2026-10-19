namespace FreelanceMarketplace.Models
{
    public class Skill
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty; 
        public string Slug { get; set; } = string.Empty;
        public List<Job> Jobs { get; set; } = new();
    }
}
