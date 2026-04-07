using Microsoft.AspNetCore.SignalR;
using WebChat.Data;
using WebChat.Components.Account.Models;

namespace WebChat.Components.Account.Hubs
{
    public class ChatHub : Hub
    {
        private readonly ApplicationDbContext _db;

        public ChatHub(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task JoinChat(int chatId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"chat_{chatId}");
        }

        public async Task SendMessage(int chatId, string userId, string text)
        {
            var message = new Message
            {
                ChatId = chatId,
                SenderId = userId,
                Text = text
            };

            _db.Messages.Add(message);
            await _db.SaveChangesAsync();

            await Clients.Group($"chat_{chatId}")
                .SendAsync("ReceiveMessage", chatId, userId, text, message.CreatedAt);
        }
    }
}
