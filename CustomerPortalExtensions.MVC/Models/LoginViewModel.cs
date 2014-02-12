using System.ComponentModel.DataAnnotations;

namespace CustomerPortalExtensions.MVC.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Please enter your user name")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Please enter your password")]
        public string Password { get; set; }
       
    }
}