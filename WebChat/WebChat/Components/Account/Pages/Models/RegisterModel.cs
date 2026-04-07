using System.ComponentModel.DataAnnotations;

namespace WebChat.Components.Account.Pages.Models
{
    public class RegisterModel
    {
        [Required(ErrorMessage = "Email required")]
        [EmailAddress(ErrorMessage = "Invalid email")]
        public string Email { get; set; } = "";

        [Required(ErrorMessage = "Username required")]
        public string Username { get; set; } = "";

        [Required(ErrorMessage = "Password required")]
        [MinLength(6, ErrorMessage = "Min 6 chars")]
        public string Password { get; set; } = "";

        [Compare("Password", ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; } = "";
    }
}
