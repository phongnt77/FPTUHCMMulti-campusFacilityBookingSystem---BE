using Applications.DTOs.Request;
using Applications.DTOs.Response;
using Applications.Helpers;
using BLL.Interfaces;
using DAL.Models;
using DAL.Repositories;

namespace BLL.Classes
{
    public class NotificationService : INotificationService
    {
        private readonly IUnitOfWork _unitOfWork;

        public NotificationService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponseWithPagination<List<NotificationResponseDto>>> GetByUserIdAsync(string userId, PagedRequestDto request)
        {
            var (items, total) = await _unitOfWork.NotificationRepo.GetByUserIdAsync(userId, request.Page, request.Limit);

            var responseDtos = items.Select(n => new NotificationResponseDto
            {
                NotificationId = n.NotificationId,
                UserId = n.UserId,
                Title = n.Title,
                Message = n.Message,
                RelatedEntityType = n.RelatedEntityType,
                RelatedEntityId = n.RelatedEntityId,
                IsRead = n.IsRead,
                CreatedAt = n.CreatedAt,
                ReadAt = n.ReadAt
            }).ToList();

            return ApiResponseWithPagination<List<NotificationResponseDto>>.Ok(
                responseDtos,
                request.Page,
                request.Limit,
                total
            );
        }

        public async Task<ApiResponse<NotificationResponseDto>> GetByIdAsync(string id)
        {
            var notification = await _unitOfWork.NotificationRepo.GetByIdAsync(id);
            if (notification == null)
            {
                return ApiResponse<NotificationResponseDto>.Fail(404, "Không tìm thấy thông báo.");
            }

            var responseDto = new NotificationResponseDto
            {
                NotificationId = notification.NotificationId,
                UserId = notification.UserId,
                Title = notification.Title,
                Message = notification.Message,
                RelatedEntityType = notification.RelatedEntityType,
                RelatedEntityId = notification.RelatedEntityId,
                IsRead = notification.IsRead,
                CreatedAt = notification.CreatedAt,
                ReadAt = notification.ReadAt
            };

            return ApiResponse<NotificationResponseDto>.Ok(responseDto);
        }

        public async Task<ApiResponse<NotificationResponseDto>> CreateAsync(CreateNotificationDto dto)
        {
            var notificationId = await GenerateNotificationIdAsync();

            var notification = new Notification
            {
                NotificationId = notificationId,
                UserId = dto.UserId,
                Title = dto.Title,
                Message = dto.Message,
                RelatedEntityType = dto.RelatedEntityType,
                RelatedEntityId = dto.RelatedEntityId,
                IsRead = false,
                CreatedAt = DateTimeHelper.VietnamNow
            };

            await _unitOfWork.NotificationRepo.CreateAsync(notification);

            var responseDto = new NotificationResponseDto
            {
                NotificationId = notification.NotificationId,
                UserId = notification.UserId,
                Title = notification.Title,
                Message = notification.Message,
                RelatedEntityType = notification.RelatedEntityType,
                RelatedEntityId = notification.RelatedEntityId,
                IsRead = notification.IsRead,
                CreatedAt = notification.CreatedAt
            };

            return ApiResponse<NotificationResponseDto>.Ok(responseDto);
        }

        public async Task<ApiResponse> MarkAsReadAsync(string id)
        {
            var notification = await _unitOfWork.NotificationRepo.GetByIdAsync(id);
            if (notification == null)
            {
                return ApiResponse.Fail(404, "Không tìm thấy thông báo.");
            }

            notification.IsRead = true;
            notification.ReadAt = DateTimeHelper.VietnamNow;

            await _unitOfWork.NotificationRepo.UpdateAsync(notification);

            return ApiResponse.Ok();
        }

        private async Task<string> GenerateNotificationIdAsync()
        {
            var notifications = await _unitOfWork.NotificationRepo.GetAllAsync();
            var maxId = 0;

            foreach (var notification in notifications)
            {
                if (notification.NotificationId.StartsWith("N") && notification.NotificationId.Length == 6)
                {
                    if (int.TryParse(notification.NotificationId.Substring(1), out var id))
                    {
                        if (id > maxId)
                            maxId = id;
                    }
                }
            }

            return $"N{(maxId + 1):D5}";
        }
    }
}

