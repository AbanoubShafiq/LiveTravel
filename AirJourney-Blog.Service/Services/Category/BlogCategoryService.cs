using AirJourney_Blog.BLL.Helper;
using AirJourney_Blog.BLL.Implementation;
using AirJourney_Blog.BLL.Interfaces;
using AirJourney_Blog.BLL.Ordering;
using AirJourney_Blog.Service.Services.Blog.Dtos;
using AirJourney_Blog.Service.Services.BlogPost;
using AirJourney_Blog.Service.Services.Category.Dtos;
using AirJourney_Blog.Service.Services.Resources;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AirJourney_Blog.Service.Services.BlogCategory
{
    public class BlogCategoryService : IBlogCategoryService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly IBlogPostService blogService;
        private readonly IStringLocalizer<SharedResources> stringLocalizer;

        public BlogCategoryService(
            IUnitOfWork _unitOfWork,
            IMapper _mapper,
            IBlogPostService _blogService,
            IStringLocalizer<SharedResources> stringLocalizer)
        {
            unitOfWork = _unitOfWork;
            mapper = _mapper;
            blogService = _blogService;
            this.stringLocalizer = stringLocalizer;
        }
        public async Task<CategoryDto> CreateCategoryAsync(CategoryDto dto)
        {
            // Validate that names in both languages don't exist
            bool nameEnExists = await unitOfWork.GenericRepository<DAL.Models.BlogCategory>()
                .AnyAsync(c => c.NameEn.ToLower() == dto.NameEn.Trim().ToLower());

            bool nameEsExists = await unitOfWork.GenericRepository<DAL.Models.BlogCategory>()
                .AnyAsync(c => c.NameEs.ToLower() == dto.NameEs.Trim().ToLower());

            if (nameEnExists)
            {
                throw new ArgumentException("An English category with this name already exists");
            }

            if (nameEsExists)
            {
                throw new ArgumentException("A Spanish category with this name already exists");
            }

            var model = mapper.Map<DAL.Models.BlogCategory>(dto);
            await unitOfWork.GenericRepository<DAL.Models.BlogCategory>().AddAsync(model);
            await unitOfWork.CompleteAsync();

            return mapper.Map<CategoryDto>(model);
        }

        public async Task<CategoryDto> UpdateCategoryAsync(int id, CategoryDto dto)
        {
            var existingCategory = await unitOfWork.GenericRepository<DAL.Models.BlogCategory>().GetByIdAsync(id);
            if (existingCategory == null)
                throw new KeyNotFoundException($"No blog category with ID {id} found");

            // Check for duplicate names in English (excluding current category)
            bool nameEnExists = await unitOfWork.GenericRepository<DAL.Models.BlogCategory>()
                .AnyAsync(c => c.Id != id && c.NameEn.ToLower() == dto.NameEn.Trim().ToLower());

            // Check for duplicate names in Spanish (excluding current category)
            bool nameEsExists = await unitOfWork.GenericRepository<DAL.Models.BlogCategory>()
                .AnyAsync(c => c.Id != id && c.NameEs.ToLower() == dto.NameEs.Trim().ToLower());

            if (nameEnExists)
            {
                throw new ArgumentException("An English category with this name already exists");
            }

            if (nameEsExists)
            {
                throw new ArgumentException("A Spanish category with this name already exists");
            }

            // Update the category
            existingCategory.NameEn = dto.NameEn;
            existingCategory.DescriptionEn = dto.DescriptionEn;
            existingCategory.NameEs = dto.NameEs;
            existingCategory.DescriptionEs = dto.DescriptionEs;

            unitOfWork.GenericRepository<DAL.Models.BlogCategory>().Update(existingCategory);
            await unitOfWork.CompleteAsync();

            return mapper.Map<CategoryDto>(existingCategory);
        }


        public async Task<PaginatedResult<AdminCategoryListDto>> GetAllAdminCategoriesAsync(
            int? take = null,
            int? skip = null,
            string sortBy = "CreatedAt",
            string sortDirection = OrderBy.Descending,
            bool? isVisible = null,
            string searchTerm = null,
            DateTime? fromDate = null,
            DateTime? toDate = null)
        {
            sortBy = sortBy?.Trim().ToLowerInvariant();

            Expression<Func<DAL.Models.BlogCategory, object>> orderBy = sortBy switch
            {
                "nameen" => x => x.NameEn,
                "namees" => x => x.NameEs,
                "createdat" => x => x.CreatedAt,
                "isvisible" => x => x.IsVisible,
                _ => x => x.CreatedAt
            };

            Expression<Func<DAL.Models.BlogCategory, bool>> filter = category =>
                (isVisible == null || category.IsVisible == isVisible) &&
                (string.IsNullOrWhiteSpace(searchTerm) ||
                    category.NameEn.Contains(searchTerm) ||
                    category.NameEs.Contains(searchTerm)) &&
                (fromDate == null || category.CreatedAt >= fromDate) &&
                (toDate == null || category.CreatedAt <= toDate);

            var categories = await unitOfWork
                .GenericRepository<DAL.Models.BlogCategory>()
                .GetAllAsync(
                    take: take,
                    skip: skip,
                    orderBy: orderBy,
                    orderByDirection: sortDirection,
                    criteria: filter
                );

            var dtoList = mapper.Map<List<AdminCategoryListDto>>(categories.Items);

            return new PaginatedResult<AdminCategoryListDto>
            {
                Items = dtoList,
                CurrentPage = categories.CurrentPage,
                PageSize = categories.PageSize,
                TotalPages = categories.TotalPages
            };
        }


        public async Task<List<CategoryDropdownDto>> GetCategoryDropdownAsync ()
        {
            var categories = await unitOfWork.GenericRepository<DAL.Models.BlogCategory>()
                .GetAllAsync(criteria: x => x.IsVisible);

            return mapper.Map<List<CategoryDropdownDto>>(categories.Items);
        }
        public async Task<List<AdminCategoryDropdownDto>> GetAdminCategoryDropdownAsync()
        {
            var categories = await unitOfWork.GenericRepository<DAL.Models.BlogCategory>()
                .GetAllAsync(); 

            return mapper.Map<List<AdminCategoryDropdownDto>>(categories.Items);
        }
        public async Task<CategoryDetailDto> GetCategoryByIDAsync(int id)
        {

            var model = await unitOfWork.GenericRepository<DAL.Models.BlogCategory>().GetByIdAsync(id);
            if (model == null)
                throw new KeyNotFoundException(stringLocalizer[SharedResourcesKeys.CategoryNotFound]);
            var dto = mapper.Map<CategoryDetailDto>(model);
            return dto;
        }

        public async Task<AdminCategoryDetailDto> GetAdminCategoryByIDAsync(int id)
        {

            var model = await unitOfWork.GenericRepository<DAL.Models.BlogCategory>().GetByIdAsync(id);
            if (model == null)
                throw new KeyNotFoundException("Category Not Found");
            var dto = mapper.Map<AdminCategoryDetailDto>(model);
            return dto;
        }


        public async Task<AdminCategoryDetailDto> ToggleCategoryActivationAsync(int id)
        {

            var category = await unitOfWork.GenericRepository<DAL.Models.BlogCategory>().GetByIdAsync(id);
            if (category == null)
                throw new KeyNotFoundException($"No Category With that Id {id}");
            // Toggle the activation status
            category.IsVisible = !category.IsVisible;

            if (!category.IsVisible)
            {
                await blogService.DisableBlogsForCategoryAsync(category.Id);
            }

            unitOfWork.GenericRepository<DAL.Models.BlogCategory>().Update(category);
            await unitOfWork.CompleteAsync();
            return mapper.Map<AdminCategoryDetailDto>(category);
        }


    }
}
