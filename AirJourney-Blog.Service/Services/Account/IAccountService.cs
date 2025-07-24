using AirJourney_Blog.BLL.Helper;
using AirJourney_Blog.BLL.Ordering;
using AirJourney_Blog.DAL.Models;
using AirJourney_Blog.Service.Services.Account.Dtos;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirJourney_Blog.Service.Services.Account
{
    public interface IAccountService
    {
        public Task<AppUser> RegisterCustomerAsync(RegisterDto model);
        public Task<AppUser> RegisterAdminAsync(RegisterDto model);
        public Task<AppUser?> LoginUserAsync(LoginDto model);
        public Task<List<string>> GetUserRoleByIdAsync(int id);
        public Task<bool> ToggleUserActivationAsync(int userId);
        Task<bool> IsUserActiveAsync(int userId);
        public Task<string> GenerateTokenAsync(AppUser user);
        public Task<string> RequestToChangePassword(PasswordResetRequestDto model);
        public Task<string> ResetPasswordAsync(ResetPasswordDto model);

        public Task<PaginatedResult<AccountDto>> GetAllAccountsAsync(
           int? take = null,
           int? skip = null,
           string sortBy = "UserName",
           string sortDirection = OrderBy.Ascending,
           string searchTerm = null,
           bool? isActive = true);

        public Task<AccountDto> GetUserByIdAsync(int id);

        public Task<AccountDto> EditUserAsync(string email, UpdateAccountDto model);




    }
}
