using Applications.DTOs.Request;
using Applications.DTOs.Response;
using BLL.Interfaces;
using DAL.Models;
using DAL.Models.Enums;
using DAL.Repositories;
using Google.Apis.Auth;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using static Google.Apis.Auth.GoogleJsonWebSignature;

namespace BLL.Classes
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;
        private const string DefaultGooglePasswordHash = "Li9EQojtoWXDlQqOM55n0hFRn76PmCiheTGGM7zblAv5cFMyICasjdabsjdnouiwh1uidasoiu";

        public AuthService(IUnitOfWork unitOfWork, IConfiguration configuration, IEmailService emailService)
        {
            _unitOfWork = unitOfWork;
            _configuration = configuration;
            _emailService = emailService;
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
        {
            var user = await _unitOfWork.UserRepo.GetByEmailOrUsernameAsync(dto.EmailOrUsername);

            if (user == null)
            {
                throw new UnauthorizedAccessException("Invalid email/username or password.");
            }

            // Check if user is trying to login with Google account
            if (user.Password == DefaultGooglePasswordHash)
            {
                throw new UnauthorizedAccessException("This account uses Google login. Please sign in with Google.");
            }

            // Check if user has password (email/username login)
            if (string.IsNullOrEmpty(user.Password))
            {
                throw new UnauthorizedAccessException("Invalid email/username or password.");
            }

            // Ensure user is using the correct login method
            // If user has email (not null), they should login with email only
            // If user has username (not null), they should login with username only
            bool isValidLoginMethod = false;
            if (!string.IsNullOrEmpty(user.Email) && user.Email == dto.EmailOrUsername)
            {
                isValidLoginMethod = true;
            }
            else if (!string.IsNullOrEmpty(user.UserName) && user.UserName == dto.EmailOrUsername)
            {
                isValidLoginMethod = true;
            }

            if (!isValidLoginMethod)
            {
                throw new UnauthorizedAccessException("Invalid email/username or password.");
            }

            // Verify password
            bool isPasswordValid = false;
            try
            {
                isPasswordValid = BCrypt.Net.BCrypt.Verify(dto.Password, user.Password);
            }
            catch
            {
                isPasswordValid = false;
            }

            if (!isPasswordValid)
            {
                throw new UnauthorizedAccessException("Invalid email/username or password.");
            }

            if (user.Status != UserStatus.Active)
            {
                throw new UnauthorizedAccessException("Your account is inactive. Please contact support.");
            }

            // Update last login
            user.LastLogin = DateTime.UtcNow;
            await _unitOfWork.UserRepo.UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync();

            var token = GenerateJwtToken(user);
            return new AuthResponseDto
            {
                Token = token,
                UserId = user.UserId,
                Email = user.Email ?? string.Empty,
                FullName = user.FullName,
                RoleId = user.RoleId,
                IsVerified = user.IsVerify == VerificationStatus.Verified
            };
        }

        public async Task<AuthResponseDto> LoginWithGoogleAsync(GoogleLoginDto dto)
        {
            // Validate Google ID token
            var payload = await GoogleJsonWebSignature.ValidateAsync(
                dto.IdToken,
                new ValidationSettings
                {
                    Audience = new[] { _configuration["Google:ClientId"] },
                    IssuedAtClockTolerance = TimeSpan.FromMinutes(5)
                });

            var email = payload.Email;

            // Validate email domain
            if (!IsValidFptEmail(email))
            {
                throw new UnauthorizedAccessException("Only @fpt.edu.vn or @fe.edu.vn email addresses are allowed.");
            }

            var user = await _unitOfWork.UserRepo.GetByEmailAsync(email);

            // If user doesn't exist, create new user
            if (user == null)
            {
                user = await CreateGoogleUserAsync(payload);
            }
            else
            {
                // Check if user is trying to login with Google but account was created with email/username
                if (user.Password != DefaultGooglePasswordHash && !string.IsNullOrEmpty(user.Password))
                {
                    throw new UnauthorizedAccessException("This account uses email/username login. Please sign in with your credentials.");
                }
            }

            if (user.Status != UserStatus.Active)
            {
                throw new UnauthorizedAccessException("Your account is inactive. Please contact support.");
            }

            // Update last login
            user.LastLogin = DateTime.UtcNow;
            await _unitOfWork.UserRepo.UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync();

            var token = GenerateJwtToken(user);
            return new AuthResponseDto
            {
                Token = token,
                UserId = user.UserId,
                Email = user.Email ?? string.Empty,
                FullName = user.FullName,
                RoleId = user.RoleId,
                IsVerified = user.IsVerify == VerificationStatus.Verified
            };
        }

        public async Task<bool> VerifyEmailAsync(string email, string token)
        {
            var user = await _unitOfWork.UserRepo.GetByEmailAsync(email);

            if (user == null ||
                string.IsNullOrEmpty(user.EmailVerificationToken) ||
                user.EmailVerificationToken != token ||
                user.EmailVerificationTokenExpiry < DateTime.UtcNow)
            {
                return false;
            }

            user.IsVerify = VerificationStatus.Verified;
            user.EmailVerificationToken = null;
            user.EmailVerificationTokenExpiry = null;
            user.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.UserRepo.UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<bool> ResendVerificationEmailAsync(string email)
        {
            var user = await _unitOfWork.UserRepo.GetByEmailAsync(email);

            if (user == null)
            {
                throw new ArgumentException("User not found with this email.");
            }

            // Check if user is already verified
            if (user.IsVerify == VerificationStatus.Verified)
            {
                throw new InvalidOperationException("Email is already verified.");
            }

            // Generate new verification token
            user.EmailVerificationToken = Guid.NewGuid().ToString();
            user.EmailVerificationTokenExpiry = DateTime.UtcNow.AddDays(1);
            user.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.UserRepo.UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync();

            // Send verification email
            var callbackUrl = _configuration["AppSettings:VerificationCallbackUrl"] ?? "http://localhost:3000/verify-email";
            await _emailService.SendVerificationEmailAsync(user.Email!, user.EmailVerificationToken, callbackUrl);

            return true;
        }

        private async Task<User> CreateGoogleUserAsync(Payload payload)
        {
            // Generate user ID (format: U0001)
            var userId = await GenerateUserIdAsync();

            // Determine role based on email domain
            var roleId = DetermineRoleByEmailDomain(payload.Email);

            var user = new User
            {
                UserId = userId,
                Email = payload.Email,
                FullName = payload.Name,
                Password = DefaultGooglePasswordHash,
                RoleId = roleId,
                CampusId = "C0001", // Default campus
                Status = UserStatus.Active,
                IsVerify = VerificationStatus.Unverified,
                AvatarUrl = null, // Leave avatar blank
                EmailVerificationToken = Guid.NewGuid().ToString(),
                EmailVerificationTokenExpiry = DateTime.UtcNow.AddDays(1),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _unitOfWork.UserRepo.AddAsync(user);
            await _unitOfWork.SaveChangesAsync();

            // Send verification email
            var callbackUrl = _configuration["AppSettings:VerificationCallbackUrl"] ?? "http://localhost:3000/verify-email";
            await _emailService.SendVerificationEmailAsync(user.Email!, user.EmailVerificationToken, callbackUrl);

            return user;
        }

        private string DetermineRoleByEmailDomain(string email)
        {
            // @fpt.edu.vn -> Student (RL0001)
            // @fe.edu.vn -> Lecturer (RL0002)
            if (email.EndsWith("@fpt.edu.vn", StringComparison.OrdinalIgnoreCase))
            {
                return "RL0001"; // Student
            }
            else if (email.EndsWith("@fe.edu.vn", StringComparison.OrdinalIgnoreCase))
            {
                return "RL0002"; // Lecturer
            }

            return "RL0001"; // Default to Student
        }

        private bool IsValidFptEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
                return false;

            return email.EndsWith("@fpt.edu.vn", StringComparison.OrdinalIgnoreCase) ||
                   email.EndsWith("@fe.edu.vn", StringComparison.OrdinalIgnoreCase);
        }

        private string GenerateJwtToken(User user)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserId),
                new Claim(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
                new Claim(ClaimTypes.Role, user.RoleId),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Name, user.FullName),
                new Claim("IsVerified", (user.IsVerify == VerificationStatus.Verified).ToString()),
                new Claim("AvatarUrl", user.AvatarUrl ?? string.Empty)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? string.Empty));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddDays(1),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private async Task<string> GenerateUserIdAsync()
        {
            var users = await _unitOfWork.UserRepo.GetAllAsync();
            var maxId = 0;
            
            foreach (var user in users)
            {
                if (user.UserId.StartsWith("U") && user.UserId.Length == 6)
                {
                    if (int.TryParse(user.UserId.Substring(1), out var id))
                    {
                        if (id > maxId)
                            maxId = id;
                    }
                }
            }

            return $"U{(maxId + 1):D5}";
        }
    }
}
