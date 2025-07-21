namespace MenuConsumerService.Application.DTO
{
    public static class MenuDtoExtensions
    {
        public static Domain.Entities.Menu ToEntity(this MenuConsumerService.Application.DTO.MenuDto dto)
        {
            return new Domain.Entities.Menu
            {
                Id = dto.Id,
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                MealType = dto.MealType,
                Available = dto.Available,
                CreatedAt = dto.CreatedAt,
                UpdatedAt = dto.UpdatedAt
            };
        }
    }
}
