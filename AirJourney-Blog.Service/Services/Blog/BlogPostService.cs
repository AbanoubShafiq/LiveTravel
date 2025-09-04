using AirJourney_Blog.BLL.Helper;
using AirJourney_Blog.BLL.Interfaces;
using AirJourney_Blog.BLL.Ordering;
using AirJourney_Blog.Service.Services.Blog.Dtos;
using AirJourney_Blog.Service.Services.BlogPost.Dtos;
using AirJourney_Blog.Service.Services.Resources;
using AutoMapper;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace AirJourney_Blog.Service.Services.BlogPost
{
    public class BlogPostService : IBlogPostService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper map;
        private readonly IStringLocalizer<SharedResources> stringLocalizer;

        public BlogPostService(IUnitOfWork _unitOfWork,
            IMapper _map,
            IStringLocalizer<SharedResources> stringLocalizer)
        {
            unitOfWork = _unitOfWork;
            map = _map;
            this.stringLocalizer = stringLocalizer;
        }

        public async Task<BlogPostDto> CreateBlogAsync(BlogPostDto model)
        {
            var category = await unitOfWork.GenericRepository<DAL.Models.BlogCategory>()
                .GetByIdAsync(model.BlogCategoryId);

            if (category == null)
                throw new ArgumentException($"No category found with ID {model.BlogCategoryId}");

            var existingEn = await unitOfWork.GenericRepository<DAL.Models.BlogPost>()
                .FindAsync(blog => blog.TitleEn == model.TitleEn && blog.BlogCategoryId == model.BlogCategoryId);

            if (existingEn != null)
                throw new ArgumentException($"A blog with the English title '{model.TitleEn}' already exists in the '{category.NameEn}' category.");

            var existingEs = await unitOfWork.GenericRepository<DAL.Models.BlogPost>()
                .FindAsync(blog => blog.TitleEs == model.TitleEs && blog.BlogCategoryId == model.BlogCategoryId);

            if (existingEs != null)
                throw new ArgumentException($"A blog with the Spanish title '{model.TitleEs}' already exists in the '{category.NameEs}' category.");

            var blog = map.Map<DAL.Models.BlogPost>(model);

            if (!category.IsVisible)
                blog.IsVisible = false;

            await unitOfWork.GenericRepository<DAL.Models.BlogPost>().AddAsync(blog);
            await unitOfWork.CompleteAsync();

            return map.Map<BlogPostDto>(blog);
        }


        public async Task<BlogDetatilsDto> GetBlogByIdAsync(int id)
        {
            var model = await unitOfWork.GenericRepository<DAL.Models.BlogPost>().GetByIdAsync(id);
            if (model == null || !model.IsVisible)
                throw new KeyNotFoundException(stringLocalizer[SharedResourcesKeys.BlogNotFound]);
            var res = map.Map<BlogDetatilsDto>(model);
            return res;
        }

        public async Task<AdminBlogDetatilsDto> GetAdminBlogByIdAsync(int id)
        {
            var model = await unitOfWork.GenericRepository<DAL.Models.BlogPost>()
                .GetByIdAsync(id);

            if (model == null)
                throw new KeyNotFoundException("Blog not found");
            var res = map.Map<AdminBlogDetatilsDto>(model);
            return res;
        }


        public Task<PaginatedResult<BlogListDto>> GetAllBlogsAsync(
            int? take = null,
            int? skip = null,
            string sortBy = "CreatedAt",
            string sortDirection = OrderBy.Descending,
            int? categoryId = null,
            string searchTerm = null,
            DateTime? fromDate = null,
            DateTime? toDate = null)
        {
            return GetPaginatedBlogsAsync<BlogListDto>(
                take, skip, sortBy, sortDirection, categoryId, searchTerm, fromDate, toDate, true, false);
        }

        public Task<PaginatedResult<AdminBlogListDto>> GetAllAdminBlogsAsync(
            int? take = null,
            int? skip = null,
            string sortBy = "CreatedAt",
            string sortDirection = OrderBy.Descending,
            int? categoryId = null,
            bool? isVisible = null,
            string searchTerm = null,
            DateTime? fromDate = null,
            DateTime? toDate = null)
        {
            return GetPaginatedBlogsAsync<AdminBlogListDto>(
                take, skip, sortBy, sortDirection, categoryId, searchTerm, fromDate, toDate, isVisible, true);
        }


        private async Task<PaginatedResult<TDto>> GetPaginatedBlogsAsync<TDto> (
            int? take,
            int? skip,
            string sortBy,
            string sortDirection,
            int? categoryId,
            string searchTerm,
            DateTime? fromDate,
            DateTime? toDate,
            bool? isVisible,
            bool forAdmin) where TDto : class
        {
            // Determine sorting field
            Expression<Func<DAL.Models.BlogPost, object>> orderBy = sortBy switch
            {
                "TitleEn" => x => x.TitleEn,
                "TitleEs" => x => x.TitleEs,
                "CreatedAt" => x => x.CreatedAt,
                _ => x => x.CreatedAt
            };

            // Build filtering expression
            Expression<Func<DAL.Models.BlogPost, bool>> filter = blog =>
                (isVisible == null || blog.IsVisible == isVisible) &&
                (categoryId == null || blog.BlogCategoryId == categoryId) &&
                (string.IsNullOrEmpty(searchTerm) ||
                    blog.TitleEn.Contains(searchTerm) ||
                    blog.TitleEs.Contains(searchTerm)) &&
                (fromDate == null || blog.CreatedAt >= fromDate) &&
                (toDate == null || blog.CreatedAt <= toDate);

            var blogs = await unitOfWork.GenericRepository<DAL.Models.BlogPost>()
                .GetAllAsync(
                    take: take,
                    skip: skip,
                    orderBy: orderBy,
                    orderByDirection: sortDirection,
                    criteria: filter
                );

            var mappedList = map.Map<List<TDto>>(blogs.Items);

            return new PaginatedResult<TDto>
            {
                Items = mappedList,
                CurrentPage = blogs.CurrentPage,
                PageSize = blogs.PageSize,
                TotalPages = blogs.TotalPages
            };
        }



        public async Task DisableBlogsForCategoryAsync(int categoryId)
        {
            if (categoryId <= 0)
                throw new ArgumentException("Category ID must be greater than 0.");

            var category = await unitOfWork.GenericRepository<DAL.Models.BlogCategory>().GetByIdAsync(categoryId);
            if (category == null)
                throw new ArgumentException("No Category with That Id");


            var relatedBlogs = await unitOfWork.GenericRepository<DAL.Models.BlogPost>().GetFilteredAsync(
                criteria: x => x.BlogCategoryId == categoryId);

            if (relatedBlogs.Count == 0)
                return;

            foreach(var blog in  relatedBlogs)
            { 
                blog.IsVisible = false;
            };

            await unitOfWork.CompleteAsync();
        }

        public async Task<AdminBlogDetatilsDto> ToggleBlogActivationAsync(int id)
        {
            var blog = await unitOfWork.GenericRepository<DAL.Models.BlogPost>()
                .GetByIdAsync(id);

            if (blog == null)
                throw new KeyNotFoundException("No blog with that ID");

            var category = await unitOfWork.GenericRepository<DAL.Models.BlogCategory>()
                .GetByIdAsync(blog.BlogCategoryId);

            // Block toggle if the category is hidden (unless forcing visibility)
            if (category.IsVisible == false && blog.IsVisible == false)
            {
                throw new InvalidOperationException(
                    "Cannot activate this blog because its category is hidden. " +
                    "Make the category visible first."
                );
            }

            blog.IsVisible = !blog.IsVisible;

            unitOfWork.GenericRepository<DAL.Models.BlogPost>().Update(blog);
            await unitOfWork.CompleteAsync();

            return map.Map<AdminBlogDetatilsDto>(blog);
        }

        public async Task<BlogPostDto> UpdateBlogAsync(int id, BlogPostDto model)
        {
            var blog = await unitOfWork.GenericRepository<DAL.Models.BlogPost>().GetByIdAsync(id);
            if (blog == null)
                throw new KeyNotFoundException("Blog not found");

            var category = await unitOfWork.GenericRepository<DAL.Models.BlogCategory>().GetByIdAsync(model.BlogCategoryId);
            if (category == null)
                throw new ArgumentException($"No category found with ID {model.BlogCategoryId}");

            var existingEn = await unitOfWork.GenericRepository<DAL.Models.BlogPost>()
                .FindAsync(x => x.Id != id && x.TitleEn == model.TitleEn && x.BlogCategoryId == model.BlogCategoryId);
            if (existingEn != null)
                throw new ArgumentException($"A blog with the English title '{model.TitleEn}' already exists in the '{category.NameEn}' category.");

            var existingEs = await unitOfWork.GenericRepository<DAL.Models.BlogPost>()
                .FindAsync(x => x.Id != id && x.TitleEs == model.TitleEs && x.BlogCategoryId == model.BlogCategoryId);
            if (existingEs != null)
                throw new ArgumentException($"A blog with the Spanish title '{model.TitleEs}' already exists in the '{category.NameEn}' category.");

            map.Map(model, blog);
            blog.Id = id;


            if (!category.IsVisible)
                blog.IsVisible = false;

            unitOfWork.GenericRepository<DAL.Models.BlogPost>().Update(blog);
            await unitOfWork.CompleteAsync();

            return map.Map<BlogPostDto>(blog);
        }


        public async Task<BlogPostDto> DeleteBlogAsync(int id)
        {

            var blog = await unitOfWork.GenericRepository<DAL.Models.BlogPost>().GetByIdAsync(id);
            if (blog == null)
                throw new KeyNotFoundException("No blog with that ID");
            var deletedBlogDto = map.Map<BlogPostDto>(blog);
            unitOfWork.GenericRepository<DAL.Models.BlogPost>().Delete(blog);
            await unitOfWork.CompleteAsync();

            return deletedBlogDto; 

        }


    }
}
