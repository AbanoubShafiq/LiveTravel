using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirJourney_Blog.Service.Services.Category.Dtos
{
    public class CategoryDetailDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int NoOfBlogs { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
