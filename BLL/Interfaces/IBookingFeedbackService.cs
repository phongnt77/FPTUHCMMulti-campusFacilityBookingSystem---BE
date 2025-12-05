using Applications.DTOs.Request;
using Applications.DTOs.Response;

namespace BLL.Interfaces
{
    public interface IBookingFeedbackService
    {
        Task<ApiResponseWithPagination<List<BookingFeedbackResponseDto>>> GetAllAsync(PagedRequestDto request);
        Task<ApiResponse<BookingFeedbackResponseDto>> GetByIdAsync(string id);
        Task<ApiResponse<BookingFeedbackResponseDto>> CreateAsync(CreateBookingFeedbackDto dto);
    }
}

