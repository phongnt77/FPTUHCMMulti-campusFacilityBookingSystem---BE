using Applications.DTOs.Request;
using Applications.DTOs.Response;
using Applications.Helpers;
using BLL.Interfaces;
using DAL.Models;
using DAL.Models.Enums;
using DAL.Repositories;

namespace BLL.Classes
{
    public class ReportService : IReportService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ReportService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<BookingReportResponseDto>> GetBookingReportAsync(BookingReportFilterDto filter)
        {
            var (startDate, endDate) = CalculatePeriod(filter);
            var periodInfo = new PeriodInfo
            {
                PeriodType = filter.PeriodType ?? "custom",
                StartDate = startDate,
                EndDate = endDate,
                TotalDays = (int)(endDate - startDate).TotalDays + 1
            };

            // Get all bookings in period
            var allBookings = await _unitOfWork.BookingRepo.GetAllAsync();
            var bookingsInPeriod = allBookings
                .Where(b => b.StartTime >= startDate && b.StartTime <= endDate)
                .ToList();

            // Apply filters
            if (!string.IsNullOrEmpty(filter.CampusId))
            {
                var facilities = await _unitOfWork.FacilityRepo.GetAllAsync();
                var facilityIds = facilities
                    .Where(f => f.CampusId == filter.CampusId)
                    .Select(f => f.FacilityId)
                    .ToList();
                bookingsInPeriod = bookingsInPeriod
                    .Where(b => facilityIds.Contains(b.FacilityId))
                    .ToList();
            }

            if (!string.IsNullOrEmpty(filter.FacilityId))
            {
                bookingsInPeriod = bookingsInPeriod
                    .Where(b => b.FacilityId == filter.FacilityId)
                    .ToList();
            }

            // Overall statistics
            var overall = new OverallStatistics
            {
                TotalBookings = bookingsInPeriod.Count,
                ApprovedBookings = bookingsInPeriod.Count(b => b.Status == BookingStatus.Approved),
                RejectedBookings = bookingsInPeriod.Count(b => b.Status == BookingStatus.Rejected),
                CancelledBookings = bookingsInPeriod.Count(b => b.Status == BookingStatus.Cancelled),
                CompletedBookings = bookingsInPeriod.Count(b => b.Status == BookingStatus.Completed),
                PendingBookings = bookingsInPeriod.Count(b => b.Status == BookingStatus.Pending_Approval)
            };

            if (overall.TotalBookings > 0)
            {
                overall.ApprovalRate = (double)overall.ApprovedBookings / overall.TotalBookings * 100;
                overall.CancellationRate = (double)overall.CancelledBookings / overall.TotalBookings * 100;
                overall.CompletionRate = (double)overall.CompletedBookings / overall.TotalBookings * 100;
            }

            // Calculate utilization rate
            var allFacilities = await _unitOfWork.FacilityRepo.GetAllAsync();
            var totalAvailableSlots = CalculateTotalAvailableSlots(allFacilities, startDate, endDate);
            if (totalAvailableSlots > 0)
            {
                overall.UtilizationRate = (double)overall.CompletedBookings / totalAvailableSlots * 100;
            }

            // Daily statistics
            var dailyStats = new List<DailyStatistics>();
            for (var date = startDate.Date; date <= endDate.Date; date = date.AddDays(1))
            {
                var dayBookings = bookingsInPeriod
                    .Where(b => b.StartTime.Date == date)
                    .ToList();

                var dayCompleted = dayBookings.Count(b => b.Status == BookingStatus.Completed);
                var dayUtilization = totalAvailableSlots > 0 
                    ? (double)dayCompleted / (allFacilities.Count * 24) * 100 
                    : 0;

                dailyStats.Add(new DailyStatistics
                {
                    Date = date,
                    TotalBookings = dayBookings.Count,
                    CompletedBookings = dayCompleted,
                    UtilizationRate = dayUtilization
                });
            }

            // Facility statistics
            var facilityStats = new List<FacilityStatistics>();
            var facilityGroups = bookingsInPeriod.GroupBy(b => b.FacilityId);
            
            foreach (var group in facilityGroups)
            {
                var facility = allFacilities.FirstOrDefault(f => f.FacilityId == group.Key);
                if (facility == null) continue;

                var facilityBookings = group.ToList();
                var completed = facilityBookings.Count(b => b.Status == BookingStatus.Completed);
                var utilization = CalculateFacilityUtilization(facility, startDate, endDate, completed);

                // Get average rating from feedbacks using the correct method
                var avgRating = await _unitOfWork.BookingFeedbackRepo.GetAverageFacilityRatingAsync(facility.FacilityId);

                facilityStats.Add(new FacilityStatistics
                {
                    FacilityId = facility.FacilityId,
                    FacilityName = facility.Name,
                    CampusName = facility.Campus?.Name ?? "",
                    TotalBookings = facilityBookings.Count,
                    CompletedBookings = completed,
                    UtilizationRate = utilization,
                    AverageRating = avgRating
                });
            }

            // Campus statistics
            var campusStats = new List<CampusStatistics>();
            var campuses = await _unitOfWork.CampusRepo.GetAllAsync();
            
            foreach (var campus in campuses)
            {
                var campusFacilities = allFacilities.Where(f => f.CampusId == campus.CampusId).ToList();
                var campusFacilityIds = campusFacilities.Select(f => f.FacilityId).ToList();
                var campusBookings = bookingsInPeriod
                    .Where(b => campusFacilityIds.Contains(b.FacilityId))
                    .ToList();

                var completed = campusBookings.Count(b => b.Status == BookingStatus.Completed);
                var utilization = campusFacilities.Any()
                    ? (double)completed / (campusFacilities.Count * CalculateDaysInPeriod(startDate, endDate) * 24) * 100
                    : 0;

                campusStats.Add(new CampusStatistics
                {
                    CampusId = campus.CampusId,
                    CampusName = campus.Name,
                    TotalBookings = campusBookings.Count,
                    CompletedBookings = completed,
                    UtilizationRate = utilization,
                    TotalFacilities = campusFacilities.Count
                });
            }

            var report = new BookingReportResponseDto
            {
                Period = periodInfo,
                Overall = overall,
                DailyStats = dailyStats,
                FacilityStats = facilityStats.OrderByDescending(f => f.UtilizationRate).ToList(),
                CampusStats = campusStats.OrderByDescending(c => c.UtilizationRate).ToList()
            };

            return ApiResponse<BookingReportResponseDto>.Ok(report);
        }

        private (DateTime startDate, DateTime endDate) CalculatePeriod(BookingReportFilterDto filter)
        {
            var now = DateTimeHelper.VietnamNow;

            if (filter.PeriodType == "day" && filter.Days.HasValue)
            {
                var days = filter.Days.Value;
                return (now.Date.AddDays(-days + 1), now.Date);
            }

            if (filter.PeriodType == "week")
            {
                return (now.Date.AddDays(-6), now.Date);
            }

            if (filter.PeriodType == "month" && filter.Month.HasValue && filter.Year.HasValue)
            {
                var start = new DateTime(filter.Year.Value, filter.Month.Value, 1);
                var end = start.AddMonths(1).AddDays(-1);
                return (start, end);
            }

            if (filter.PeriodType == "year" && filter.Year.HasValue)
            {
                var start = new DateTime(filter.Year.Value, 1, 1);
                var end = new DateTime(filter.Year.Value, 12, 31);
                return (start, end);
            }

            // Custom period
            if (filter.StartDate.HasValue && filter.EndDate.HasValue)
            {
                return (filter.StartDate.Value.Date, filter.EndDate.Value.Date);
            }

            // Default: last 30 days
            return (now.Date.AddDays(-29), now.Date);
        }

        private int CalculateTotalAvailableSlots(List<Facility> facilities, DateTime startDate, DateTime endDate)
        {
            var days = CalculateDaysInPeriod(startDate, endDate);
            return facilities.Count(f => f.Status == FacilityStatus.Available) * days * 24; // 24 hours per day
        }

        private double CalculateFacilityUtilization(Facility facility, DateTime startDate, DateTime endDate, int completedBookings)
        {
            if (facility.Status != FacilityStatus.Available) return 0;

            var days = CalculateDaysInPeriod(startDate, endDate);
            var totalSlots = days * 24; // 24 hours per day

            return totalSlots > 0 ? (double)completedBookings / totalSlots * 100 : 0;
        }

        private int CalculateDaysInPeriod(DateTime startDate, DateTime endDate)
        {
            return (int)(endDate - startDate).TotalDays + 1;
        }
    }
}

