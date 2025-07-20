using MenuConsumerService.Application.Interfaces;
using MenuConsumerService.Domain.Entities;
using MenuConsumerService.Infrastructure.Persistence;

namespace MenuConsumerService.Infrastructure.Services
{
    public class MenuService : IMenuService
    {
        private readonly IMenuRepository _menuRepository;
        public MenuService(IMenuRepository menuRepository)
        {
            _menuRepository = menuRepository;
        }
        public async Task SalvarMenuAsync(Menu menu)
        {
            // Garantir que a DataCriacao e DataAlteracao sejam atualizadas antes de salvar
            menu.CreatedAt = DateTime.Now;
            menu.UpdatedAt = DateTime.Now;
            await _menuRepository.AddMenuAsync(menu);
        }
    }
}
