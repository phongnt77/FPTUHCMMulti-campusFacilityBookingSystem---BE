using DAL.Basic;
using DAL.Dbcontext;
using DAL.Models;
using DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories.Classes
{
    public class FacilityMaintenanceRepository : GenericRepository<FacilityMaintenance>, IFacilityMaintenanceRepository
    {
        public FacilityMaintenanceRepository(FacilityBookingDbContext context) : base(context)
        {
        }

        public async Task<(List<FacilityMaintenance> items, int total)> GetPagedAsync(string? facilityId, string? status, int page, int limit)
        {
            var query = _context.Set<FacilityMaintenance>()
                .Include(m => m.Facility)
                .Include(m => m.AssignedToUser)
                .AsQueryable();

            if (!string.IsNullOrEmpty(facilityId))
            {
                query = query.Where(m => m.FacilityId == facilityId);
            }

            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(m => m.Status.ToString() == status);
            }

            var total = await query.CountAsync();
            var items = await query
                .OrderByDescending(m => m.CreatedAt)
                .Skip((page - 1) * limit)
                .Take(limit)
                .ToListAsync();

            return (items, total);
        }
    }
}

