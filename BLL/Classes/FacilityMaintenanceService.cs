using Applications.DTOs.Request;
using Applications.DTOs.Response;
using BLL.Interfaces;
using DAL.Models;
using DAL.Models.Enums;
using DAL.Repositories;

namespace BLL.Classes
{
    public class FacilityMaintenanceService : IFacilityMaintenanceService
    {
        private readonly IUnitOfWork _unitOfWork;

        public FacilityMaintenanceService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponseWithPagination<List<MaintenanceResponseDto>>> GetAllAsync(string? facilityId, string? status, PagedRequestDto request)
        {
            var (items, total) = await _unitOfWork.FacilityMaintenanceRepo.GetPagedAsync(facilityId, status, request.Page, request.Limit);

            var responseDtos = items.Select(m => new MaintenanceResponseDto
            {
                MaintenanceId = m.MaintenanceId,
                FacilityId = m.FacilityId,
                FacilityName = m.Facility?.Name ?? string.Empty,
                IssueType = m.IssueType,
                Description = m.Description,
                Priority = m.Priority.ToString(),
                Status = m.Status.ToString(),
                AssignedTo = m.AssignedTo,
                AssignedToName = m.AssignedToUser?.FullName ?? string.Empty,
                ScheduledStart = m.ScheduledStart,
                ScheduledEnd = m.ScheduledEnd,
                ActualStart = m.ActualStart,
                ActualEnd = m.ActualEnd,
                CompletionNotes = m.CompletionNotes,
                CreatedAt = m.CreatedAt
            }).ToList();

            return ApiResponseWithPagination<List<MaintenanceResponseDto>>.Ok(
                responseDtos,
                request.Page,
                request.Limit,
                total
            );
        }

        public async Task<ApiResponse<MaintenanceResponseDto>> GetByIdAsync(string id)
        {
            var maintenance = await _unitOfWork.FacilityMaintenanceRepo.GetByIdAsync(id);
            if (maintenance == null)
            {
                return ApiResponse<MaintenanceResponseDto>.Fail(404, "Không tìm thấy bản ghi bảo trì.");
            }

            var responseDto = new MaintenanceResponseDto
            {
                MaintenanceId = maintenance.MaintenanceId,
                FacilityId = maintenance.FacilityId,
                FacilityName = maintenance.Facility?.Name ?? string.Empty,
                IssueType = maintenance.IssueType,
                Description = maintenance.Description,
                Priority = maintenance.Priority.ToString(),
                Status = maintenance.Status.ToString(),
                AssignedTo = maintenance.AssignedTo,
                AssignedToName = maintenance.AssignedToUser?.FullName ?? string.Empty,
                ScheduledStart = maintenance.ScheduledStart,
                ScheduledEnd = maintenance.ScheduledEnd,
                ActualStart = maintenance.ActualStart,
                ActualEnd = maintenance.ActualEnd,
                CompletionNotes = maintenance.CompletionNotes,
                CreatedAt = maintenance.CreatedAt
            };

            return ApiResponse<MaintenanceResponseDto>.Ok(responseDto);
        }

        public async Task<ApiResponse<MaintenanceResponseDto>> CreateAsync(CreateMaintenanceDto dto)
        {
            var maintenanceId = await GenerateMaintenanceIdAsync();

            var maintenance = new FacilityMaintenance
            {
                MaintenanceId = maintenanceId,
                FacilityId = dto.FacilityId,
                IssueType = dto.IssueType,
                Description = dto.Description,
                Priority = Enum.Parse<MaintenancePriority>(dto.Priority),
                Status = Enum.Parse<MaintenanceStatus>(dto.Status),
                AssignedTo = dto.AssignedTo,
                ScheduledStart = dto.ScheduledStart,
                ScheduledEnd = dto.ScheduledEnd,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.FacilityMaintenanceRepo.CreateAsync(maintenance);

            var responseDto = new MaintenanceResponseDto
            {
                MaintenanceId = maintenance.MaintenanceId,
                FacilityId = maintenance.FacilityId,
                IssueType = maintenance.IssueType,
                Description = maintenance.Description,
                Priority = maintenance.Priority.ToString(),
                Status = maintenance.Status.ToString(),
                AssignedTo = maintenance.AssignedTo,
                ScheduledStart = maintenance.ScheduledStart,
                ScheduledEnd = maintenance.ScheduledEnd,
                CreatedAt = maintenance.CreatedAt
            };

            return ApiResponse<MaintenanceResponseDto>.Ok(responseDto);
        }

        private async Task<string> GenerateMaintenanceIdAsync()
        {
            var maintenances = await _unitOfWork.FacilityMaintenanceRepo.GetAllAsync();
            var maxId = 0;

            foreach (var maintenance in maintenances)
            {
                if (maintenance.MaintenanceId.StartsWith("M") && maintenance.MaintenanceId.Length == 6)
                {
                    if (int.TryParse(maintenance.MaintenanceId.Substring(1), out var id))
                    {
                        if (id > maxId)
                            maxId = id;
                    }
                }
            }

            return $"M{(maxId + 1):D5}";
        }
    }
}

