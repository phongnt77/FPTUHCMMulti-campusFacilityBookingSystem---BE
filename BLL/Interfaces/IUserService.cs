using Applications.DTOs.Request;
using Applications.DTOs.Response;

namespace BLL.Interfaces
{
    public interface IUserService
    {
        Task<UserResponseDto?> GetProfileAsync(string userId);
        Task<UserResponseDto?> UpdateProfileAsync(string userId, UpdateUserProfileDto dto);
        Task<bool> ChangePasswordAsync(string userId, ChangePasswordDto dto);
    }
}



