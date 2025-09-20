using Microsoft.AspNetCore.Mvc;
using SkillSwapApp.Data;
using SkillSwapApp.Models;
using System.Linq;
using System.Security.Claims;

namespace SkillSwapApp.Controllers
{
    public class ChatController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ChatController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ✅ Individual chat page
        public IActionResult Chat(string userId)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var messages = _context.Messages
                .Where(m => (m.FromUserId == currentUserId && m.ToUserId == userId) ||
                            (m.FromUserId == userId && m.ToUserId == currentUserId))
                .OrderBy(m => m.SentAt)
                .ToList();

            ViewBag.CurrentUserId = currentUserId;
            ViewBag.OtherUserId = userId;

            return View(messages); // uses your Chat.cshtml
        }
    }
}
