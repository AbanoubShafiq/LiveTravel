using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirJourney_Blog.Service.Services.Blog.Dtos
{
    public class BlogDetatilsDto
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public string ImageUrl { get; set; }
        public string FileId { get; set; }
        public string CategoryName{ get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
