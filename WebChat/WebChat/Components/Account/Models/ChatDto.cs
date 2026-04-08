using static WebChat.Components.Account.Pages.Chat;

namespace WebChat.Components.Account.Models
{
    public class ChatDto
    {
        public int Id { get; set; }
        public List<UserDto> Users { get; set; } = new();
    }
}
