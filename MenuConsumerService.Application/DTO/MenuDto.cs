using System.Text.Json.Serialization;

namespace MenuConsumerService.Application.DTO
{
    namespace MenuConsumerService.Application.DTO
    {
        public class MenuDto
        {
            public Guid Id { get; set; }

            public string Name { get; set; } = string.Empty;

            public string Description { get; set; } = string.Empty;

            public decimal Price { get; set; }

            public string MealType { get; set; } = string.Empty;

            public bool Available { get; set; } = true;

            public DateTime CreatedAt { get; set; }

            public DateTime UpdatedAt { get; set; }

            public string Action { get; set; } = string.Empty;
        }
    }
}
