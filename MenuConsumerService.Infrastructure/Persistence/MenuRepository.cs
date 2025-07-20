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
            // Garantir que a DataCriacao e DataAlteracao sejam atualizadas antes de salvar
            menu.CreatedAt = DateTime.Now;

            const string query = @"
            INSERT INTO menu_items  (name, description, price, meal_type, available, created_at, updated_at) 
            VALUES (@Name, @Description, @Price, @MealType, @Available, @CreatedAt, @UpdatedAt);";

            Console.WriteLine($"Item Menu: {menu.Id}, Nome: {menu.Name}, Preço: {menu.Price}, Tipo Comida: {menu.MealType}, Disponibilidade: {menu.Available}, Data Criação: {menu.CreatedAt}, Data Atualização: {menu.UpdatedAt}");

            using var connection = new MySqlConnection(_connectionString);

            await connection.ExecuteAsync(query, menu);
        }
    }
}
