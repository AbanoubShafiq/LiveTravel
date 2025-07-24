using AirJourney_Blog.BLL.Helper;
using AirJourney_Blog.DAL.Models;
using AirJourney_Blog.Service.Services.Account;
using AirJourney_Blog.Service.Services.Account.Dtos;
using AirJourney_Blog.Service.Services.Image;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace AirJourney_Blog.PL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService accountService;
        private readonly IImageService imageService;
        private readonly UserManager<AppUser> userManager;

        public AccountController(
            IAccountService _accountService,
            IImageService imageService,
            UserManager<AppUser> userManager
            )
        {
            accountService = _accountService;
            this.imageService = imageService;
            this.userManager = userManager;
        }


        [HttpPost("RegisterAdmin")]
        [Authorize(Roles = "SuperAdmin")]

        public async Task<IActionResult> RegisterAdmin(RegisterDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { message = "Invalid model state" });

            try
            {
                var user = await accountService.RegisterAdminAsync(model);
                var token = await accountService.GenerateTokenAsync(user);

                return Ok(new
                {
                    token,
                    expiration = DateTime.Now.AddDays(1)
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Registration error occurred",
                    error = ex.Message
                });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { message = "Invalid model state" });

            try
            {
                var user = await accountService.LoginUserAsync(model);

                if (user == null)
                    return Unauthorized(new { message = "Invalid credentials" });

                if (!user.IsActive)
                    return Unauthorized(new { message = "Account is not active" });

                var token = await accountService.GenerateTokenAsync(user);

                return Ok(new
                {
                    token,
                    expiration = DateTime.Now.AddDays(1)
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Internal server error",
                    details = ex.Message
                });
            }
        }

        [HttpPost("toggle/{userId:int}")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> ToggleActivation(int userId)
        {
            if (userId <= 0)
                return BadRequest(new { message = "Invalid user ID" });

            try
            {
                bool isActive = await accountService.ToggleUserActivationAsync(userId);
                string message = isActive ?
                    "User activated successfully" :
                    "User deactivated successfully";

                return Ok(new
                {
                    Success = true,
                    IsActive = isActive,
                    Message = message
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Success = false,
                    Message = "Internal server error",
                    Detail = ex.Message
                });
            }
        }

        [HttpGet("status/{userId}")]
        public async Task<IActionResult> GetActivationStatus(int userId)
        {
            try
            {
                var isActive = await accountService.IsUserActiveAsync(userId);
                return Ok(new { IsActive = isActive });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Internal server error",
                    details = ex.Message
                });
            }
        }

        [HttpPost("request-password-reset")]
        public async Task<IActionResult> RequestPasswordReset(PasswordResetRequestDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { message = "Invalid model state" });

            try
            {
                var message = await accountService.RequestToChangePassword(model);
                return Ok(new { message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Internal server error",
                    details = ex.InnerException?.Message
                });
            }
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { message = "Invalid model state" });

            try
            {
                var message = await accountService.ResetPasswordAsync(model);
                return Ok(new
                {
                    message = "Password reset successfully"
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new
                {
                    message = "An unexpected error occurred"
                });
            }
        }

        [HttpGet]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<ActionResult<PaginatedResult<AccountDto>>> GetAllAccountsAsync(
            int? take = null,
            int? skip = null,
            string sortBy = "UserName",
            string sortDirection = "Ascending",
            string searchTerm = null,
            bool? isActive = true)
        {
            try
            {
                if (take <= 0 || skip < 0)
                {
                    return BadRequest(new
                    {
                        message = "Invalid pagination parameters"
                    });
                }

                var result = await accountService.GetAllAccountsAsync(
                    take, skip, sortBy, sortDirection, searchTerm, isActive);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Internal server error",
                    detail = ex.Message
                });
            }
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            try
            {
                var user = await accountService.GetUserByIdAsync(id);
                return Ok(user);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = "User not found" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Message = "Internal server error",
                    Detail = ex.Message
                });
            }
        }

        [Authorize]
        [HttpPut("{email}")]
        public async Task<IActionResult> EditProfile(string email, [FromBody] UpdateAccountDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { message = "Invalid model state" });

            try
            {
                var user = await userManager.FindByEmailAsync(email);
                if (user == null)
                    return NotFound(new { message = "No user found with that email." });

                if (!string.IsNullOrEmpty(model.ProfilePicture) && user.PictureId != null)
                {
                    await imageService.DeleteImageAsync(user.PictureId);
                }

                var updatedUser = await accountService.EditUserAsync(email, model);

                return Ok(new
                {
                    message = "Profile updated successfully",
                    user = updatedUser
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "An unexpected error occurred" });
            }
        }




    }
}