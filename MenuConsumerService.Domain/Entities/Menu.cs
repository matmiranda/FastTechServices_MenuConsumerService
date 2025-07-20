namespace MenuConsumerService.Domain.Entities
{
    public class Menu
    {
        public decimal Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public decimal Price { get; set; }
        public string MealType { get; set; } = string.Empty;

        public string Available { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; } = DateTime.Now;
    }
}
