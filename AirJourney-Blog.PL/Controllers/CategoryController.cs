using AirJourney_Blog.BLL.Ordering;
using AirJourney_Blog.Service.Services.BlogCategory;
using AirJourney_Blog.Service.Services.Category.Dtos;
using AirJourney_Blog.Service.Services.Resources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System.Security.Cryptography;

namespace AirJourney_Blog.PL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly IBlogCategoryService categoryService;
        private readonly IStringLocalizer<SharedResources> stringLocalizer;

        public CategoryController(IBlogCategoryService _categoryService,
            IStringLocalizer<SharedResources> stringLocalizer)
        {
            categoryService = _categoryService;
            this.stringLocalizer = stringLocalizer;
        }

        [HttpPost]
        [EndpointSummary("Creates a new blog category in the database.")]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> AddCategory (CategoryDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var createdCategory = await categoryService.CreateCategoryAsync(model);
                return CreatedAtAction(
                    nameof(GetCategoryById),
                    new { id = createdCategory.Id },
                    createdCategory);
            }
            catch(ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while creating the category", error = ex.Message });
            }
        }

        
        
        [HttpPut("{id}")]
        [EndpointSummary("Updates an existing blog category.")]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] CategoryDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var updatedCategory = await categoryService.UpdateCategoryAsync(id, model);

                return Ok(updatedCategory); 
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "An error occurred while updating the category",
                    error = ex.Message
                });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategoryById(int id)
        {
            try
            {
                var category = await categoryService.GetCategoryByIDAsync(id);
                return Ok(category);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "An internal server error occurred",
                    error = ex.Message
                });
            }
        }

        [HttpGet("admin/{id}")]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> GetAdminCategoryById(int id)
        {
            try
            {
                var category = await categoryService.GetAdminCategoryByIDAsync(id);
                return Ok(category);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "An internal server error occurred",
                    error = ex.Message
                });
            }
        }


        [HttpPut("ToggleActivation/{id}")]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> ToggleCategoryActivation(int id)
        {
            try
            {
                var updatedCategory = await categoryService.ToggleCategoryActivationAsync(id);
                return Ok(new
                {
                    id = id,
                    isActive = updatedCategory.IsVisible,
                    message = $"Category activation status updated to {(updatedCategory.IsVisible ? "Active" : "Inactive")}."
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
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while toggling the category activation", error = ex.Message });
            }
        }

        [HttpGet("admin")]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> GetAllAdminCategories(
            [FromQuery] int? take = null,
            [FromQuery] int? skip = null,
            [FromQuery] string sortBy = "CreatedAt",
            [FromQuery] string sortDirection = OrderBy.Descending,
            [FromQuery] bool? isVisible = null, 
            [FromQuery] string searchTerm = null,  
            [FromQuery] DateTime? fromDate = null,  
            [FromQuery] DateTime? toDate = null)
        {
            try
            {

                if (take <= 0 || skip < 0)
                {
                    return BadRequest(new { message = "Invalid pagination parameters" });
                }

                var result = await categoryService.GetAllAdminCategoriesAsync(
                    take,
                    skip,
                    sortBy,
                    sortDirection,
                    isVisible,
                    searchTerm,
                    fromDate,
                    toDate
                );

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "An internal server error occurred",
                    error = ex.Message
                });
            }
        }


        [HttpGet("dropdown")]
        public async Task<IActionResult> GetCategoryDropdownAsync()
        {
            try
            {
                var result = await categoryService.GetCategoryDropdownAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Message = "An internal server error occurred",
                    Detail = ex.Message
                });
            }
        }

        [HttpGet("admin/dropdown")]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> GetAdminCategoryDropdownAsync()
        {
            try
            {
                var result = await categoryService.GetAdminCategoryDropdownAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Message = "An internal server error occurred",
                    Detail = ex.Message
                });
            }
        }


    }
}
