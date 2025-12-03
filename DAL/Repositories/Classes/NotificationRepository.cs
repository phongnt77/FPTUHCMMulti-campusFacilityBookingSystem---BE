using DAL.Basic;
using DAL.Dbcontext;
using DAL.Models;
using DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories.Classes
{
    public class NotificationRepository : GenericRepository<Notification>, INotificationRepository
    {
        public NotificationRepository(FacilityBookingDbContext context) : base(context)
        {
        }

        public async Task<(List<Notification> items, int total)> GetByUserIdAsync(string userId, int page, int limit)
        {
            var query = _context.Set<Notification>()
                .Where(n => n.UserId == userId)
                .AsQueryable();

            var total = await query.CountAsync();
            var items = await query
                .OrderByDescending(n => n.CreatedAt)
                .Skip((page - 1) * limit)
                .Take(limit)
                .ToListAsync();

            return (items, total);
        }
    }
}

