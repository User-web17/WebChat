using System.ComponentModel.DataAnnotations;

namespace WebChat.Components.Account.Models
{
    public class LoginModel
    {

            [Required(ErrorMessage = "Email required")]
            [EmailAddress(ErrorMessage = "Invalid email")]
            public string Email { get; set; } = "";

            [Required(ErrorMessage = "Password required")]
            public string Password { get; set; } = "";

    }
}
