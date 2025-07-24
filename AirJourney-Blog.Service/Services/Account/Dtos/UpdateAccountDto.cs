using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirJourney_Blog.Service.Services.Account.Dtos
{
    public class UpdateAccountDto
    {
        [Required(ErrorMessage = "First name is required")]
        [StringLength(50, ErrorMessage = "First name cannot exceed 50 characters")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required")]
        [StringLength(50, ErrorMessage = "Last name cannot exceed 50 characters")]
        public string LastName { get; set; }
        public string? ProfilePicture { get; set; }
        public string? PictureId { get; set; }
        [DataType(DataType.Date)]
        [CustomValidation(typeof(UpdateAccountDto), "ValidateDateOfBirth")]
        public DateTime? DateOfBirth { get; set; }

        [Required(ErrorMessage = "User Name is required")]
        [StringLength(50, ErrorMessage = "User Name cannot exceed 50 characters")]
        public virtual string? UserName { get; set; }


        [Phone(ErrorMessage = "Invalid phone number format")]
        [StringLength(20, ErrorMessage = "Phone number cannot exceed 20 characters")]
        public string? PhoneNumber { get; set; }

        public static ValidationResult ValidateDateOfBirth(DateTime? dateOfBirth, ValidationContext context)
        {
            if (dateOfBirth > DateTime.Today)
            {
                return new ValidationResult("Date of birth cannot be in the future");
            }
            return ValidationResult.Success;
        }
    }



}
