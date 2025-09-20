using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using SkillSwapApp.Models;
using SkillSwapApp.Repositories;

namespace SkillSwapApp.Controllers
{
    public class SkillController : Controller
    {
        private readonly ISkillRepository _skillRepo;
        private readonly UserManager<ApplicationUser> _userManager;

        public SkillController(ISkillRepository skillRepo, UserManager<ApplicationUser> userManager)
        {
            _skillRepo = skillRepo;
            _userManager = userManager;
        }

        // My Skills (Dashboard)
        public IActionResult Index()
        {
            var userId = _userManager.GetUserId(User);
            var skills = _skillRepo.GetSkillsByUser(userId);
            return View(skills);
        }

        // Add Skill (GET)
        public IActionResult Create()
        {
            return View();
        }

        // Add Skill (POST)
        [HttpPost]
        public IActionResult Create(Skill skill)
        {
            if (ModelState.IsValid)
            {
                skill.UserId = _userManager.GetUserId(User);
                _skillRepo.AddSkill(skill);
                return RedirectToAction("Index");
            }
            return View(skill);
        }

        // Edit Skill
        public IActionResult Edit(int id)
        {
            var skill = _skillRepo.GetSkill(id);
            return View(skill);
        }

        [HttpPost]
        public IActionResult Edit(Skill skill)
        {
            if (ModelState.IsValid)
            {
                _skillRepo.UpdateSkill(skill);
                return RedirectToAction("Index");
            }
            return View(skill);
        }

        // Delete Skill
        public IActionResult Delete(int id)
        {
            var skill = _skillRepo.GetSkill(id);
            return View(skill);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            _skillRepo.DeleteSkill(id);
            return RedirectToAction("Index");
        }
    }
}