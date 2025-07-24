using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace AirJourney_Blog.Service.Services.BlogPost.Dtos
{
    public class BlogPostDto
    {
        public int Id { get; set; }

        // English properties
        [Required(ErrorMessage = "English title is required")]
        [MinLength(3, ErrorMessage = "English title should be at least 3 characters long")]
        [MaxLength(150, ErrorMessage = "English title can't exceed 150 characters")]
        public string TitleEn { get; set; }

        [Required(ErrorMessage = "English content is required")]
        [MinLength(10, ErrorMessage = "English content should be at least 10 characters long")]
        public string ContentEn { get; set; }

        // Spanish properties
        [Required(ErrorMessage = "Spanish title is required")]
        [MinLength(3, ErrorMessage = "Spanish title should be at least 3 characters long")]
        [MaxLength(150, ErrorMessage = "Spanish title can't exceed 150 characters")]
        public string TitleEs { get; set; }

        [Required(ErrorMessage = "Spanish content is required")]
        [MinLength(10, ErrorMessage = "Spanish content should be at least 10 characters long")]
        public string ContentEs { get; set; }

        // Shared properties
        [Required(ErrorMessage = "Image URL is required")]
        public string ImageUrl { get; set; }

        [Required(ErrorMessage = "Image ID is required")]
        public string FileId { get; set; }

        [Required(ErrorMessage = "Blog Category ID is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Blog Category ID must be greater than 0")]
        public int BlogCategoryId { get; set; }
    }
}
