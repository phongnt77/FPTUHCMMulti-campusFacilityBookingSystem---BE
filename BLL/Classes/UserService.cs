using Applications.DTOs.Request;
using Applications.DTOs.Response;
using Applications.Helpers;
using BLL.Interfaces;
using DAL.Models.Enums;
using DAL.Repositories;

namespace BLL.Classes
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;

        public UserService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<UserResponseDto?> GetProfileAsync(string userId)
        {
            var user = await _unitOfWork.UserRepo.GetByIdAsync(userId);

            if (user == null)
            {
                return null;
            }

            return new UserResponseDto
            {
                UserId = user.UserId,
                Email = user.Email,
                FullName = user.FullName,
                PhoneNumber = user.PhoneNumber,
                UserName = user.UserName,
                RoleId = user.RoleId,
                RoleName = user.Role.RoleName,
                CampusId = user.CampusId,
                CampusName = user.Campus.Name,
                Status = user.Status.ToString(),
                IsVerify = user.IsVerify.ToString(),
                AvatarUrl = user.AvatarUrl,
                LastLogin = user.LastLogin,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt
            };
        }

        public async Task<UserResponseDto?> UpdateProfileAsync(string userId, UpdateUserProfileDto dto)
        {
            var user = await _unitOfWork.UserRepo.GetByIdAsync(userId);

            if (user == null)
            {
                return null;
            }

            user.PhoneNumber = dto.PhoneNumber;
            user.AvatarUrl = dto.AvatarUrl;
            user.UpdatedAt = DateTimeHelper.VietnamNow;

            await _unitOfWork.UserRepo.UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync();

            return new UserResponseDto
            {
                UserId = user.UserId,
                Email = user.Email,
                FullName = user.FullName,
                PhoneNumber = user.PhoneNumber,
                UserName = user.UserName,
                RoleId = user.RoleId,
                RoleName = user.Role.RoleName,
                CampusId = user.CampusId,
                CampusName = user.Campus.Name,
                Status = user.Status.ToString(),
                IsVerify = user.IsVerify.ToString(),
                AvatarUrl = user.AvatarUrl,
                LastLogin = user.LastLogin,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt
            };
        }

        public async Task<bool> ChangePasswordAsync(string userId, ChangePasswordDto dto)
        {
            var user = await _unitOfWork.UserRepo.GetByIdAsync(userId);

            if (user == null)
            {
                throw new ArgumentException("User not found.");
            }

            // check nếu user login với google 
            const string DefaultGooglePasswordHash = "Li9EQojtoWXDlQqOM55n0hFRn76PmCiheTGGM7zblAv5cFMyICasjdabsjdnouiwh1uidasoiu";
            if (user.Password == DefaultGooglePasswordHash || string.IsNullOrEmpty(user.Password))
            {
                throw new InvalidOperationException("This account uses Google login. Password cannot be changed.");
            }

            // Verify old password
            bool isOldPasswordValid = false;
            try
            {
                isOldPasswordValid = BCrypt.Net.BCrypt.Verify(dto.OldPassword, user.Password);
            }
            catch
            {
                isOldPasswordValid = false;
            }

            if (!isOldPasswordValid)
            {
                throw new UnauthorizedAccessException("Current password is incorrect.");
            }

            // check nếu password mới trùng với password cũ
            if (dto.OldPassword == dto.NewPassword)
            {
                throw new ArgumentException("New password must be different from current password.");
            }

            // update password mới 
            user.Password = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
            user.UpdatedAt = DateTimeHelper.VietnamNow;

            await _unitOfWork.UserRepo.UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
        public async Task<ApiResponseWithPagination<List<UserResponseDto>>> GetAllAsync(UserFilterDto filter)
        {
            var (items, total) = await _unitOfWork.UserRepo.GetFilteredAsync(
                filter.Name,
                filter.Email,
                filter.RoleId,
                filter.CampusId,
                filter.Status?.ToString(),
                filter.Page,
                filter.Limit
            );

            var responseDtos = items.Select(u => new UserResponseDto
            {
                UserId = u.UserId,
                Email = u.Email,
                FullName = u.FullName,
                PhoneNumber = u.PhoneNumber,
                UserName = u.UserName,
                RoleId = u.RoleId,
                RoleName = u.Role?.RoleName ?? string.Empty,
                CampusId = u.CampusId,
                CampusName = u.Campus?.Name ?? string.Empty,
                Status = u.Status.ToString(),
                IsVerify = u.IsVerify.ToString(),
                AvatarUrl = u.AvatarUrl,
                LastLogin = u.LastLogin,
                CreatedAt = u.CreatedAt,
                UpdatedAt = u.UpdatedAt
            }).ToList();

            return ApiResponseWithPagination<List<UserResponseDto>>.Ok(
                responseDtos,
                filter.Page,
                filter.Limit,
                total
            );
        }

        public async Task<ApiResponse<UserResponseDto>> GetByIdAsync(string id)
        {
            var user = await _unitOfWork.UserRepo.GetByIdAsync(id);
            if (user == null)
            {
                return ApiResponse<UserResponseDto>.Fail(404, "User not found");
            }

            var responseDto = new UserResponseDto
            {
                UserId = user.UserId,
                Email = user.Email,
                FullName = user.FullName,
                PhoneNumber = user.PhoneNumber,
                UserName = user.UserName,
                RoleId = user.RoleId,
                CampusId = user.CampusId,
                Status = user.Status.ToString(),
                IsVerify = user.IsVerify.ToString(),
                AvatarUrl = user.AvatarUrl,
                LastLogin = user.LastLogin,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt
            };

            return ApiResponse<UserResponseDto>.Ok(responseDto);
        }

        public async Task<ApiResponse> DeleteAsync(string userId)
        {
            var user = await _unitOfWork.UserRepo.GetByIdAsync(userId);
            if (user == null)
            {
                return ApiResponse.Fail(404, "Không tìm thấy user.");
            }

            // Soft delete: set status to Inactive (user cannot login anymore)
            user.Status = UserStatus.Inactive;
            user.UpdatedAt = DateTimeHelper.VietnamNow;

            await _unitOfWork.UserRepo.UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync();

            return ApiResponse.Ok();
        }
    }
}



