using DAL.Models;

namespace DAL.Repositories.Interfaces
{
    public interface INotificationRepository
    {
        Task<Notification?> GetByIdAsync(string id);
        Task<(List<Notification> items, int total)> GetByUserIdAsync(string userId, int page, int limit);
        Task<int> CreateAsync(Notification entity);
        Task<int> UpdateAsync(Notification entity);
        Task<List<Notification>> GetAllAsync();
    }
}

