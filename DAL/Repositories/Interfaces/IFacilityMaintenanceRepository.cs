using DAL.Models;

namespace DAL.Repositories.Interfaces
{
    public interface IFacilityMaintenanceRepository
    {
        Task<FacilityMaintenance?> GetByIdAsync(string id);
        Task<(List<FacilityMaintenance> items, int total)> GetPagedAsync(string? facilityId, string? status, int page, int limit);
        Task<int> CreateAsync(FacilityMaintenance entity);
        Task<int> UpdateAsync(FacilityMaintenance entity);
        Task<List<FacilityMaintenance>> GetAllAsync();
    }
}

