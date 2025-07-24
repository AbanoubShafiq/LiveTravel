using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirJourney_Blog.Service.Services.Category.Dtos
{
    public class CategoryDto
    {
        public int? Id { get; set; }

        // English properties
        [Required(ErrorMessage = "English category name is required")]
        [MaxLength(100, ErrorMessage = "English name can't exceed 100 characters")]
        public string NameEn { get; set; }
        [MaxLength(500, ErrorMessage = "English description can't exceed 500 characters")]
        public string DescriptionEn { get; set; }


        // Spanish properties
        [Required(ErrorMessage = "Spanish category name is required")]
        [MaxLength(100, ErrorMessage = "Spanish name can't exceed 100 characters")]
        public string NameEs { get; set; }
        [MaxLength(500, ErrorMessage = "Spanish description can't exceed 500 characters")]
        public string DescriptionEs { get; set; }
    }
}
