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
    }
}



