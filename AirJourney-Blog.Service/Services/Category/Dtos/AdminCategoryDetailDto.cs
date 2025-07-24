using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirJourney_Blog.Service.Services.Category.Dtos
{
    public class AdminCategoryDetailDto
    {
        public string NameEn { get; set; }
        public string DescriptionEn { get; set; }
        public string NameEs { get; set; }
        public string DescriptionEs { get; set; }
        public int NoOfBlogs { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsVisible { get; set; }

    }
}
