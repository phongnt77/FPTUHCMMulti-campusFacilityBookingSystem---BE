using Applications.DTOs.Request;
using Applications.DTOs.Response;
using BLL.Interfaces;
using DAL.Models;
using DAL.Models.Enums;
using DAL.Repositories;

namespace BLL.Classes
{
    public class RoleService : IRoleService
    {
        private readonly IUnitOfWork _unitOfWork;

        public RoleService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponseWithPagination<List<RoleResponseDto>>> GetAllAsync(PagedRequestDto request)
        {
            var (items, total) = await _unitOfWork.RoleRepo.GetPagedAsync(request.Page, request.Limit);

            var responseDtos = items.Select(r => new RoleResponseDto
            {
                RoleId = r.RoleId,
                RoleName = r.RoleName,
                CreatedAt = r.CreatedAt,
                UpdatedAt = r.UpdatedAt
            }).ToList();

            return ApiResponseWithPagination<List<RoleResponseDto>>.Ok(
                responseDtos,
                request.Page,
                request.Limit,
                total
            );
        }

        public async Task<ApiResponse<RoleResponseDto>> GetByIdAsync(string id)
        {
            var role = await _unitOfWork.RoleRepo.GetByIdAsync(id);
            if (role == null)
            {
                return ApiResponse<RoleResponseDto>.Fail(404, "Không tìm thấy vai trò.");
            }

            var responseDto = new RoleResponseDto
            {
                RoleId = role.RoleId,
                RoleName = role.RoleName,
                CreatedAt = role.CreatedAt,
                UpdatedAt = role.UpdatedAt
            };

            return ApiResponse<RoleResponseDto>.Ok(responseDto);
        }

        public async Task<ApiResponse<RoleResponseDto>> CreateAsync(CreateRoleDto dto)
        {
            var roleId = await GenerateRoleIdAsync();

            var role = new Role
            {
                RoleId = roleId,
                RoleName = dto.RoleName,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _unitOfWork.RoleRepo.CreateAsync(role);

            var responseDto = new RoleResponseDto
            {
                RoleId = role.RoleId,
                RoleName = role.RoleName,
                CreatedAt = role.CreatedAt,
                UpdatedAt = role.UpdatedAt
            };

            return ApiResponse<RoleResponseDto>.Ok(responseDto);
        }

        public async Task<ApiResponse<RoleResponseDto>> UpdateAsync(string id, UpdateRoleDto dto)
        {
            var role = await _unitOfWork.RoleRepo.GetByIdAsync(id);
            if (role == null)
            {
                return ApiResponse<RoleResponseDto>.Fail(404, "Không tìm thấy vai trò.");
            }

            if (!string.IsNullOrEmpty(dto.RoleName))
                role.RoleName = dto.RoleName;

            role.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.RoleRepo.UpdateAsync(role);

            var responseDto = new RoleResponseDto
            {
                RoleId = role.RoleId,
                RoleName = role.RoleName,
                CreatedAt = role.CreatedAt,
                UpdatedAt = role.UpdatedAt
            };

            return ApiResponse<RoleResponseDto>.Ok(responseDto);
        }

        public Task<ApiResponse> DeleteAsync(string id)
        {
            return Task.FromResult(ApiResponse.Fail(403, "Không thể xóa vai trò hệ thống."));
        }

        private async Task<string> GenerateRoleIdAsync()
        {
            var roles = await _unitOfWork.RoleRepo.GetAllAsync();
            var maxId = 0;

            foreach (var role in roles)
            {
                if (role.RoleId.StartsWith("RL") && role.RoleId.Length == 6)
                {
                    if (int.TryParse(role.RoleId.Substring(2), out var id))
                    {
                        if (id > maxId)
                            maxId = id;
                    }
                }
            }

            return $"RL{(maxId + 1):D4}";
        }
    }
}

