using DAL.Basic;
using DAL.Dbcontext;
using DAL.Models;
using DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories.Classes
{
    public class FacilityImageRepository : GenericRepository<FacilityImage>, IFacilityImageRepository
    {
        public FacilityImageRepository(FacilityBookingDbContext context) : base(context)
        {
        }

        public async Task<List<FacilityImage>> GetByFacilityIdAsync(string facilityId)
        {
            return await _context.Set<FacilityImage>()
                .Where(img => img.FacilityId == facilityId)
                .OrderBy(img => img.ImageOrder)
                .ToListAsync();
        }
    }
}

