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
            await _menuRepository.AddMenuAsync(menu);
        }

        public async Task AtualizarMenuAsync(Menu menu)
        {
            await _menuRepository.UpdateMenuAsync(menu);
        }
    }
}
