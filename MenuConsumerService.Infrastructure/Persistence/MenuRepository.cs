using Dapper;
using MenuConsumerService.Domain.Entities;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;

namespace MenuConsumerService.Infrastructure.Persistence
{
    public class MenuRepository(IConfiguration configuration) : IMenuRepository
    {
        private readonly string _connectionString = configuration.GetConnectionString("DefaultConnection");

        public async Task AddMenuAsync(Menu menu)
        {
            menu.CreatedAt = DateTime.UtcNow.AddHours(-3);
            menu.UpdatedAt = DateTime.UtcNow.AddHours(-3);

            const string query = @"
    INSERT INTO menu_db.menu_items (
        name,
        description,
        price,
        meal_type_id,
        available,
        image_url,
        tags,
        calories,
        created_at,
        updated_at
    )
    VALUES (
        @Name,
        @Description,
        @Price,
        @MealTypeId,
        @Available,
        @ImageUrl,
        @Tags,
        @Calories,
        @CreatedAt,
        @UpdatedAt
    );";


            using var connection = new MySqlConnection(_connectionString);
            await connection.ExecuteAsync(query, menu);
        }

        public async Task UpdateMenuAsync(Menu menu)
        {
            menu.UpdatedAt = DateTime.UtcNow.AddHours(-3);

            const string query = @"
    UPDATE menu_db.menu_items
    SET 
        name = @Name,
        description = @Description,
        price = @Price,
        meal_type_id = @MealTypeId,
        available = @Available,
        image_url = @ImageUrl,
        tags = @Tags,
        calories = @Calories,
        updated_at = @UpdatedAt
    WHERE id = @Id;";

            using var connection = new MySqlConnection(_connectionString);
            await connection.ExecuteAsync(query, menu);
        }
    }
}
