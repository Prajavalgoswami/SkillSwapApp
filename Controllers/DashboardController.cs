using Microsoft.AspNetCore.Mvc;
using SkillSwapApp.Data;
using SkillSwapApp.Models;
using SkillSwapApp.Repositories;
using SkillSwapApp.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace SkillSwapApp.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ISkillRepository _skillRepo;

        public DashboardController(ApplicationDbContext context, ISkillRepository skillRepo)
        {
            _context = context;
            _skillRepo = skillRepo;
        }

        public IActionResult Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }

            var user = _context.Users.FirstOrDefault(u => u.Id == userId);
            var skills = _skillRepo.GetSkillsByUser(userId) ?? new List<Skill>();
            var otherUsers = _context.Users.Where(u => u.Id != userId).ToList();

            var incomingRequests = _context.SwapRequests
                .Where(r => r.ToUserId == userId && r.Status == "Pending")
                .ToList();

            var sentRequests = _context.SwapRequests
                .Where(r => r.FromUserId == userId)
                .ToList();

            var vm = new DashboardViewModel
            {
                User = user,
                Skills = skills.ToList(),
                OtherUsers = otherUsers,
                IncomingRequests = incomingRequests,
                SentRequests = sentRequests
            };

            return View(vm);
        }

        // Send a swap request
        // Send a swap request
        public IActionResult RequestSwap(string userId)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var fromUser = _context.Users.FirstOrDefault(u => u.Id == currentUserId);
            var toUser = _context.Users.FirstOrDefault(u => u.Id == userId);

            if (fromUser == null || toUser == null)
                return RedirectToAction("Index");

            var request = new SwapRequest
            {
                FromUserId = fromUser.Id,
                ToUserId = toUser.Id,
                OfferedSkill = fromUser.OfferedSkill,  // ✅ now valid
                NeededSkill = fromUser.NeededSkill,    // ✅ now valid
                Status = "Pending"
            };

            _context.SwapRequests.Add(request);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }


        // Accept request
        public IActionResult AcceptRequest(int requestId)
        {
            var request = _context.SwapRequests.FirstOrDefault(r => r.Id == requestId);
            if (request != null)
            {
                request.Status = "Accepted";
                _context.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        // Decline request
        public IActionResult DeclineRequest(int requestId)
        {
            var request = _context.SwapRequests.FirstOrDefault(r => r.Id == requestId);
            if (request != null)
            {
                request.Status = "Declined";
                _context.SaveChanges();
            }

            return RedirectToAction("Index");
        }
    }
}
