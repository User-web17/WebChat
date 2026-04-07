using WebChat.Data;

namespace WebChat.Components.Account.Models
{
    public class Chat
    {
        public int Id { get; set; }

        public List<ApplicationUser> Users { get; set; } = new();
        public List<Message> Messages { get; set; } = new();
    }
}
