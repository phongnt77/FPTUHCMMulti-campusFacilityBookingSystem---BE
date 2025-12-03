using Applications.DTOs.Request;
using Applications.DTOs.Response;
using BLL.Interfaces;
using DAL.Models;
using DAL.Models.Enums;
using DAL.Repositories;

namespace BLL.Classes
{
    public class FacilityService : IFacilityService
    {
        private readonly IUnitOfWork _unitOfWork;

        public FacilityService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponseWithPagination<List<FacilityResponseDto>>> GetAllAsync(FacilityFilterDto filter)
        {
            var (items, total) = await _unitOfWork.FacilityRepo.GetFilteredAsync(
                filter.Name,
                filter.Status,
                filter.TypeId,
                filter.CampusId,
                filter.Page,
                filter.Limit
            );

            var responseDtos = items.Select(f => new FacilityResponseDto
            {
                FacilityId = f.FacilityId,
                Name = f.Name,
                Description = f.Description,
                Capacity = f.Capacity,
                RoomNumber = f.RoomNumber,
                FloorNumber = f.FloorNumber,
                CampusId = f.CampusId,
                CampusName = f.Campus?.Name ?? string.Empty,
                TypeId = f.TypeId,
                TypeName = f.FacilityType?.Name ?? string.Empty,
                Status = f.Status.ToString(),
                Amenities = f.Amenities,
                FacilityManagerId = f.FacilityManagerId,
                MaxConcurrentBookings = f.MaxConcurrentBookings,
                CreatedAt = f.CreatedAt,
                UpdatedAt = f.UpdatedAt
            }).ToList();

            return ApiResponseWithPagination<List<FacilityResponseDto>>.Ok(
                responseDtos,
                filter.Page,
                filter.Limit,
                total
            );
        }

        public async Task<ApiResponse<FacilityResponseDto>> GetByIdAsync(string id)
        {
            var facility = await _unitOfWork.FacilityRepo.GetByIdAsync(id);
            if (facility == null)
            {
                return ApiResponse<FacilityResponseDto>.Fail(404, "Facility not found");
            }

            var responseDto = new FacilityResponseDto
            {
                FacilityId = facility.FacilityId,
                Name = facility.Name,
                Description = facility.Description,
                Capacity = facility.Capacity,
                RoomNumber = facility.RoomNumber,
                FloorNumber = facility.FloorNumber,
                CampusId = facility.CampusId,
                CampusName = facility.Campus?.Name ?? string.Empty,
                TypeId = facility.TypeId,
                TypeName = facility.FacilityType?.Name ?? string.Empty,
                Status = facility.Status.ToString(),
                Amenities = facility.Amenities,
                FacilityManagerId = facility.FacilityManagerId,
                MaxConcurrentBookings = facility.MaxConcurrentBookings,
                CreatedAt = facility.CreatedAt,
                UpdatedAt = facility.UpdatedAt
            };

            return ApiResponse<FacilityResponseDto>.Ok(responseDto);
        }

        public async Task<ApiResponse<FacilityResponseDto>> CreateAsync(CreateFacilityDto dto)
        {
            var facilityId = await GenerateFacilityIdAsync();

            var facility = new Facility
            {
                FacilityId = facilityId,
                Name = dto.Name,
                Description = dto.Description,
                Capacity = dto.Capacity,
                RoomNumber = dto.RoomNumber,
                FloorNumber = dto.FloorNumber,
                CampusId = dto.CampusId,
                TypeId = dto.TypeId,
                Status = Enum.Parse<FacilityStatus>(dto.Status),
                Amenities = dto.Amenities,
                FacilityManagerId = dto.FacilityManagerId,
                MaxConcurrentBookings = dto.MaxConcurrentBookings,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _unitOfWork.FacilityRepo.CreateAsync(facility);

            var responseDto = new FacilityResponseDto
            {
                FacilityId = facility.FacilityId,
                Name = facility.Name,
                Description = facility.Description,
                Capacity = facility.Capacity,
                RoomNumber = facility.RoomNumber,
                FloorNumber = facility.FloorNumber,
                CampusId = facility.CampusId,
                TypeId = facility.TypeId,
                Status = facility.Status.ToString(),
                Amenities = facility.Amenities,
                FacilityManagerId = facility.FacilityManagerId,
                MaxConcurrentBookings = facility.MaxConcurrentBookings,
                CreatedAt = facility.CreatedAt,
                UpdatedAt = facility.UpdatedAt
            };

            return ApiResponse<FacilityResponseDto>.Ok(responseDto);
        }

        public async Task<ApiResponse<FacilityResponseDto>> UpdateAsync(string id, UpdateFacilityDto dto)
        {
            var facility = await _unitOfWork.FacilityRepo.GetByIdAsync(id);
            if (facility == null)
            {
                return ApiResponse<FacilityResponseDto>.Fail(404, "Facility not found");
            }

            if (!string.IsNullOrEmpty(dto.Name))
                facility.Name = dto.Name;
            if (dto.Description != null)
                facility.Description = dto.Description;
            if (dto.Capacity.HasValue)
                facility.Capacity = dto.Capacity.Value;
            if (dto.RoomNumber != null)
                facility.RoomNumber = dto.RoomNumber;
            if (dto.FloorNumber != null)
                facility.FloorNumber = dto.FloorNumber;
            if (!string.IsNullOrEmpty(dto.Status))
                facility.Status = Enum.Parse<FacilityStatus>(dto.Status);
            if (dto.Amenities != null)
                facility.Amenities = dto.Amenities;
            if (dto.FacilityManagerId != null)
                facility.FacilityManagerId = dto.FacilityManagerId;
            if (dto.MaxConcurrentBookings.HasValue)
                facility.MaxConcurrentBookings = dto.MaxConcurrentBookings.Value;

            facility.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.FacilityRepo.UpdateAsync(facility);

            var responseDto = new FacilityResponseDto
            {
                FacilityId = facility.FacilityId,
                Name = facility.Name,
                Description = facility.Description,
                Capacity = facility.Capacity,
                RoomNumber = facility.RoomNumber,
                FloorNumber = facility.FloorNumber,
                CampusId = facility.CampusId,
                TypeId = facility.TypeId,
                Status = facility.Status.ToString(),
                Amenities = facility.Amenities,
                FacilityManagerId = facility.FacilityManagerId,
                MaxConcurrentBookings = facility.MaxConcurrentBookings,
                CreatedAt = facility.CreatedAt,
                UpdatedAt = facility.UpdatedAt
            };

            return ApiResponse<FacilityResponseDto>.Ok(responseDto);
        }

        public async Task<ApiResponse> DeleteAsync(string id)
        {
            var facility = await _unitOfWork.FacilityRepo.GetByIdAsync(id);
            if (facility == null)
            {
                return ApiResponse.Fail(404, "Facility not found");
            }

            // Soft delete: set status to Under_Maintenance (inactive)
            facility.Status = FacilityStatus.Under_Maintenance;
            facility.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.FacilityRepo.UpdateAsync(facility);

            return ApiResponse.Ok();
        }

        private async Task<string> GenerateFacilityIdAsync()
        {
            var facilities = await _unitOfWork.FacilityRepo.GetAllAsync();
            var maxId = 0;

            foreach (var facility in facilities)
            {
                if (facility.FacilityId.StartsWith("F") && facility.FacilityId.Length == 6)
                {
                    if (int.TryParse(facility.FacilityId.Substring(1), out var id))
                    {
                        if (id > maxId)
                            maxId = id;
                    }
                }
            }

            return $"F{(maxId + 1):D5}";
        }
    }
}


