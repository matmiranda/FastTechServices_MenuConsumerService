namespace MenuConsumerService.Application.DTO
{
    namespace MenuConsumerService.Application.DTO
    {
        public class MenuDto
        {
            public ulong? Id { get; set; }
            public string Name { get; set; } = string.Empty;
            public string? Description { get; set; }
            public decimal Price { get; set; }
            public string? MealType { get; set; }
            public byte? MealTypeId { get; set; }
            public bool Available { get; set; }
            public string? ImageUrl { get; set; }
            public List<string>? Tags { get; set; }
            public uint? Calories { get; set; }
            public DateTime CreatedAt { get; set; }
            public DateTime UpdatedAt { get; set; }
        }
    }
}
