using DAL.Basic;
using DAL.Dbcontext;
using DAL.Models;
using DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories.Classes
{
    public class FacilityRepository : GenericRepository<Facility>, IFacilityRepository
    {
        public FacilityRepository(FacilityBookingDbContext context) : base(context)
        {
        }

        public async Task<(List<Facility> items, int total)> GetFilteredAsync(string? name, string? status, string? typeId, string? campusId, int page, int limit)
        {
            var query = _context.Set<Facility>()
                .Include(f => f.Campus)
                .Include(f => f.FacilityType)
                .AsQueryable();

            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(f => f.Name.Contains(name));
            }

            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(f => f.Status.ToString() == status);
            }

            if (!string.IsNullOrEmpty(typeId))
            {
                query = query.Where(f => f.TypeId == typeId);
            }

            if (!string.IsNullOrEmpty(campusId))
            {
                query = query.Where(f => f.CampusId == campusId);
            }

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


