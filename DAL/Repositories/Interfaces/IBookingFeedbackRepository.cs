using DAL.Models;

namespace DAL.Repositories.Interfaces
{
    public interface IBookingFeedbackRepository
    {
        Task<BookingFeedback?> GetByIdAsync(string id);
        Task<(List<BookingFeedback> items, int total)> GetPagedAsync(int page, int limit);
        Task<int> CreateAsync(BookingFeedback entity);
        Task<int> UpdateAsync(BookingFeedback entity);
        Task<List<BookingFeedback>> GetAllAsync();
    }
}

