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
        INSERT INTO menuitems (Id, Name, Description, Price, mealType, Available, CreatedAt, UpdatedAt)
        VALUES (@Id, @Name, @Description, @Price, @MealType, @Available, @CreatedAt, @UpdatedAt);";

            Console.WriteLine($"Item Menu: {menu.Id}, Nome: {menu.Name}, Preço: {menu.Price}, Tipo Comida: {menu.MealType}, Disponibilidade: {menu.Available}, Data Criação: {menu.CreatedAt}, Data Atualização: {menu.UpdatedAt}");

            using var connection = new MySqlConnection(_connectionString);

            await connection.ExecuteAsync(query, menu);            
        }


        public async Task UpdateMenuAsync(Menu menu)
        {
            menu.UpdatedAt = DateTime.Now;

            const string query = @"
        UPDATE menuitems
        SET 
            Name = @Name,
            Description = @Description,
            Price = @Price,
            mealType = @MealType,
            Available = @Available,
            UpdatedAt = @UpdatedAt
        WHERE Id = @Id;";

            Console.WriteLine($"Atualizando Menu: {menu.Id}, Nome: {menu.Name}, Preço: {menu.Price}, Tipo Comida: {menu.MealType}, Disponível: {menu.Available}, Atualizado em: {menu.UpdatedAt}");

            using var connection = new MySqlConnection(_connectionString);
            await connection.ExecuteAsync(query, menu);
        }


    }
}
