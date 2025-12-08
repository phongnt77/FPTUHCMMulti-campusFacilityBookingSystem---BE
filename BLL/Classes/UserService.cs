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
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UserService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<UserResponseDto?> GetProfileAsync(string userId)
        {
            var user = await _unitOfWork.UserRepo.GetByIdAsync(userId);
            if (user == null)
            {
                return null;
            }

            return _mapper.Map<UserResponseDto>(user);
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

            return _mapper.Map<UserResponseDto>(user);
        }

        public async Task<bool> ChangePasswordAsync(string userId, ChangePasswordDto dto)
        {
            var user = await _unitOfWork.UserRepo.GetByIdAsync(userId);

            if (user == null)
            {
                throw new ArgumentException("Không tìm thấy người dùng.");
            }

            const string DefaultGooglePasswordHash = "Li9EQojtoWXDlQqOM55n0hFRn76PmCiheTGGM7zblAv5cFMyICasjdabsjdnouiwh1uidasoiu";
            if (user.Password == DefaultGooglePasswordHash || string.IsNullOrEmpty(user.Password))
            {
                throw new InvalidOperationException("Tài khoản này sử dụng đăng nhập Google. Không thể thay đổi mật khẩu.");
            }

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
                throw new UnauthorizedAccessException("Mật khẩu hiện tại không đúng.");
            }

            if (dto.OldPassword == dto.NewPassword)
            {
                throw new ArgumentException("Mật khẩu mới phải khác mật khẩu hiện tại.");
            }

            // Validate password strength
            var validationResult = ValidatePasswordStrength(dto.NewPassword);
            if (!validationResult.IsValid)
            {
                throw new ArgumentException(validationResult.ErrorMessage);
            }

            user.Password = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
            user.UpdatedAt = DateTimeHelper.VietnamNow;

            await _unitOfWork.UserRepo.UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        private (bool IsValid, string ErrorMessage) ValidatePasswordStrength(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                return (false, "Mật khẩu không được để trống.");
            }

            // Check minimum length
            if (password.Length < 8)
            {
                return (false, "Mật khẩu phải có ít nhất 8 ký tự.");
            }

            // Check for uppercase letter
            if (!password.Any(char.IsUpper))
            {
                return (false, "Mật khẩu phải có ít nhất một chữ cái viết hoa.");
            }

            // Check for lowercase letter
            if (!password.Any(char.IsLower))
            {
                return (false, "Mật khẩu phải có ít nhất một chữ cái thường.");
            }

            // Check for digit
            if (!password.Any(char.IsDigit))
            {
                return (false, "Mật khẩu phải có ít nhất một chữ số.");
            }

            // Check for special character
            var specialChars = "!@#$%^&*()_+-=[]{}|;:,.<>?";
            if (!password.Any(c => specialChars.Contains(c)))
            {
                return (false, "Mật khẩu phải có ít nhất một ký tự đặc biệt (!@#$%^&*()_+-=[]{}|;:,.<>?).");
            }

            return (true, string.Empty);
        }

        public async Task<ApiResponseWithPagination<List<UserResponseDto>>> GetAllAsync(UserFilterDto filter)
        {
            var (items, total) = await _unitOfWork.UserRepo.GetFilteredAsync(
                filter.Name,
                filter.Email,
                filter.RoleId,
                filter.Status?.ToString(),
                filter.Page,
                filter.Limit
            );

            var responseDtos = _mapper.Map<List<UserResponseDto>>(items);

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
                return ApiResponse<UserResponseDto>.Fail(404, "Không tìm thấy người dùng.");
            }

            var responseDto = _mapper.Map<UserResponseDto>(user);
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
