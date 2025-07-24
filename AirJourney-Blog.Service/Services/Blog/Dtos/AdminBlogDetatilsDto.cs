using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirJourney_Blog.Service.Services.Blog.Dtos
{
    public class AdminBlogDetatilsDto
    {
        public string TitleEn { get; set; }
        public string TitleEs { get; set; }
        public string ContentEs { get; set; }
        public string ContentEn { get; set; }
        public string ImageUrl { get; set; }
        public string FileId { get; set; }
        public string CategoryName { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsVisible { get; set; }

    }
}
