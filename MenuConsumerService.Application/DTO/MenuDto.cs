using System.Text.Json.Serialization;

namespace MenuConsumerService.Application.DTO
{
    public class MenuDto
    {
        [JsonPropertyName("id")]
        public decimal Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("description")]
        public string Description { get; set; } = string.Empty;

        [JsonPropertyName("price")]
        public decimal Price { get; set; }

        [JsonPropertyName("meal_type")]
        public string MealType { get; set; } = string.Empty;

        [JsonPropertyName("create_at")]
        public DateTime CreatedAt { get; set; }

        [JsonPropertyName("updated_at")]
        public DateTime UpdatedAt { get; set; }
    }
}
