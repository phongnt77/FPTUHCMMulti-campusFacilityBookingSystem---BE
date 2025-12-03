using Applications.DTOs.Request;
using Applications.DTOs.Response;
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
            user.UpdatedAt = DateTime.UtcNow;

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
            user.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.UserRepo.UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}



