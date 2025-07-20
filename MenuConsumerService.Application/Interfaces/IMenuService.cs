using MenuConsumerService.Domain.Entities;

namespace MenuConsumerService.Application.Interfaces
{
    public interface IMenuService
    {
        Task SalvarMenuAsync(Menu menu);
    }
}
