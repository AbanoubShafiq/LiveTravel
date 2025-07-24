using AirJourney_Blog.BLL.Helper;
using AirJourney_Blog.BLL.Ordering;
using AirJourney_Blog.Service.Services.Blog.Dtos;
using AirJourney_Blog.Service.Services.Category.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirJourney_Blog.Service.Services.BlogCategory
{
    public interface IBlogCategoryService
    {
        public Task<CategoryDto> CreateCategoryAsync(CategoryDto dto);
        Task<CategoryDto> UpdateCategoryAsync(int id, CategoryDto dto);
        public Task<PaginatedResult<AdminCategoryListDto>> GetAllAdminCategoriesAsync(
            int? take = null,
            int? skip = null,
            string sortBy = "CreatedAt",
            string sortDirection = OrderBy.Descending,
            bool? isVisible = null,
            string searchTerm = null,
            DateTime? fromDate = null,
            DateTime? toDate = null);
        Task<CategoryDetailDto> GetCategoryByIDAsync(int id);
        public Task<AdminCategoryDetailDto> ToggleCategoryActivationAsync(int id);

        public Task<List<CategoryDropdownDto>> GetCategoryDropdownAsync();
        public Task<List<AdminCategoryDropdownDto>> GetAdminCategoryDropdownAsync();
        public Task<AdminCategoryDetailDto> GetAdminCategoryByIDAsync(int id);

    }
}
