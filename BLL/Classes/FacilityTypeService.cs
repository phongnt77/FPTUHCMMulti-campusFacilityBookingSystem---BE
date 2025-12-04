using Applications.DTOs.Request;
using Applications.DTOs.Response;
using Applications.Helpers;
using BLL.Interfaces;
using DAL.Models;
using DAL.Repositories;

namespace BLL.Classes
{
    public class FacilityTypeService : IFacilityTypeService
    {
        private readonly IUnitOfWork _unitOfWork;

        public FacilityTypeService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponseWithPagination<List<FacilityTypeResponseDto>>> GetAllAsync(PagedRequestDto request)
        {
            var (items, total) = await _unitOfWork.FacilityTypeRepo.GetPagedAsync(request.Page, request.Limit);

            var responseDtos = items.Select(ft => new FacilityTypeResponseDto
            {
                TypeId = ft.TypeId,
                Name = ft.Name,
                Description = ft.Description,
                DefaultAmenities = ft.DefaultAmenities,
                DefaultCapacity = ft.DefaultCapacity,
                TypicalDurationHours = ft.TypicalDurationHours,
                IconUrl = ft.IconUrl,
                CreatedAt = ft.CreatedAt,
                UpdatedAt = ft.UpdatedAt
            }).ToList();

            return ApiResponseWithPagination<List<FacilityTypeResponseDto>>.Ok(
                responseDtos,
                request.Page,
                request.Limit,
                total
            );
        }

        public async Task<ApiResponse<FacilityTypeResponseDto>> GetByIdAsync(string id)
        {
            var facilityType = await _unitOfWork.FacilityTypeRepo.GetByIdAsync(id);
            if (facilityType == null)
            {
                return ApiResponse<FacilityTypeResponseDto>.Fail(404, "Không tìm thấy loại cơ sở vật chất.");
            }

            var responseDto = new FacilityTypeResponseDto
            {
                TypeId = facilityType.TypeId,
                Name = facilityType.Name,
                Description = facilityType.Description,
                DefaultAmenities = facilityType.DefaultAmenities,
                DefaultCapacity = facilityType.DefaultCapacity,
                TypicalDurationHours = facilityType.TypicalDurationHours,
                IconUrl = facilityType.IconUrl,
                CreatedAt = facilityType.CreatedAt,
                UpdatedAt = facilityType.UpdatedAt
            };

            return ApiResponse<FacilityTypeResponseDto>.Ok(responseDto);
        }

        public async Task<ApiResponse<FacilityTypeResponseDto>> CreateAsync(CreateFacilityTypeDto dto)
        {
            var typeId = await GenerateTypeIdAsync();

            var facilityType = new FacilityType
            {
                TypeId = typeId,
                Name = dto.Name,
                Description = dto.Description,
                DefaultAmenities = dto.DefaultAmenities,
                DefaultCapacity = dto.DefaultCapacity,
                TypicalDurationHours = dto.TypicalDurationHours,
                IconUrl = dto.IconUrl,
                CreatedAt = DateTimeHelper.VietnamNow,
                UpdatedAt = DateTimeHelper.VietnamNow
            };

            await _unitOfWork.FacilityTypeRepo.CreateAsync(facilityType);

            var responseDto = new FacilityTypeResponseDto
            {
                TypeId = facilityType.TypeId,
                Name = facilityType.Name,
                Description = facilityType.Description,
                DefaultAmenities = facilityType.DefaultAmenities,
                DefaultCapacity = facilityType.DefaultCapacity,
                TypicalDurationHours = facilityType.TypicalDurationHours,
                IconUrl = facilityType.IconUrl,
                CreatedAt = facilityType.CreatedAt,
                UpdatedAt = facilityType.UpdatedAt
            };

            return ApiResponse<FacilityTypeResponseDto>.Ok(responseDto);
        }

        public async Task<ApiResponse<FacilityTypeResponseDto>> UpdateAsync(string id, UpdateFacilityTypeDto dto)
        {
            var facilityType = await _unitOfWork.FacilityTypeRepo.GetByIdAsync(id);
            if (facilityType == null)
            {
                return ApiResponse<FacilityTypeResponseDto>.Fail(404, "Không tìm thấy loại cơ sở vật chất.");
            }

            if (!string.IsNullOrEmpty(dto.Name))
                facilityType.Name = dto.Name;
            if (dto.Description != null)
                facilityType.Description = dto.Description;
            if (dto.DefaultAmenities != null)
                facilityType.DefaultAmenities = dto.DefaultAmenities;
            if (dto.DefaultCapacity.HasValue)
                facilityType.DefaultCapacity = dto.DefaultCapacity;
            if (dto.TypicalDurationHours.HasValue)
                facilityType.TypicalDurationHours = dto.TypicalDurationHours;
            if (dto.IconUrl != null)
                facilityType.IconUrl = dto.IconUrl;

            facilityType.UpdatedAt = DateTimeHelper.VietnamNow;

            await _unitOfWork.FacilityTypeRepo.UpdateAsync(facilityType);

            var responseDto = new FacilityTypeResponseDto
            {
                TypeId = facilityType.TypeId,
                Name = facilityType.Name,
                Description = facilityType.Description,
                DefaultAmenities = facilityType.DefaultAmenities,
                DefaultCapacity = facilityType.DefaultCapacity,
                TypicalDurationHours = facilityType.TypicalDurationHours,
                IconUrl = facilityType.IconUrl,
                CreatedAt = facilityType.CreatedAt,
                UpdatedAt = facilityType.UpdatedAt
            };

            return ApiResponse<FacilityTypeResponseDto>.Ok(responseDto);
        }

        private async Task<string> GenerateTypeIdAsync()
        {
            var types = await _unitOfWork.FacilityTypeRepo.GetAllAsync();
            var maxId = 0;

            foreach (var type in types)
            {
                if (type.TypeId.StartsWith("FT") && type.TypeId.Length == 6)
                {
                    if (int.TryParse(type.TypeId.Substring(2), out var id))
                    {
                        if (id > maxId)
                            maxId = id;
                    }
                }
            }

            return $"FT{(maxId + 1):D4}";
        }
    }
}


