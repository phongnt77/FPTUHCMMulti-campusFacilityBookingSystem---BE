using DAL.Models;

namespace DAL.Repositories.Interfaces
{
    public interface IFacilityImageRepository
    {
        Task<FacilityImage?> GetByIdAsync(string id);
        Task<List<FacilityImage>> GetByFacilityIdAsync(string facilityId);
        Task<int> CreateAsync(FacilityImage entity);
        Task<bool> RemoveAsync(FacilityImage entity);
        Task<List<FacilityImage>> GetAllAsync();
    }
}

