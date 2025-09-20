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
        [HttpGet]
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
                OfferedSkill = fromUser.OfferedSkill,
                NeededSkill = fromUser.NeededSkill,
                Status = "Pending"
            };

            _context.SwapRequests.Add(request);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        // GET: Edit profile
        // Inside AccountController
        [HttpGet]
        public IActionResult Edit()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return RedirectToAction("Login", "Account");

            var user = _context.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null)
                return NotFound();

            return View(user); // looks for Views/Dashboard/Edit.cshtml
        }


        [HttpPost]
        public IActionResult Edit(ApplicationUser model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return RedirectToAction("Login", "Account");

            var user = _context.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null)
                return NotFound();

            user.FullName = model.FullName;
            user.UserName = model.UserName;
            user.Email = model.Email;
            user.OfferedSkill = model.OfferedSkill;
            user.NeededSkill = model.NeededSkill;

            _context.SaveChanges();

            return RedirectToAction("Index", "Dashboard");
        }

        // Accept request
        [HttpGet]
        public IActionResult AcceptRequest(int requestId)
        {
            var request = _context.SwapRequests.FirstOrDefault(r => r.Id == requestId);
            if (request != null)
            {
                request.Status = "Accepted";
                _context.SaveChanges();

                return RedirectToAction("Chat", new { requestId = request.Id });
            }

            return RedirectToAction("Index");
        }

        // Decline request
        [HttpGet]
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

        public IActionResult Chat()
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var acceptedRequests = _context.SwapRequests
                .Where(r => (r.FromUserId == currentUserId || r.ToUserId == currentUserId)
                            && r.Status == "Accepted")
                .Select(r => new
                {
                    Partner = r.FromUserId == currentUserId ? r.ToUser : r.FromUser
                })
                .ToList();

            return View(acceptedRequests.Select(a => a.Partner).ToList());
        }

        public IActionResult Chat(int requestId)
        {
            var request = _context.SwapRequests.FirstOrDefault(r => r.Id == requestId);
            if (request == null || request.Status != "Accepted")
                return RedirectToAction("Index");

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var otherUserId = request.FromUserId == currentUserId ? request.ToUserId : request.FromUserId;

            var messages = _context.Messages
                .Where(m => (m.FromUserId == currentUserId && m.ToUserId == otherUserId) ||
                            (m.FromUserId == otherUserId && m.ToUserId == currentUserId))
                .OrderBy(m => m.SentAt)
                .ToList();

            ViewBag.CurrentUserId = currentUserId;
            ViewBag.OtherUserId = otherUserId;

            return View(messages);
        }

        public IActionResult ChatList()
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var chats = _context.SwapRequests
                .Where(r => (r.FromUserId == currentUserId || r.ToUserId == currentUserId)
                            && r.Status == "Accepted")
                .Select(r => new
                {
                    Partner = r.FromUserId == currentUserId ? r.ToUser : r.FromUser
                })
                .ToList();

            return View(chats.Select(c => c.Partner).ToList());
        }
    }
}
