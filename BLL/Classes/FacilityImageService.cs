using Applications.DTOs.Request;
using Applications.DTOs.Response;
using BLL.Interfaces;
using DAL.Models;
using DAL.Repositories;

namespace BLL.Classes
{
    public class FacilityImageService : IFacilityImageService
    {
        private readonly IUnitOfWork _unitOfWork;

        public FacilityImageService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<List<FacilityImageResponseDto>>> GetByFacilityIdAsync(string facilityId)
        {
            var images = await _unitOfWork.FacilityImageRepo.GetByFacilityIdAsync(facilityId);

            var responseDtos = images.Select(img => new FacilityImageResponseDto
            {
                ImageId = img.ImageId,
                FacilityId = img.FacilityId,
                ImageUrl = img.ImageUrl,
                Description = img.Description,
                UploadDate = img.UploadDate,
                ImageOrder = img.ImageOrder
            }).ToList();

            return ApiResponse<List<FacilityImageResponseDto>>.Ok(responseDtos);
        }

        public async Task<ApiResponse<FacilityImageResponseDto>> CreateAsync(CreateFacilityImageDto dto)
        {
            var imageId = await GenerateImageIdAsync();

            var image = new FacilityImage
            {
                ImageId = imageId,
                FacilityId = dto.FacilityId,
                ImageUrl = dto.ImageUrl,
                Description = dto.Description,
                ImageOrder = dto.ImageOrder,
                UploadDate = DateTime.UtcNow
            };

            await _unitOfWork.FacilityImageRepo.CreateAsync(image);

            var responseDto = new FacilityImageResponseDto
            {
                ImageId = image.ImageId,
                FacilityId = image.FacilityId,
                ImageUrl = image.ImageUrl,
                Description = image.Description,
                UploadDate = image.UploadDate,
                ImageOrder = image.ImageOrder
            };

            return ApiResponse<FacilityImageResponseDto>.Ok(responseDto);
        }

        public async Task<ApiResponse> DeleteAsync(string id)
        {
            var image = await _unitOfWork.FacilityImageRepo.GetByIdAsync(id);
            if (image == null)
            {
                return ApiResponse.Fail(404, "Không tìm thấy hình ảnh.");
            }

            await _unitOfWork.FacilityImageRepo.RemoveAsync(image);

            return ApiResponse.Ok();
        }

        private async Task<string> GenerateImageIdAsync()
        {
            var images = await _unitOfWork.FacilityImageRepo.GetAllAsync();
            var maxId = 0;

            foreach (var image in images)
            {
                if (image.ImageId.StartsWith("IMG") && image.ImageId.Length == 6)
                {
                    if (int.TryParse(image.ImageId.Substring(3), out var id))
                    {
                        if (id > maxId)
                            maxId = id;
                    }
                }
            }

            return $"IMG{(maxId + 1):D3}";
        }
    }
}

