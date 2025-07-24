using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirJourney_Blog.DAL.Models
{
    public class BlogPost: BaseEntity<int>
    {
        // English properties
        [Required(ErrorMessage = "English title is required")]
        [MaxLength(150, ErrorMessage = "English title can't exceed 150 characters")]
        public string TitleEn { get; set; }

        [Required(ErrorMessage = "English content is required")]
        public string ContentEn { get; set; }

        // Spanish properties
        [Required(ErrorMessage = "Spanish title is required")]
        [MaxLength(150, ErrorMessage = "Spanish title can't exceed 150 characters")]
        public string TitleEs { get; set; }

        [Required(ErrorMessage = "Spanish content is required")]
        public string ContentEs { get; set; }
        public string ImageUrl { get; set; }
        public string FileId { get; set; }
        [ForeignKey("BlogCategoryId")]
        public virtual BlogCategory BlogCategory { get; set; }
        [Required]
        public int BlogCategoryId { get; set; }
    }
}
