using MenuConsumerService.Application.DTO.MenuConsumerService.Application.DTO;
using MenuConsumerService.Domain.Entities;
using System.Text.Json;

namespace MenuConsumerService.Application.DTO
{
    public static class MenuDtoExtensions
    {
        public static Menu ToEntity(this MenuDto dto)
        {
            return new Menu
            {
                Id = dto.Id ?? null,
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                MealTypeId = dto.MealTypeId ?? 0,
                Available = dto.Available,
                ImageUrl = dto.ImageUrl,
                Tags = dto.Tags is not null ? JsonSerializer.Serialize(dto.Tags) : null,
                Calories = dto.Calories ?? 0,
                CreatedAt = dto.CreatedAt,
                UpdatedAt = dto.UpdatedAt
            };
        }

        public static byte MapMealType(string? mealType)
        {
            return mealType?.ToUpperInvariant() switch
            {
                "BEBIDAS" => 1,
                "PRATOS" => 2,
                "SOBREMESAS" => 3,
                _ => 0
            };
        }
    }
}
