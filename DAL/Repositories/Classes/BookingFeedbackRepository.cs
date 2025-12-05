using DAL.Basic;
using DAL.Dbcontext;
using DAL.Models;
using DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories.Classes
{
    public class BookingFeedbackRepository : GenericRepository<BookingFeedback>, IBookingFeedbackRepository
    {
        public BookingFeedbackRepository(FacilityBookingDbContext context) : base(context)
        {
        }

        public async Task<(List<BookingFeedback> items, int total)> GetPagedAsync(int page, int limit)
        {
            var query = _context.Set<BookingFeedback>()
                .Include(f => f.User)
                .Include(f => f.Booking)
                .AsQueryable();

            var total = await query.CountAsync();
            var items = await query
                .OrderByDescending(f => f.CreatedAt)
                .Skip((page - 1) * limit)
                .Take(limit)
                .ToListAsync();

            return (items, total);
        }
    }
}

