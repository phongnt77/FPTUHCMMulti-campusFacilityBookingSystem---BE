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
        private readonly INotificationService _notificationService;
        private readonly ISystemSettingsService _systemSettingsService;

        public BookingService(IUnitOfWork unitOfWork, IMapper mapper, INotificationService notificationService, ISystemSettingsService systemSettingsService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _notificationService = notificationService;
            _systemSettingsService = systemSettingsService;
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
            // validate thời gian hợp lý
            var timeValidation = await ValidateBookingTimeAsync(dto.StartTime, dto.EndTime);
            if (!timeValidation.IsValid)
            {
                return ApiResponse<BookingResponseDto>.Fail(400, timeValidation.ErrorMessage);
            }

            // validate facility tồn tại và có sẵn
            var facility = await _unitOfWork.FacilityRepo.GetByIdAsync(dto.FacilityId);
            if (facility == null)
            {
                return ApiResponse<BookingResponseDto>.Fail(404, "Không tìm thấy facility");
            }

            // check trạng thái facility
            if (facility.Status != FacilityStatus.Available)
            {
                return ApiResponse<BookingResponseDto>.Fail(400, $"Facility đang ở trạng thái {facility.Status}. Vui lòng chọn facility khác hoặc đợi facility sẵn sàng.");
            }

            // check conflict 
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
            booking.Status = BookingStatus.Pending_Approval;
            booking.CreatedAt = DateTimeHelper.VietnamNow;
            booking.UpdatedAt = DateTimeHelper.VietnamNow;

            await _unitOfWork.BookingRepo.CreateAsync(booking);
            await _unitOfWork.SaveChangesAsync();

            // tạo thông báo cho admin
            await _notificationService.CreateBookingPendingApprovalNotificationAsync(bookingId);

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

                // validate thời gian 
                var timeValidation = await ValidateBookingTimeAsync(newStartTime, newEndTime);
                if (!timeValidation.IsValid)
                {
                    return ApiResponse<BookingResponseDto>.Fail(400, timeValidation.ErrorMessage);
                }

                // check conflict 
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

            // tạo thông báo cho user
            await _notificationService.CreateBookingApprovedNotificationAsync(bookingId);

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

            // tạo thông báo cho user
            await _notificationService.CreateBookingRejectedNotificationAsync(bookingId, reason);

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

            // booking đã tự động là Pending_Approval khi tạo, nên chỉ cần check thông báo đã được gửi
            if (booking.Status != BookingStatus.Pending_Approval)
            {
                return ApiResponse<BookingResponseDto>.Fail(400, $"Booking không ở trạng thái Pending_Approval. Trạng thái hiện tại: {booking.Status}.");
            }



            var responseDto = _mapper.Map<BookingResponseDto>(booking);
            return ApiResponse<BookingResponseDto>.Ok(responseDto);
        }


        public async Task<ApiResponse> CancelAsync(string bookingId, string userId, string? reason = null)
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

            if (booking.Status != BookingStatus.Pending_Approval && booking.Status != BookingStatus.Approved)
            {
                return ApiResponse.Fail(400, "Chỉ có thể hủy booking ở trạng thái Pending_Approval hoặc Approved.");
            }

            // chỉ cho phép hủy trước 2 giờ từ StartTime
            var now = DateTimeHelper.VietnamNow;
            var minCancelTime = booking.StartTime.AddHours(-2);
            if (now > minCancelTime)
            {
                return ApiResponse.Fail(400, $"Chỉ có thể hủy booking trước 2 giờ từ thời gian bắt đầu. Thời gian hủy tối đa: {minCancelTime:dd/MM/yyyy HH:mm:ss}.");
            }

            booking.Status = BookingStatus.Cancelled;
            booking.CancelledAt = now;
            booking.CancellationReason = string.IsNullOrEmpty(reason) ? "Hủy bởi người dùng" : reason;
            booking.UpdatedAt = now;

            await _unitOfWork.BookingRepo.UpdateAsync(booking);
            await _unitOfWork.SaveChangesAsync();

            // thông báo cho admin khi user hủy booking
            await _notificationService.CreateBookingCancelledByUserNotificationAsync(bookingId);

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

            // check trạng thái facility
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

            // check conflict
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

            // lấy facility trong cùng campus với capacity tương đương
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
                    // tìm thời gian available tiếp theo
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

            // validate thời gian check-in (lấy từ settings)
            var now = DateTimeHelper.VietnamNow;
            var checkInMinutesBefore = await _systemSettingsService.GetCheckInMinutesBeforeStartAsync();
            var checkInMinutesAfter = await _systemSettingsService.GetCheckInMinutesAfterStartAsync();
            
            var allowedCheckInStart = booking.StartTime.AddMinutes(-checkInMinutesBefore);
            var allowedCheckInEnd = booking.StartTime.AddMinutes(checkInMinutesAfter);
            
            if (now < allowedCheckInStart)
            {
                return ApiResponse<BookingResponseDto>.Fail(400, $"Chỉ có thể check-in từ {checkInMinutesBefore} phút trước thời gian bắt đầu ({allowedCheckInStart:dd/MM/yyyy HH:mm:ss}).");
            }

            if (now > allowedCheckInEnd)
            {
                return ApiResponse<BookingResponseDto>.Fail(400, $"Đã quá thời gian check-in. Thời gian check-in cho phép: từ {allowedCheckInStart:dd/MM/yyyy HH:mm:ss} đến {allowedCheckInEnd:dd/MM/yyyy HH:mm:ss}.");
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

        /// <summary>
        /// Validate thời gian booking hợp lý
        /// </summary>
        private (bool IsValid, string ErrorMessage) ValidateBookingTime(DateTime startTime, DateTime endTime)
        {
            var now = DateTimeHelper.VietnamNow;

            // 1. EndTime phải sau StartTime
            if (endTime <= startTime)
            {
                return (false, "Thời gian kết thúc phải sau thời gian bắt đầu.");
            }

            // 2. Thời gian tối thiểu: 1 giờ
            var duration = endTime - startTime;
            if (duration.TotalHours < 1)
            {
                return (false, "Thời gian booking phải tối thiểu 1 giờ.");
            }

            // 3. Thời gian tối đa: 3 giờ
            if (duration.TotalHours > 3)
            {
                return (false, "Thời gian booking không được vượt quá 3 giờ.");
            }

            // 4. StartTime phải trước ít nhất X giờ (lấy từ settings, mặc định 3 giờ)
            // Note: Method này là sync, nhưng validation thực tế được làm trong ValidateBookingTimeAsync
            // Giữ lại method này để tương thích, nhưng logic chính sẽ dùng async version
            var minStartTime = now.AddHours(-1);
            if (startTime < minStartTime)
            {
                return (false, "Không thể đặt booking trong quá khứ. Thời gian bắt đầu phải từ 1 giờ trước trở đi.");
            }

            // 5. Không được đặt quá xa trong tương lai: 3 tháng
            var maxStartTime = now.AddMonths(3);
            if (startTime > maxStartTime)
            {
                return (false, "Không thể đặt booking quá 3 tháng trong tương lai.");
            }

            // 6. EndTime không được quá xa trong tương lai
            var maxEndTime = now.AddMonths(3).AddDays(1);
            if (endTime > maxEndTime)
            {
                return (false, "Thời gian kết thúc không được vượt quá 3 tháng trong tương lai.");
            }

            return (true, string.Empty);
        }

        /// <summary>
        /// Validate thời gian booking hợp lý (async version với settings)
        /// </summary>
        private async Task<(bool IsValid, string ErrorMessage)> ValidateBookingTimeAsync(DateTime startTime, DateTime endTime)
        {
            var now = DateTimeHelper.VietnamNow;

            // 1. EndTime phải sau StartTime
            if (endTime <= startTime)
            {
                return (false, "Thời gian kết thúc phải sau thời gian bắt đầu.");
            }

            // 2. Thời gian tối thiểu: 1 giờ
            var duration = endTime - startTime;
            if (duration.TotalHours < 1)
            {
                return (false, "Thời gian booking phải tối thiểu 1 giờ.");
            }

            // 3. Thời gian tối đa: 3 giờ
            if (duration.TotalHours > 3)
            {
                return (false, "Thời gian booking không được vượt quá 3 giờ.");
            }

            // 4. StartTime phải trước ít nhất X giờ (lấy từ settings, mặc định 3 giờ)
            var minBookingHours = await _systemSettingsService.GetMinimumBookingHoursBeforeStartAsync();
            var minStartTime = now.AddHours(minBookingHours);
            if (startTime < minStartTime)
            {
                return (false, $"Không thể đặt booking. Thời gian bắt đầu phải trước ít nhất {minBookingHours} giờ từ bây giờ.");
            }

            // 5. Không được đặt quá xa trong tương lai: 3 tháng
            var maxStartTime = now.AddMonths(3);
            if (startTime > maxStartTime)
            {
                return (false, "Không thể đặt booking quá 3 tháng trong tương lai.");
            }

            // 6. EndTime không được quá xa trong tương lai
            var maxEndTime = now.AddMonths(3).AddDays(1);
            if (endTime > maxEndTime)
            {
                return (false, "Thời gian kết thúc không được vượt quá 3 tháng trong tương lai.");
            }

            return (true, string.Empty);
        }

        public async Task ProcessLateCheckInBookingsAsync()
        {
            var now = DateTimeHelper.VietnamNow;
            var checkInMinutesAfter = await _systemSettingsService.GetCheckInMinutesAfterStartAsync();

            // Get bookings that should be cancelled (quá thời gian check-in - sau X phút từ StartTime)
            var bookings = await _unitOfWork.BookingRepo.GetAllAsync();
            var lateCheckInBookings = bookings
                .Where(b => b.Status == BookingStatus.Approved
                    && b.CheckInTime == null
                    && b.StartTime.AddMinutes(checkInMinutesAfter) < now)
                .ToList();

            foreach (var booking in lateCheckInBookings)
            {
                booking.Status = BookingStatus.Cancelled;
                booking.CancelledAt = DateTimeHelper.VietnamNow;
                booking.CancellationReason = "Quá thời gian check-in";
                booking.UpdatedAt = DateTimeHelper.VietnamNow;
                await _unitOfWork.BookingRepo.UpdateAsync(booking);

                // Create notification for user
                var facility = await _unitOfWork.FacilityRepo.GetByIdAsync(booking.FacilityId);
                var notificationId = await GenerateNotificationIdAsync();
                var notification = new Notification
                {
                    NotificationId = notificationId,
                    UserId = booking.UserId,
                    Type = NotificationType.Booking_Cancelled,
                    Title = "Booking đã bị hủy do quá thời gian check-in",
                    Message = $"Booking {booking.BookingId} cho facility {facility?.Name ?? "N/A"} đã bị hủy do quá thời gian check-in ({checkInMinutesAfter} phút sau giờ bắt đầu).",
                    Status = NotificationStatus.Unread,
                    BookingId = booking.BookingId,
                    CreatedAt = now
                };
                await _unitOfWork.NotificationRepo.CreateAsync(notification);
            }

            if (lateCheckInBookings.Any())
            {
                await _unitOfWork.SaveChangesAsync();
            }
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
