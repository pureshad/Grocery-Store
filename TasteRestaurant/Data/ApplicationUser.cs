using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace TasteRestaurant.Data
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        [Display(Name = "Full name")]
        public string FirstName { get; set; }

        public string LastName { get; set; }

        [NotMapped]
        public string FullName
        {
            get { return FirstName + " " + LastName; }
            set { }
        }
    }
}
