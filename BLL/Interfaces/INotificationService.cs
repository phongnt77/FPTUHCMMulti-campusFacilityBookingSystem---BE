using Applications.DTOs.Request;
using Applications.DTOs.Response;

namespace BLL.Interfaces
{
    public interface INotificationService
    {
        Task<ApiResponseWithPagination<List<NotificationResponseDto>>> GetByUserIdAsync(string userId, PagedRequestDto request);
        Task<ApiResponse<NotificationResponseDto>> GetByIdAsync(string id);
        Task<ApiResponse<NotificationResponseDto>> CreateAsync(CreateNotificationDto dto);
        Task<ApiResponse> MarkAsReadAsync(string id);
    }
}

