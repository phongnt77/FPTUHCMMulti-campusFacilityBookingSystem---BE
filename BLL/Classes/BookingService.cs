using Applications.DTOs.Request;
using Applications.DTOs.Response;
using Applications.Helpers;
using AutoMapper;
using BLL.Interfaces;
using DAL.Models;
using DAL.Models.Enums;
using DAL.Repositories;

namespace BLL.Classes
{
    public class BookingService : IBookingService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public BookingService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
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

            var responseDtos = _mapper.Map<List<BookingResponseDto>>(items);

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

            var responseDto = _mapper.Map<BookingResponseDto>(booking);
            return ApiResponse<BookingResponseDto>.Ok(responseDto);
        }

        public async Task<ApiResponse<BookingResponseDto>> CreateAsync(CreateBookingDto dto)
        {
            // Validate facility exists and is available
            var facility = await _unitOfWork.FacilityRepo.GetByIdAsync(dto.FacilityId);
            if (facility == null)
            {
                return ApiResponse<BookingResponseDto>.Fail(404, "Không tìm thấy facility");
            }

            // Check facility status
            if (facility.Status != FacilityStatus.Available)
            {
                return ApiResponse<BookingResponseDto>.Fail(400, $"Facility đang ở trạng thái {facility.Status}. Vui lòng chọn facility khác hoặc đợi facility sẵn sàng.");
            }

            // Real-time conflict check
            var hasConflict = await _unitOfWork.BookingRepo.HasConflictAsync(
                dto.FacilityId,
                dto.StartTime,
                dto.EndTime
            );

            if (hasConflict)
            {
                return ApiResponse<BookingResponseDto>.Fail(409, "Facility đã được đặt trong khoảng thời gian này. Vui lòng chọn thời gian khác hoặc facility khác.");
            }

            var bookingId = await GenerateBookingIdAsync();
            
            var booking = _mapper.Map<Booking>(dto);
            booking.BookingId = bookingId;
            booking.Status = BookingStatus.Draft;
            booking.CreatedAt = DateTimeHelper.VietnamNow;
            booking.UpdatedAt = DateTimeHelper.VietnamNow;

            await _unitOfWork.BookingRepo.CreateAsync(booking);
            await _unitOfWork.SaveChangesAsync();

            var responseDto = _mapper.Map<BookingResponseDto>(booking);
            return ApiResponse<BookingResponseDto>.Ok(responseDto);
        }

        public async Task<ApiResponse<BookingResponseDto>> UpdateAsync(string id, UpdateBookingDto dto)
        {
            var booking = await _unitOfWork.BookingRepo.GetByIdAsync(id);
            if (booking == null)
            {
                return ApiResponse<BookingResponseDto>.Fail(404, "Không tìm thấy lượt đặt.");
            }

            // validate facility tồn tại và có sẵn
            var facility = await _unitOfWork.FacilityRepo.GetByIdAsync(booking.FacilityId);
            if (facility == null)
            {
                return ApiResponse<BookingResponseDto>.Fail(404, "Không tìm thấy facility.");
            }

            // check trạng thái của facility
            if (facility.Status != FacilityStatus.Available)
            {
                return ApiResponse<BookingResponseDto>.Fail(400, $"Facility đang ở trạng thái {facility.Status}. Không thể cập nhật booking cho facility đang bảo trì.");
            }

            // check nếu giờ bắt đầu hoặc giờ kết thúc thay đổi
            bool timeChanged = (dto.StartTime.HasValue && dto.StartTime.Value != booking.StartTime) ||
                              (dto.EndTime.HasValue && dto.EndTime.Value != booking.EndTime);

            // nếu giờ bắt đầu hoặc giờ kết thúc thay đổi, check conflict
            if (timeChanged)
            {
                var newStartTime = dto.StartTime ?? booking.StartTime;
                var newEndTime = dto.EndTime ?? booking.EndTime;

                // validate khoảng thời gian
                if (newEndTime <= newStartTime)
                {
                    return ApiResponse<BookingResponseDto>.Fail(400, "Thời gian kết thúc phải sau thời gian bắt đầu.");
                }

                // check conflict thực tế (loại trừ booking hiện tại)
                var hasConflict = await _unitOfWork.BookingRepo.HasConflictAsync(
                    booking.FacilityId,
                    newStartTime,
                    newEndTime,
                    id 
                );

                if (hasConflict)
                {
                    return ApiResponse<BookingResponseDto>.Fail(409, "Facility đã được đặt trong khoảng thời gian này. Vui lòng chọn thời gian khác hoặc facility khác.");
                }
            }

            // update booking fields
            if (dto.StartTime.HasValue)
                booking.StartTime = dto.StartTime.Value;
            if (dto.EndTime.HasValue)
                booking.EndTime = dto.EndTime.Value;
            if (dto.Purpose != null)
                booking.Purpose = dto.Purpose;
            if (dto.Category != null)
                booking.Category = dto.Category;
            if (dto.EstimatedAttendees.HasValue)
                booking.EstimatedAttendees = dto.EstimatedAttendees;
            if (dto.SpecialRequirements != null)
                booking.SpecialRequirements = dto.SpecialRequirements;

            booking.UpdatedAt = DateTimeHelper.VietnamNow;

            await _unitOfWork.BookingRepo.UpdateAsync(booking);
            await _unitOfWork.SaveChangesAsync();

            var responseDto = _mapper.Map<BookingResponseDto>(booking);
            return ApiResponse<BookingResponseDto>.Ok(responseDto);
        }

        public async Task<ApiResponse> DeleteAsync(string id)
        {
            var booking = await _unitOfWork.BookingRepo.GetByIdAsync(id);
            if (booking == null)
            {
                return ApiResponse.Fail(404, "Không tìm thấy lượt đặt.");
            }

            booking.Status = BookingStatus.Cancelled;
            booking.CancelledAt = DateTimeHelper.VietnamNow;
            booking.UpdatedAt = DateTimeHelper.VietnamNow;

            await _unitOfWork.BookingRepo.UpdateAsync(booking);
            await _unitOfWork.SaveChangesAsync();

            return ApiResponse.Ok();
        }

        public async Task<ApiResponse<BookingResponseDto>> ApproveBookingAsync(string bookingId, string approverId)
        {
            var booking = await _unitOfWork.BookingRepo.GetByIdAsync(bookingId);
            if (booking == null)
            {
                return ApiResponse<BookingResponseDto>.Fail(404, "Không tìm thấy booking.");
            }

            if (booking.Status != BookingStatus.Pending_Approval)
            {
                return ApiResponse<BookingResponseDto>.Fail(400, "Chỉ có thể duyệt booking ở trạng thái Pending_Approval.");
            }

            var hasConflict = await _unitOfWork.BookingRepo.HasConflictAsync(
                booking.FacilityId,
                booking.StartTime,
                booking.EndTime,
                bookingId
            );

            if (hasConflict)
            {
                return ApiResponse<BookingResponseDto>.Fail(409, "Facility đã được đặt trong khoảng thời gian này.");
            }

            booking.Status = BookingStatus.Approved;
            booking.ApprovedBy = approverId;
            booking.ApprovedAt = DateTimeHelper.VietnamNow;
            booking.UpdatedAt = DateTimeHelper.VietnamNow;

            await _unitOfWork.BookingRepo.UpdateAsync(booking);
            await _unitOfWork.SaveChangesAsync();

            var responseDto = _mapper.Map<BookingResponseDto>(booking);
            return ApiResponse<BookingResponseDto>.Ok(responseDto);
        }

        public async Task<ApiResponse<BookingResponseDto>> RejectBookingAsync(string bookingId, string approverId, string? reason)
        {
            var booking = await _unitOfWork.BookingRepo.GetByIdAsync(bookingId);
            if (booking == null)
            {
                return ApiResponse<BookingResponseDto>.Fail(404, "Không tìm thấy booking.");
            }

            if (booking.Status != BookingStatus.Pending_Approval)
            {
                return ApiResponse<BookingResponseDto>.Fail(400, "Chỉ có thể từ chối booking ở trạng thái Pending_Approval.");
            }

            booking.Status = BookingStatus.Rejected;
            booking.ApprovedBy = approverId;
            booking.ApprovedAt = DateTimeHelper.VietnamNow;
            booking.RejectionReason = reason;
            booking.UpdatedAt = DateTimeHelper.VietnamNow;

            await _unitOfWork.BookingRepo.UpdateAsync(booking);
            await _unitOfWork.SaveChangesAsync();

            var responseDto = _mapper.Map<BookingResponseDto>(booking);
            return ApiResponse<BookingResponseDto>.Ok(responseDto);
        }

        public async Task<ApiResponse<BookingResponseDto>> SubmitBookingAsync(string bookingId)
        {
            var booking = await _unitOfWork.BookingRepo.GetByIdAsync(bookingId);
            if (booking == null)
            {
                return ApiResponse<BookingResponseDto>.Fail(404, "Không tìm thấy booking.");
            }

            if (booking.Status != BookingStatus.Draft)
            {
                return ApiResponse<BookingResponseDto>.Fail(400, "Chỉ có thể submit booking ở trạng thái Draft.");
            }

            booking.Status = BookingStatus.Pending_Approval;
            booking.UpdatedAt = DateTimeHelper.VietnamNow;

            await _unitOfWork.BookingRepo.UpdateAsync(booking);
            await _unitOfWork.SaveChangesAsync();

            var responseDto = _mapper.Map<BookingResponseDto>(booking);
            return ApiResponse<BookingResponseDto>.Ok(responseDto);
        }


        public async Task<ApiResponse> CancelAsync(string bookingId, string userId)
        {
            var booking = await _unitOfWork.BookingRepo.GetByIdAsync(bookingId);
            if (booking == null)
            {
                return ApiResponse.Fail(404, "Không tìm thấy booking.");
            }

            if (booking.UserId != userId)
            {
                return ApiResponse.Fail(403, "Bạn không có quyền hủy booking này.");
            }

            if (booking.Status != BookingStatus.Draft && booking.Status != BookingStatus.Pending_Approval && booking.Status != BookingStatus.Approved)
            {
                return ApiResponse.Fail(400, "Chỉ có thể hủy booking ở trạng thái Draft, Pending_Approval hoặc Approved.");
            }

            booking.Status = BookingStatus.Cancelled;
            booking.CancelledAt = DateTimeHelper.VietnamNow;
            booking.CancellationReason = "Hủy bởi người dùng";
            booking.UpdatedAt = DateTimeHelper.VietnamNow;

            await _unitOfWork.BookingRepo.UpdateAsync(booking);
            await _unitOfWork.SaveChangesAsync();

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

        public async Task<ApiResponse<AvailabilityCheckResponseDto>> CheckAvailabilityAsync(CheckAvailabilityDto dto)
        {
            var facility = await _unitOfWork.FacilityRepo.GetByIdWithDetailsAsync(dto.FacilityId);
            if (facility == null)
            {
                return ApiResponse<AvailabilityCheckResponseDto>.Fail(404, "Không tìm thấy facility");
            }

            var response = new AvailabilityCheckResponseDto
            {
                FacilityStatus = facility.Status.ToString()
            };

            // Check facility status
            if (facility.Status != FacilityStatus.Available)
            {
                response.IsAvailable = false;
                response.ConflictReason = $"Facility đang ở trạng thái {facility.Status}";
                
                // Get alternative facilities
                var alternatives = await GetAlternativeFacilitiesAsync(
                    dto.FacilityId,
                    dto.StartTime,
                    dto.EndTime,
                    facility.Capacity
                );
                response.AlternativeFacilities = alternatives.Data ?? new List<AlternativeFacilityDto>();
                
                return ApiResponse<AvailabilityCheckResponseDto>.Ok(response);
            }

            // Check time conflict
            var hasConflict = await _unitOfWork.BookingRepo.HasConflictAsync(
                dto.FacilityId,
                dto.StartTime,
                dto.EndTime,
                dto.ExcludeBookingId
            );

            if (hasConflict)
            {
                response.IsAvailable = false;
                response.ConflictReason = "Facility đã được đặt trong khoảng thời gian này";
                
                // Get alternative facilities
                var alternatives = await GetAlternativeFacilitiesAsync(
                    dto.FacilityId,
                    dto.StartTime,
                    dto.EndTime,
                    facility.Capacity
                );
                response.AlternativeFacilities = alternatives.Data ?? new List<AlternativeFacilityDto>();
                
                return ApiResponse<AvailabilityCheckResponseDto>.Ok(response);
            }

            response.IsAvailable = true;
            return ApiResponse<AvailabilityCheckResponseDto>.Ok(response);
        }

        public async Task<ApiResponse<List<AlternativeFacilityDto>>> GetAlternativeFacilitiesAsync(
            string facilityId,
            DateTime startTime,
            DateTime endTime,
            int capacity)
        {
            var originalFacility = await _unitOfWork.FacilityRepo.GetByIdWithDetailsAsync(facilityId);
            if (originalFacility == null)
            {
                return ApiResponse<List<AlternativeFacilityDto>>.Fail(404, "Không tìm thấy facility");
            }

            // Get facilities in same campus with similar capacity
            var facilities = await _unitOfWork.FacilityRepo.GetFilteredAsync(
                null,
                FacilityStatus.Available.ToString(),
                originalFacility.TypeId,
                originalFacility.CampusId,
                1,
                20
            );

            var alternatives = new List<AlternativeFacilityDto>();

            foreach (var facility in facilities.items.Where(f => f.FacilityId != facilityId && f.Capacity >= capacity))
            {
                var hasConflict = await _unitOfWork.BookingRepo.HasConflictAsync(
                    facility.FacilityId,
                    startTime,
                    endTime
                );

                if (!hasConflict)
                {
                    alternatives.Add(new AlternativeFacilityDto
                    {
                        FacilityId = facility.FacilityId,
                        Name = facility.Name,
                        CampusName = facility.Campus.Name,
                        Capacity = facility.Capacity,
                        RoomNumber = facility.RoomNumber,
                        FloorNumber = facility.FloorNumber,
                        IsAvailable = true,
                        NextAvailableTime = null
                    });
                }
                else
                {
                    // Find next available time
                    var nextAvailable = await FindNextAvailableTimeAsync(
                        facility.FacilityId,
                        startTime,
                        endTime
                    );

                    alternatives.Add(new AlternativeFacilityDto
                    {
                        FacilityId = facility.FacilityId,
                        Name = facility.Name,
                        CampusName = facility.Campus.Name,
                        Capacity = facility.Capacity,
                        RoomNumber = facility.RoomNumber,
                        FloorNumber = facility.FloorNumber,
                        IsAvailable = false,
                        NextAvailableTime = nextAvailable
                    });
                }
            }

            return ApiResponse<List<AlternativeFacilityDto>>.Ok(alternatives.OrderBy(a => a.IsAvailable ? 0 : 1).ToList());
        }

        private async Task<DateTime?> FindNextAvailableTimeAsync(string facilityId, DateTime startTime, DateTime endTime)
        {
            var duration = endTime - startTime;
            var checkTime = endTime;

            // Check next 7 days
            for (int i = 0; i < 7; i++)
            {
                var nextStart = checkTime.AddDays(i);
                var nextEnd = nextStart.Add(duration);

                var hasConflict = await _unitOfWork.BookingRepo.HasConflictAsync(
                    facilityId,
                    nextStart,
                    nextEnd
                );

                if (!hasConflict)
                {
                    return nextStart;
                }
            }

            return null;
        }

        public async Task<ApiResponse<BookingResponseDto>> CheckInAsync(string bookingId, string userId)
        {
            var booking = await _unitOfWork.BookingRepo.GetByIdAsync(bookingId);
            if (booking == null)
            {
                return ApiResponse<BookingResponseDto>.Fail(404, "Không tìm thấy booking.");
            }

            // validate booking thuộc về user
            if (booking.UserId != userId)
            {
                return ApiResponse<BookingResponseDto>.Fail(403, "Bạn không có quyền check-in booking này.");
            }

            // validate status phải là Approved
            if (booking.Status != BookingStatus.Approved)
            {
                return ApiResponse<BookingResponseDto>.Fail(400, $"Chỉ có thể check-in booking ở trạng thái Approved. Trạng thái hiện tại: {booking.Status}.");
            }

            // validate chưa check-in
            if (booking.CheckInTime.HasValue)
            {
                return ApiResponse<BookingResponseDto>.Fail(400, "Booking đã được check-in trước đó.");
            }

            // validate thời gian check-in (cho phép check-in từ 15 phút trước StartTime đến EndTime)
            var now = DateTimeHelper.VietnamNow;
            var allowedCheckInStart = booking.StartTime.AddMinutes(-15);
            if (now < allowedCheckInStart)
            {
                return ApiResponse<BookingResponseDto>.Fail(400, $"Chỉ có thể check-in từ 15 phút trước thời gian bắt đầu ({allowedCheckInStart:dd/MM/yyyy HH:mm}).");
            }

            if (now > booking.EndTime)
            {
                return ApiResponse<BookingResponseDto>.Fail(400, $"Không thể check-in sau thời gian kết thúc ({booking.EndTime:dd/MM/yyyy HH:mm}).");
            }

            // set check-in time
            booking.CheckInTime = now;
            booking.UpdatedAt = now;

            await _unitOfWork.BookingRepo.UpdateAsync(booking);
            await _unitOfWork.SaveChangesAsync();

            var responseDto = _mapper.Map<BookingResponseDto>(booking);
            return ApiResponse<BookingResponseDto>.Ok(responseDto);
        }

        public async Task<ApiResponse<BookingResponseDto>> CheckOutAsync(string bookingId, string userId)
        {
            var booking = await _unitOfWork.BookingRepo.GetByIdAsync(bookingId);
            if (booking == null)
            {
                return ApiResponse<BookingResponseDto>.Fail(404, "Không tìm thấy booking.");
            }

            // validate booking thuộc về user
            if (booking.UserId != userId)
            {
                return ApiResponse<BookingResponseDto>.Fail(403, "Bạn không có quyền check-out booking này.");
            }

            // validate đã check-in
            if (!booking.CheckInTime.HasValue)
            {
                return ApiResponse<BookingResponseDto>.Fail(400, "Phải check-in trước khi check-out.");
            }

            // validate chưa check-out
            if (booking.CheckOutTime.HasValue)
            {
                return ApiResponse<BookingResponseDto>.Fail(400, "Booking đã được check-out trước đó.");
            }

            // validate thời gian check-out phải sau CheckInTime
            var now = DateTimeHelper.VietnamNow;
            if (now < booking.CheckInTime.Value)
            {
                return ApiResponse<BookingResponseDto>.Fail(400, "Thời gian check-out phải sau thời gian check-in.");
            }

            // set check-out time
            booking.CheckOutTime = now;
            booking.UpdatedAt = now;

            // nếu check-out sau EndTime, set status = Completed
            if (now >= booking.EndTime)
            {
                booking.Status = BookingStatus.Completed;
            }

            await _unitOfWork.BookingRepo.UpdateAsync(booking);
            await _unitOfWork.SaveChangesAsync();

            var responseDto = _mapper.Map<BookingResponseDto>(booking);
            return ApiResponse<BookingResponseDto>.Ok(responseDto);
        }
    }
}
