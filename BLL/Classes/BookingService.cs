using Applications.DTOs.Request;
using Applications.DTOs.Response;
using Applications.Helpers;
using BLL.Interfaces;
using DAL.Models;
using DAL.Models.Enums;
using DAL.Repositories;

namespace BLL.Classes
{
    public class BookingService : IBookingService
    {
        private readonly IUnitOfWork _unitOfWork;

        public BookingService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponseWithPagination<List<BookingResponseDto>>> GetAllAsync(BookingFilterDto filter)
        {
            var (items, total) = await _unitOfWork.BookingRepo.GetPagedAsync(
                filter.UserId,
                filter.FacilityId,
                filter.Status?.ToString(),
                filter.Page,
                filter.Limit
            );

            var responseDtos = items.Select(b => new BookingResponseDto
            {
                BookingId = b.BookingId,
                UserId = b.UserId,
                UserName = b.User?.FullName ?? string.Empty,
                FacilityId = b.FacilityId,
                FacilityName = b.Facility?.Name ?? string.Empty,
                StartTime = b.StartTime,
                EndTime = b.EndTime,
                Purpose = b.Purpose,
                Category = b.Category,
                EstimatedAttendees = b.EstimatedAttendees,
                SpecialRequirements = b.SpecialRequirements,
                Status = b.Status.ToString(),
                ApprovedBy = b.ApprovedBy,
                ApprovedAt = b.ApprovedAt,
                RejectionReason = b.RejectionReason,
                CheckInTime = b.CheckInTime,
                CheckOutTime = b.CheckOutTime,
                IsUsed = b.IsUsed,
                CreatedAt = b.CreatedAt,
                UpdatedAt = b.UpdatedAt
            }).ToList();

            return ApiResponseWithPagination<List<BookingResponseDto>>.Ok(
                responseDtos,
                filter.Page,
                filter.Limit,
                total
            );
        }

        public async Task<ApiResponse<BookingResponseDto>> GetByIdAsync(string id)
        {
            var booking = await _unitOfWork.BookingRepo.GetByIdAsync(id);
            if (booking == null)
            {
                return ApiResponse<BookingResponseDto>.Fail(404, "Không tìm thấy lượt đặt.");
            }

            var responseDto = new BookingResponseDto
            {
                BookingId = booking.BookingId,
                UserId = booking.UserId,
                UserName = booking.User?.FullName ?? string.Empty,
                FacilityId = booking.FacilityId,
                FacilityName = booking.Facility?.Name ?? string.Empty,
                StartTime = booking.StartTime,
                EndTime = booking.EndTime,
                Purpose = booking.Purpose,
                Category = booking.Category,
                EstimatedAttendees = booking.EstimatedAttendees,
                SpecialRequirements = booking.SpecialRequirements,
                Status = booking.Status.ToString(),
                ApprovedBy = booking.ApprovedBy,
                ApprovedAt = booking.ApprovedAt,
                RejectionReason = booking.RejectionReason,
                CheckInTime = booking.CheckInTime,
                CheckOutTime = booking.CheckOutTime,
                IsUsed = booking.IsUsed,
                CreatedAt = booking.CreatedAt,
                UpdatedAt = booking.UpdatedAt
            };

            return ApiResponse<BookingResponseDto>.Ok(responseDto);
        }

        public async Task<ApiResponse<BookingResponseDto>> CreateAsync(CreateBookingDto dto)
        {
            // Check for time conflicts
            var hasConflict = await _unitOfWork.BookingRepo.HasConflictAsync(dto.FacilityId, dto.StartTime, dto.EndTime);
            if (hasConflict)
            {
                return ApiResponse<BookingResponseDto>.Fail(409, "Khung giờ này đã được đặt trước.");
            }

            var bookingId = await GenerateBookingIdAsync();

            var booking = new Booking
            {
                BookingId = bookingId,
                UserId = dto.UserId,
                FacilityId = dto.FacilityId,
                StartTime = dto.StartTime,
                EndTime = dto.EndTime,
                Purpose = dto.Purpose,
                Category = dto.Category,
                EstimatedAttendees = dto.EstimatedAttendees,
                SpecialRequirements = dto.SpecialRequirements,
                Status = BookingStatus.Draft,
                CreatedAt = DateTimeHelper.VietnamNow,
                UpdatedAt = DateTimeHelper.VietnamNow
            };

            await _unitOfWork.BookingRepo.CreateAsync(booking);

            var responseDto = new BookingResponseDto
            {
                BookingId = booking.BookingId,
                UserId = booking.UserId,
                FacilityId = booking.FacilityId,
                StartTime = booking.StartTime,
                EndTime = booking.EndTime,
                Purpose = booking.Purpose,
                Category = booking.Category,
                EstimatedAttendees = booking.EstimatedAttendees,
                SpecialRequirements = booking.SpecialRequirements,
                Status = booking.Status.ToString(),
                CreatedAt = booking.CreatedAt,
                UpdatedAt = booking.UpdatedAt
            };

            return ApiResponse<BookingResponseDto>.Ok(responseDto);
        }

        public async Task<ApiResponse<BookingResponseDto>> UpdateAsync(string id, UpdateBookingDto dto)
        {
            var booking = await _unitOfWork.BookingRepo.GetByIdAsync(id);
            if (booking == null)
            {
                return ApiResponse<BookingResponseDto>.Fail(404, "Không tìm thấy lượt đặt.");
            }

            // Check for time conflicts if time is being updated
            if (dto.StartTime.HasValue || dto.EndTime.HasValue)
            {
                var newStartTime = dto.StartTime ?? booking.StartTime;
                var newEndTime = dto.EndTime ?? booking.EndTime;
                
                var hasConflict = await _unitOfWork.BookingRepo.HasConflictAsync(
                    booking.FacilityId, 
                    newStartTime, 
                    newEndTime, 
                    id
                );
                
                if (hasConflict)
                {
                    return ApiResponse<BookingResponseDto>.Fail(409, "Khung giờ này đã được đặt trước.");
                }

                booking.StartTime = newStartTime;
                booking.EndTime = newEndTime;
            }

            if (dto.Purpose != null)
                booking.Purpose = dto.Purpose;
            if (dto.Category != null)
                booking.Category = dto.Category;
            if (dto.EstimatedAttendees.HasValue)
                booking.EstimatedAttendees = dto.EstimatedAttendees;
            if (dto.SpecialRequirements != null)
                booking.SpecialRequirements = dto.SpecialRequirements;
            if (!string.IsNullOrEmpty(dto.Status))
                booking.Status = Enum.Parse<BookingStatus>(dto.Status);

            booking.UpdatedAt = DateTimeHelper.VietnamNow;

            await _unitOfWork.BookingRepo.UpdateAsync(booking);

            var responseDto = new BookingResponseDto
            {
                BookingId = booking.BookingId,
                UserId = booking.UserId,
                FacilityId = booking.FacilityId,
                StartTime = booking.StartTime,
                EndTime = booking.EndTime,
                Purpose = booking.Purpose,
                Category = booking.Category,
                EstimatedAttendees = booking.EstimatedAttendees,
                SpecialRequirements = booking.SpecialRequirements,
                Status = booking.Status.ToString(),
                ApprovedBy = booking.ApprovedBy,
                ApprovedAt = booking.ApprovedAt,
                CreatedAt = booking.CreatedAt,
                UpdatedAt = booking.UpdatedAt
            };

            return ApiResponse<BookingResponseDto>.Ok(responseDto);
        }

        public async Task<ApiResponse> CancelAsync(string id, string reason)
        {
            var booking = await _unitOfWork.BookingRepo.GetByIdAsync(id);
            if (booking == null)
            {
                return ApiResponse.Fail(404, "Không tìm thấy lượt đặt.");
            }

            booking.Status = BookingStatus.Cancelled;
            booking.CancelledAt = DateTimeHelper.VietnamNow;
            booking.CancellationReason = reason;
            booking.UpdatedAt = DateTimeHelper.VietnamNow;

            await _unitOfWork.BookingRepo.UpdateAsync(booking);

            return ApiResponse.Ok();
        }

        private async Task<string> GenerateBookingIdAsync()
        {
            var bookings = await _unitOfWork.BookingRepo.GetAllAsync();
            var maxId = 0;

            foreach (var booking in bookings)
            {
                if (booking.BookingId.StartsWith("B") && booking.BookingId.Length == 6)
                {
                    if (int.TryParse(booking.BookingId.Substring(1), out var id))
                    {
                        if (id > maxId)
                            maxId = id;
                    }
                }
            }

            return $"B{(maxId + 1):D5}";
        }

        public async Task<ApiResponse<BookingResponseDto>> ApproveBookingAsync(string bookingId, string approverId)
        {
            var booking = await _unitOfWork.BookingRepo.GetByIdAsync(bookingId);
            if (booking == null)
            {
                return ApiResponse<BookingResponseDto>.Fail(404, "Không tìm thấy lượt đặt.");
            }

            // Check if booking is in draft or pending status (can be approved)
            if (booking.Status != BookingStatus.Draft && booking.Status != BookingStatus.Pending_Approval)
            {
                return ApiResponse<BookingResponseDto>.Fail(400, $"Booking cannot be approved. Current status: {booking.Status}");
            }

            // check nếu thời gian không hợp lệ trước khi phê duyệt
            var hasConflict = await _unitOfWork.BookingRepo.HasConflictAsync(
                booking.FacilityId,
                booking.StartTime,
                booking.EndTime,
                bookingId
            );

            if (hasConflict)
            {
                return ApiResponse<BookingResponseDto>.Fail(409, "Cannot approve booking. Time slot conflict detected.");
            }

            // phê duyệt booking
            booking.Status = BookingStatus.Approved;
            booking.ApprovedBy = approverId;
            booking.ApprovedAt = DateTimeHelper.VietnamNow;
            booking.RejectionReason = null;
            booking.UpdatedAt = DateTimeHelper.VietnamNow;

            await _unitOfWork.BookingRepo.UpdateAsync(booking);

            var responseDto = new BookingResponseDto
            {
                BookingId = booking.BookingId,
                UserId = booking.UserId,
                UserName = booking.User?.FullName ?? string.Empty,
                FacilityId = booking.FacilityId,
                FacilityName = booking.Facility?.Name ?? string.Empty,
                StartTime = booking.StartTime,
                EndTime = booking.EndTime,
                Purpose = booking.Purpose,
                Category = booking.Category,
                EstimatedAttendees = booking.EstimatedAttendees,
                SpecialRequirements = booking.SpecialRequirements,
                Status = booking.Status.ToString(),
                ApprovedBy = booking.ApprovedBy,
                ApprovedAt = booking.ApprovedAt,
                RejectionReason = booking.RejectionReason,
                CheckInTime = booking.CheckInTime,
                CheckOutTime = booking.CheckOutTime,
                IsUsed = booking.IsUsed,
                CreatedAt = booking.CreatedAt,
                UpdatedAt = booking.UpdatedAt
            };

            return ApiResponse<BookingResponseDto>.Ok(responseDto);
        }

        public async Task<ApiResponse<BookingResponseDto>> RejectBookingAsync(string bookingId, string approverId, string? reason)
        {
            var booking = await _unitOfWork.BookingRepo.GetByIdAsync(bookingId);
            if (booking == null)
            {
                return ApiResponse<BookingResponseDto>.Fail(404, "Không tìm thấy lượt đặt.");
            }

            // check nếu booking đang draft hoặc pending approval thì có thể bị từ chối
            if (booking.Status != BookingStatus.Draft && booking.Status != BookingStatus.Pending_Approval)
            {
                return ApiResponse<BookingResponseDto>.Fail(400, $"Booking cannot be rejected. Current status: {booking.Status}");
            }

            // từ chối booking
            booking.Status = BookingStatus.Rejected;
            booking.ApprovedBy = approverId;
            booking.ApprovedAt = DateTimeHelper.VietnamNow;
            booking.RejectionReason = reason;
            booking.UpdatedAt = DateTimeHelper.VietnamNow;

            await _unitOfWork.BookingRepo.UpdateAsync(booking);

            var responseDto = new BookingResponseDto
            {
                BookingId = booking.BookingId,
                UserId = booking.UserId,
                UserName = booking.User?.FullName ?? string.Empty,
                FacilityId = booking.FacilityId,
                FacilityName = booking.Facility?.Name ?? string.Empty,
                StartTime = booking.StartTime,
                EndTime = booking.EndTime,
                Purpose = booking.Purpose,
                Category = booking.Category,
                EstimatedAttendees = booking.EstimatedAttendees,
                SpecialRequirements = booking.SpecialRequirements,
                Status = booking.Status.ToString(),
                ApprovedBy = booking.ApprovedBy,
                ApprovedAt = booking.ApprovedAt,
                RejectionReason = booking.RejectionReason,
                CheckInTime = booking.CheckInTime,
                CheckOutTime = booking.CheckOutTime,
                IsUsed = booking.IsUsed,
                CreatedAt = booking.CreatedAt,
                UpdatedAt = booking.UpdatedAt
            };

            return ApiResponse<BookingResponseDto>.Ok(responseDto);
        }
    }
}

