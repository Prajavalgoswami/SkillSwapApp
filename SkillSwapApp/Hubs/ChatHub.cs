using Microsoft.AspNetCore.SignalR;
using SkillSwapApp.Data;
using SkillSwapApp.Models;
using System;
using System.Threading.Tasks;

namespace SkillSwapApp.Hubs
{
    public class ChatHub : Hub
    {
        private readonly ApplicationDbContext _context;

        public ChatHub(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task SendMessage(string fromUser, string toUser, string message)
        {
            if (string.IsNullOrWhiteSpace(message))
                return;

            // ✅ Save to DB
            var newMessage = new Message
            {
                FromUserId = fromUser,
                ToUserId = toUser,
                Content = message,
                SentAt = DateTime.UtcNow
            };

            _context.Messages.Add(newMessage);
            await _context.SaveChangesAsync();

            // ✅ Send in real-time
            await Clients.User(toUser).SendAsync("ReceiveMessage", fromUser, message);

            // Optional: also show sender’s own message immediately
            await Clients.User(fromUser).SendAsync("ReceiveMessage", fromUser, message);
        }
    }
}
