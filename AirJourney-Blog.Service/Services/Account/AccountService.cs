using AirJourney_Blog.BLL.Helper;
using AirJourney_Blog.BLL.Interfaces;
using AirJourney_Blog.BLL.Ordering;
using AirJourney_Blog.DAL.Models;
using AirJourney_Blog.Service.Services.Account.Dtos;
using AirJourney_Blog.Service.Services.EmailService;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using AutoMapper;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace AirJourney_Blog.Service.Services.Account
{
    public class AccountService : IAccountService
    {
        private readonly IMapper mapper;
        private readonly UserManager<AppUser> userManager;
        private readonly IConfiguration config;
        private readonly IUnitOfWork unitOfWork;
        private readonly IEmailSender emailSender;

        public AccountService(
            IMapper _mapper,
            UserManager<AppUser> _userManager,
            IConfiguration _config,
            IUnitOfWork _unitOfWork,
            IEmailSender _emailSender)
        {
            mapper = _mapper;
            userManager = _userManager;
            config = _config;
            unitOfWork = _unitOfWork;
            emailSender = _emailSender;
        }

        public async Task<AppUser> RegisterCustomerAsync(RegisterDto model)
        {
            return await RegisterUserByRoleAsync(model, "Customer");
        }

        public async Task<AppUser> RegisterAdminAsync(RegisterDto model)
        {
            return await RegisterUserByRoleAsync(model, "Admin");
        }

        private async Task<AppUser> RegisterUserByRoleAsync(RegisterDto model, string role)
        {
            var existingUser = await userManager.FindByEmailAsync(model.Email);
            if (existingUser != null)
                throw new ArgumentException("User with this email already exists");

            var phoneExists = await unitOfWork.GenericRepository<AppUser>()
                .AnyAsync(user => user.PhoneNumber == model.PhoneNumber);
            if (phoneExists)
                throw new ArgumentException("User with this phone number already exists");

            AppUser userData = mapper.Map<AppUser>(model);
            userData.UserName = model.Email.Split('@')[0];

            var createResult = await userManager.CreateAsync(userData, model.Password);
            if (!createResult.Succeeded)
            {
                var errors = string.Join(" ", createResult.Errors.Select(e => e.Description));
                throw new ArgumentException($"User creation failed: {errors}");
            }

            var roleResult = await userManager.AddToRoleAsync(userData, role);
            if (!roleResult.Succeeded)
            {
                var errors = string.Join(" ", roleResult.Errors.Select(e => e.Description));
                throw new ArgumentException($"Role assignment failed: {errors}");
            }

            return userData;
        }

        public async Task<AppUser?> LoginUserAsync(LoginDto model)
        {

            var user = await userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return null;

            bool isPasswordValid = await userManager.CheckPasswordAsync(user, model.Password);
            if (!isPasswordValid)
                return null;

            return user;
        }

        public async Task<List<string>> GetUserRoleByIdAsync(int id)
        {
            var user = await userManager.FindByIdAsync(id.ToString());
            return user == null ? new List<string>() : (await userManager.GetRolesAsync(user)).ToList();
        }

        public async Task<bool> ToggleUserActivationAsync(int userId)
        {
            var user = await userManager.FindByIdAsync(userId.ToString());
            if (user == null)
                throw new KeyNotFoundException("User not found");

            var roles = await userManager.GetRolesAsync(user);
            if (roles.Contains("SuperAdmin"))
                throw new InvalidOperationException("Super Admin activation status cannot be changed.");



            user.IsActive = !user.IsActive;
            var result = await userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"User status update failed: {errors}");
            }

            return user.IsActive;
        }

        public async Task<bool> IsUserActiveAsync(int userId)
        {
            var user = await userManager.FindByIdAsync(userId.ToString());
            if (user == null)
                throw new KeyNotFoundException("User not found");
            return user.IsActive;
        }

        public async Task<string> GenerateTokenAsync(AppUser user)
        {
            var userClaims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email)
            };

            var userRoles = await GetUserRoleByIdAsync(user.Id);
            userRoles.ForEach(role => userClaims.Add(new Claim(ClaimTypes.Role, role)));

            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["JWT:SecurityKey"]));
            var token = new JwtSecurityToken(
                expires: DateTime.Now.AddDays(1),
                claims: userClaims,
                signingCredentials: new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<string> RequestToChangePassword(PasswordResetRequestDto model)
        {
            var user = await userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return "If your email exists in our system, you'll receive a password reset link";

            var token = await userManager.GeneratePasswordResetTokenAsync(user);
            var resetLink = $"{config["ClientApp:BaseUrl"]}/reset-password?email={Uri.EscapeDataString(model.Email)}&token={Uri.EscapeDataString(token)}";

            await emailSender.SendEmailAsync(
                model.Email,
                "Password Reset Request",
                $"Hello {user.UserName},<br><br>Please reset your password by <a href='{resetLink}'>clicking here</a>.");

            return "If your email exists in our system, you'll receive a password reset link";
        }

        public async Task<string> ResetPasswordAsync(ResetPasswordDto model)
        {
            var user = await userManager.FindByEmailAsync(model.Email);
            if (user == null)
                throw new KeyNotFoundException("User not found");

            var result = await userManager.ResetPasswordAsync(user, model.Token, model.NewPassword);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new ArgumentException($"Password reset failed: {errors}");
            }

            return "Password has been reset successfully";
        }

        public async Task<PaginatedResult<AccountDto>> GetAllAccountsAsync(
           int? take = null,
           int? skip = null,
           string sortBy = "UserName",
           string sortDirection = OrderBy.Ascending,
           string searchTerm = null,
           bool? isActive = true)
        {
            Expression<Func<AppUser, object>> orderBy = sortBy switch
            {
                "FirstName" => x => x.FirstName,
                "LastName" => x => x.LastName,
                "Email" => x => x.Email,
                "UserName" => x => x.UserName,
                _ => x => x.UserName
            };

            Expression<Func<AppUser, bool>> filter = user =>
                user.Email != "superadmin@example.com" &&
                (isActive == null || user.IsActive == isActive) &&
                (string.IsNullOrEmpty(searchTerm) ||
                    user.FirstName.Contains(searchTerm) ||
                    user.LastName.Contains(searchTerm) ||
                    user.Email.Contains(searchTerm) ||
                    user.UserName.Contains(searchTerm));




            var users = await unitOfWork
                .GenericRepository<AppUser>()
                .GetAllAsync(
                    take: take,
                    skip: skip,
                    orderBy: orderBy,
                    orderByDirection: sortDirection,
                    criteria: filter
                );

            var dtos = mapper.Map<List<AccountDto>>(users.Items);

            foreach (var dto in dtos)
            {
                var user = await userManager.FindByIdAsync(dto.Id.ToString());
                var roles = await userManager.GetRolesAsync(user);
                dto.Role = roles.FirstOrDefault();
            }

            return new PaginatedResult<AccountDto>
            {
                Items = dtos,
                CurrentPage = users.CurrentPage,
                PageSize = users.PageSize,
                TotalPages = users.TotalPages
            };
        }

        public async Task<AccountDto> GetUserByIdAsync(int id)
        {
            var user = await unitOfWork.GenericRepository<AppUser>().GetByIdAsync(id);

            if (user == null)
                throw new KeyNotFoundException("User not found");

            var dto = mapper.Map<AccountDto>(user);

            var identityUser = await userManager.FindByIdAsync(user.Id.ToString());
            var roles = await userManager.GetRolesAsync(identityUser);
            dto.Role = roles.FirstOrDefault();

            return dto;
        }

        public async Task<AccountDto> EditUserAsync(string email, UpdateAccountDto model)
        {


            var user = await userManager.FindByEmailAsync(email);


            if (!string.IsNullOrEmpty(model.PhoneNumber))
            {
                var existingUserWithSamePhone = await userManager.Users
                    .FirstOrDefaultAsync(u => u.PhoneNumber == model.PhoneNumber && u.Id != user.Id);

                if (existingUserWithSamePhone != null)
                {
                    throw new InvalidOperationException("Phone number is already in use by another account.");
                }
            }


            // Map properties manually
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.ProfilePicture = model.ProfilePicture;
            user.PictureId = model.PictureId;
            user.DateOfBirth = model.DateOfBirth;
            user.UserName = model.UserName;
            user.PhoneNumber = model.PhoneNumber;

            var result = await userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"User update failed: {errors}");
            }

            var updatedDto = mapper.Map<AccountDto>(user);

            return updatedDto;
        }




    }
}