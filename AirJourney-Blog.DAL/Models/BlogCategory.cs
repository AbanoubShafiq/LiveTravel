using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirJourney_Blog.DAL.Models
{
    public class BlogCategory: BaseEntity<int>
    {

        [Required(ErrorMessage = "Category name is required")]
        [MaxLength(100, ErrorMessage = "Name can't exceed 100 characters")]
        public string NameEn { get; set; }
        [MaxLength(500, ErrorMessage = "Description can't exceed 500 characters")]
        public string DescriptionEn { get; set; }
        [Required(ErrorMessage = "Category name is required")]
        [MaxLength(100, ErrorMessage = "Name can't exceed 100 characters")]
        public string NameEs { get; set; }
        [MaxLength(500, ErrorMessage = "Description can't exceed 500 characters")]
        public string DescriptionEs { get; set; }
        public virtual List<BlogPost> Blogs { get; set; } = new List<BlogPost>();
    }
}
