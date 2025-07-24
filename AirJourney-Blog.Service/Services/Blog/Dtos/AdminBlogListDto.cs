using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirJourney_Blog.Service.Services.Blog.Dtos
{
    public class AdminBlogListDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Excerpt { get; set; }
        public string ImageUrl { get; set; }
        public bool IsVisible { get; set; }
        public string BlogCategoryName { get; set; }
    }
}
