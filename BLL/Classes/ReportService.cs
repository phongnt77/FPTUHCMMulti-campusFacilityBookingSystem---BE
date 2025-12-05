using Applications.DTOs.Request;
using Applications.DTOs.Response;
using Applications.Helpers;
using BLL.Interfaces;
using DAL.Dbcontext;
using DAL.Models;
using DAL.Models.Enums;
using DAL.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BLL.Classes
{
    public class ReportService : IReportService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly FacilityBookingDbContext _context;

        public ReportService(IUnitOfWork unitOfWork, FacilityBookingDbContext context)
        {
            _unitOfWork = unitOfWork;
            _context = context;
        }

        public async Task<ApiResponse<UsageReportResponseDto>> GetFacilityUsageReportAsync(UsageReportRequestDto request)
        {
            var fromDate = request.From ?? DateTimeHelper.VietnamNow.AddMonths(-1);
            var toDate = request.To ?? DateTimeHelper.VietnamNow;

            // Get tất cả booking trong time range
            var bookingsQuery = _context.Set<Booking>()
                .Where(b => b.StartTime >= fromDate && b.StartTime <= toDate)
                .AsQueryable();

            // Lọc campus nếu có
            if (!string.IsNullOrEmpty(request.CampusId))
            {
                bookingsQuery = bookingsQuery
                    .Where(b => b.Facility != null && b.Facility.CampusId == request.CampusId);
            }

            // Load các data liên quan
            bookingsQuery = bookingsQuery
                .Include(b => b.Facility)
                    .ThenInclude(f => f!.Campus)
                .Include(b => b.User)
                .Include(b => b.BookingFeedbacks);

            var bookings = await bookingsQuery
                .Include(b => b.Facility)
                .ThenInclude(f => f!.Campus)
                .Include(b => b.User)
                .Include(b => b.BookingFeedbacks)
                .ToListAsync();

            var totalBookings = bookings.Count;
            var approvedBookings = bookings.Count(b => b.Status == BookingStatus.Approved);
            var rejectedBookings = bookings.Count(b => b.Status == BookingStatus.Rejected);
            var completedBookings = bookings.Count(b => b.Status == BookingStatus.Completed);
            var cancelledBookings = bookings.Count(b => b.Status == BookingStatus.Cancelled);

            // Tính tỉ lệ sử dụng (approved bookings / total bookings * 100)
            var utilizationRate = totalBookings > 0 
                ? (decimal)approvedBookings / totalBookings * 100 
                : 0;

            // Tính điểm trung bình từ feedbacks
            var feedbacks = bookings
                .SelectMany(b => b.BookingFeedbacks)
                .Where(f => f.Rating > 0)
                .ToList();
            var avgRating = feedbacks.Any() 
                ? (decimal)feedbacks.Average(f => f.Rating) 
                : 0;

            // Count unique users
            var uniqueUsers = bookings
                .Select(b => b.UserId)
                .Distinct()
                .Count();

            // tìm giờ cao điểm (most bookings by hour)
            var bookingsByHour = bookings
                .GroupBy(b => b.StartTime.Hour)
                .Select(g => new { Hour = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .FirstOrDefault();
            var peakHours = bookingsByHour != null 
                ? $"{bookingsByHour.Hour:00}:00 - {(bookingsByHour.Hour + 1):00}:00" 
                : null;

            // tìm ngày cao điểm (most bookings by day of week)
            var bookingsByDay = bookings
                .GroupBy(b => b.StartTime.DayOfWeek.ToString())
                .Select(g => new { Day = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .FirstOrDefault();
            var busiestDay = bookingsByDay?.Day;

            // Bookings by status
            var bookingsByStatus = bookings
                .GroupBy(b => b.Status.ToString())
                .ToDictionary(g => g.Key, g => g.Count());

            // Bookings by facility
            var bookingsByFacility = bookings
                .Where(b => b.Facility != null)
                .GroupBy(b => b.Facility!.Name)
                .ToDictionary(g => g.Key, g => g.Count());

            // Get campus info
            string? campusName = null;
            if (!string.IsNullOrEmpty(request.CampusId))
            {
                var campus = await _unitOfWork.CampusRepo.GetByIdAsync(request.CampusId);
                campusName = campus?.Name;
            }

            var response = new UsageReportResponseDto
            {
                CampusId = request.CampusId,
                CampusName = campusName,
                ReportPeriodStart = fromDate,
                ReportPeriodEnd = toDate,
                TotalBookings = totalBookings,
                ApprovedBookings = approvedBookings,
                RejectedBookings = rejectedBookings,
                CompletedBookings = completedBookings,
                CancelledBookings = cancelledBookings,
                UtilizationRate = Math.Round(utilizationRate, 2),
                AvgRating = Math.Round(avgRating, 2),
                UniqueUsers = uniqueUsers,
                PeakHours = peakHours,
                BusiestDay = busiestDay,
                BookingsByStatus = bookingsByStatus,
                BookingsByFacility = bookingsByFacility
            };

            return ApiResponse<UsageReportResponseDto>.Ok(response);
        }
    }
}

