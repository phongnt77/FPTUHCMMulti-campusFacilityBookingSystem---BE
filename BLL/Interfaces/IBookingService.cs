using Applications.DTOs.Request;
using Applications.DTOs.Response;

namespace BLL.Interfaces
{
    public interface IBookingService
    {
        Task<ApiResponseWithPagination<List<BookingResponseDto>>> GetAllAsync(BookingFilterDto filter);
        Task<ApiResponse<BookingResponseDto>> GetByIdAsync(string id);
        Task<ApiResponse<BookingResponseDto>> CreateAsync(CreateBookingDto dto);
        Task<ApiResponse<BookingResponseDto>> UpdateAsync(string id, UpdateBookingDto dto);
        Task<ApiResponse> CancelAsync(string id, string reason);
    }
}

