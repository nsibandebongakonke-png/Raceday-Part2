using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Raceday.Data
{
    // Add profile data for application users by adding properties to the IdentityUser class
   public enum Gender
    {
        Male,
        Female,
        Other,

       prefernottosay
    }
    public class ApplicationUser : IdentityUser
    {
        [Required, MaxLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Required, MaxLength(50)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        public Gender Gender { get; set; }

        [MaxLength(260)]
        public string? ProfilePictureUrl { get; set; } = string.Empty;

        public string fullName => $"{FirstName} {LastName}";
    }
}
