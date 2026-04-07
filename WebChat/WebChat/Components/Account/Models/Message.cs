using WebChat.Data;

namespace WebChat.Components.Account.Models
{
    public class Message
    {
        public int Id { get; set; }

        public string Text { get; set; } = "";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public string SenderId { get; set; } = "";
        public ApplicationUser Sender { get; set; } = null!;

        public int ChatId { get; set; }
        public Chat Chat { get; set; } = null!;
    }
}
