using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SkillSwapApp.Models;
using SkillSwapApp.Repositories;
using SkillSwapApp.ViewModels;
using System.Linq;

namespace SkillSwapApp.Controllers
{
    public class DashboardController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ISkillRepository _skillRepo;

        public DashboardController(UserManager<ApplicationUser> userManager, ISkillRepository skillRepo)
        {
            _userManager = userManager;
            _skillRepo = skillRepo;
        }

        public IActionResult Index()
        {
            var userId = _userManager.GetUserId(User);
            var user = _userManager.Users.FirstOrDefault(u => u.Id == userId);
            var skills = _skillRepo.GetSkillsByUser(userId);

            var vm = new DashboardViewModel
            {
                User = user,
                Skills = skills
            };

            return View(vm);
        }
    }
}
