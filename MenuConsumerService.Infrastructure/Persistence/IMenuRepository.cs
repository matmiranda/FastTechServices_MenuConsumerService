using MenuConsumerService.Domain.Entities;

namespace MenuConsumerService.Infrastructure.Persistence
{
    public interface IMenuRepository
    {
        Task AddMenuAsync(Menu menu);
    }
}
