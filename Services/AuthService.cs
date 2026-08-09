using Microsoft.AspNetCore.Identity;
using SmartRecruitment_Project.DTOs.Auth;
using SmartRecruitment_Project.Exceptions;
using SmartRecruitment_Project.Interfaces.Repositories;
using SmartRecruitment_Project.Interfaces.Services;
using SmartRecruitment_Project.Models;
using SmartRecruitment_Project.Models.Enums;

namespace SmartRecruitment_Project.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _authRepository;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly PasswordHasher<User> _passwordHasher;

        public AuthService(
            IAuthRepository authRepository,
            IJwtTokenService jwtTokenService)
        {
            _authRepository = authRepository;
            _jwtTokenService = jwtTokenService;
            _passwordHasher = new PasswordHasher<User>();
        }

        public async Task<AuthResponseDto> RegisterJobSeekerAsync(
            JobSeekerRegisterDto dto)
        {
            var email = dto.Email.Trim().ToLower();

            var emailExists =
                await _authRepository.EmailExistsAsync(email);

            if (emailExists)
            {
                throw new ConflictException(
                    "An account already exists with this email.");
            }

            var user = new User
            {
                Email = email,
                Role = UserRole.JobSeeker,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            user.PasswordHash =
                _passwordHasher.HashPassword(
                    user,
                    dto.Password);

            var createdUser =
                await _authRepository.CreateUserAsync(user);

            var token =
                _jwtTokenService.GenerateToken(createdUser);

            return new AuthResponseDto
            {
                UserId = createdUser.Id,
                Email = createdUser.Email,
                Role = createdUser.Role.ToString(),
                Token = token,
                ExpiresAt = _jwtTokenService.GetTokenExpiry()
            };
        }

        public async Task<AuthResponseDto> RegisterEmployerAsync(
            EmployerRegisterDto dto)
        {
            var email = dto.Email.Trim().ToLower();

            var emailExists =
                await _authRepository.EmailExistsAsync(email);

            if (emailExists)
            {
                throw new ConflictException(
                    "An account already exists with this email.");
            }

            var user = new User
            {
                Email = email,
                Role = UserRole.Employer,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            user.PasswordHash =
                _passwordHasher.HashPassword(
                    user,
                    dto.Password);

            var createdUser =
                await _authRepository.CreateUserAsync(user);

            var token =
                _jwtTokenService.GenerateToken(createdUser);

            return new AuthResponseDto
            {
                UserId = createdUser.Id,
                Email = createdUser.Email,
                Role = createdUser.Role.ToString(),
                Token = token,
                ExpiresAt = _jwtTokenService.GetTokenExpiry()
            };
        }

        public async Task<AuthResponseDto> LoginAsync(
            LoginDto dto)
        {
            var email = dto.Email.Trim().ToLower();

            var user =
                await _authRepository.GetByEmailAsync(email);

            if (user == null)
            {
                throw new UnauthorizedException(
                    "Invalid email or password.");
            }

            if (!user.IsActive)
            {
                throw new UnauthorizedException(
                    "This account is inactive.");
            }

            var result =
                _passwordHasher.VerifyHashedPassword(
                    user,
                    user.PasswordHash,
                    dto.Password);

            if (result == PasswordVerificationResult.Failed)
            {
                throw new UnauthorizedException(
                    "Invalid email or password.");
            }

            var token =
                _jwtTokenService.GenerateToken(user);

            return new AuthResponseDto
            {
                UserId = user.Id,
                Email = user.Email,
                Role = user.Role.ToString(),
                Token = token,
                ExpiresAt = _jwtTokenService.GetTokenExpiry()
            };
        }
    }
}