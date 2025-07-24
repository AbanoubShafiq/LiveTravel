using AirJourney_Blog.BLL.Helper;
using AirJourney_Blog.BLL.Ordering;
using AirJourney_Blog.Service.Services.Blog.Dtos;
using AirJourney_Blog.Service.Services.BlogPost.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirJourney_Blog.Service.Services.BlogPost
{
    public interface IBlogPostService
    {


        public Task<BlogPostDto> CreateBlogAsync(BlogPostDto Model);
        public Task<BlogDetatilsDto> GetBlogByIdAsync(int id);

        public Task<BlogPostDto> UpdateBlogAsync(int id, BlogPostDto model);

        public Task<BlogPostDto> DeleteBlogAsync(int id);
        public Task<AdminBlogDetatilsDto> ToggleBlogActivationAsync(int id);

        public Task DisableBlogsForCategoryAsync(int categoryId);

        public Task<PaginatedResult<BlogListDto>> GetAllBlogsAsync(
            int? take = null,
            int? skip = null,
            string sortBy = "CreatedAt",
            string sortDirection = OrderBy.Descending,
            int? categoryId = null,
            string searchTerm = null,
            DateTime? fromDate = null,
            DateTime? toDate = null);

        public Task<PaginatedResult<AdminBlogListDto>> GetAllAdminBlogsAsync(
            int? take = null,
            int? skip = null,
            string sortBy = "CreatedAt",
            string sortDirection = OrderBy.Descending,
            int? categoryId = null,
            bool? isVisible = null,
            string searchTerm = null,
            DateTime? fromDate = null,
            DateTime? toDate = null);

        public Task<AdminBlogDetatilsDto> GetAdminBlogByIdAsync(int id);


    }
}
