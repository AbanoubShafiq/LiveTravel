using AirJourney_Blog.BLL.Helper;
using AirJourney_Blog.PL.Helper;
using AirJourney_Blog.Service.Services;
using AirJourney_Blog.Service.Services.Blog.Dtos;
using AirJourney_Blog.Service.Services.BlogCategory;
using AirJourney_Blog.Service.Services.BlogPost;
using AirJourney_Blog.Service.Services.BlogPost.Dtos;
using AirJourney_Blog.Service.Services.Image;
using AirJourney_Blog.Service.Services.Resources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace AirJourney_Blog.PL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogController : ControllerBase
    {
        private readonly IBlogPostService blogService;
        private readonly IBlogCategoryService categoryService;
        private readonly IImageService imgSer;
        private readonly IStringLocalizer<SharedResources> stringLocalizer;

        public BlogController(
            IBlogPostService _blogService,
            IBlogCategoryService _categoryService,
            IImageService imgSer,
            IStringLocalizer<SharedResources> stringLocalizer
            )
        {
            blogService = _blogService;
            categoryService = _categoryService;
            this.imgSer = imgSer;
            this.stringLocalizer = stringLocalizer;
        }


        [HttpPost]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> CreateBlog([FromBody] BlogPostDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            try
            {
                var createdBlog = await blogService.CreateBlogAsync(model);
                return CreatedAtAction(
                    nameof(GetBlogById), 
                    new { id = createdBlog.Id }, 
                    createdBlog); 
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while creating the blog", detail = ex.Message });
            }
        }


        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetBlogById(int id)
        {
            try
            {
                var blog = await blogService.GetBlogByIdAsync(id);
                return Ok(blog);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
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

        [HttpGet("admin/{id:int}")]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> GetAdminBlogById(int id)
        {
            try
            {
                var blog = await blogService.GetAdminBlogByIdAsync(id);
                return Ok(blog);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
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

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> UpdateBlog(int id, [FromBody] BlogPostDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            try
            {

                var ExsitingModel = await blogService.GetBlogByIdAsync(id);

                var updatedBlog = await blogService.UpdateBlogAsync(id, model); 

                if (ExsitingModel.ImageUrl != updatedBlog.ImageUrl)
                    await imgSer.DeleteImageAsync(ExsitingModel.FileId);

                return Ok(updatedBlog);                  
            }
            catch (KeyNotFoundException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "An error occurred while updating the blog",
                    detail = ex.Message
                });
            }
        }


        [HttpGet]
        public async Task<ActionResult<PaginatedResult<BlogListDto>>> GetAllBlogsAsync(
            int? take = null,
            int? skip = null,
            string sortBy = "CreatedAt",
            string sortDirection = "Descending",
            int? categoryId = null,
            string searchTerm = null,
            DateTime? fromDate = null,
            DateTime? toDate = null
            )
        {
            try
            {
                if (take <= 0 || skip < 0)
                {
                    return BadRequest(new { message = "Invalid pagination parameters" });

                }

                var result = await blogService.GetAllBlogsAsync(
                    take,
                    skip,
                    sortBy,
                    sortDirection,
                    categoryId,
                    searchTerm,
                    fromDate,
                    toDate
                );
                return Ok( result );
            }
            catch(Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "An internal server error occurred",
                    detail = ex.Message
                });
            }
        }


        [HttpGet("admin")]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<ActionResult<PaginatedResult<AdminBlogListDto>>> GetAllAdminBlogsAsync(
            int? take = null,
            int? skip = null,
            string sortBy = "CreatedAt",
            string sortDirection = "Descending",
            int? categoryId = null,
            bool? isVisible = null,
            string searchTerm = null,
            DateTime? fromDate = null,
            DateTime? toDate = null)
        {
            try
            {
                if (take <= 0 || skip < 0)
                {
                    return BadRequest(new { message = "Invalid pagination parameters" });
                }

                var result = await blogService.GetAllAdminBlogsAsync(
                    take,
                    skip,
                    sortBy,
                    sortDirection,
                    categoryId,
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
                    detail = ex.Message
                });
            }
        }


        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> DeleteBlog(int id)
        {
            try
            {
                var deletedBlog = await blogService.DeleteBlogAsync(id);
                if (!string.IsNullOrEmpty(deletedBlog.FileId))
                    await imgSer.DeleteImageAsync(deletedBlog.FileId);
                return Ok("Blog deleted successfully.");
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "An error occurred while deleting the blog",
                    detail = ex.Message
                });
            }
        }


        [HttpPut("ToggleActivation/{_id}")]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> ToggleBlogActivation(int _id)
        {
            try
            {
                var updatedBlog = await blogService.ToggleBlogActivationAsync(_id);
                return Ok(new
                {
                    id = _id,
                    isActive = updatedBlog.IsVisible,
                    message = $"Blog activation status updated to {(updatedBlog.IsVisible ? "Active" : "Inactive")}."
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
                    message = "An error occurred while toggling blog activation",
                    detail = ex.Message
                });
            }
        }
    }
}
