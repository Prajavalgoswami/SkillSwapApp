using Microsoft.AspNetCore.Identity;
using SkillSwapApp.Data;
using SkillSwapApp.Models;
using SkillSwapApp.ViewModels;
using System.Linq;

namespace SkillSwapApp.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly ApplicationDbContext _context;

        public AccountRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public bool Register(RegisterViewModel model)
        {
            // Check if email already exists
            var existingUser = _context.Users.FirstOrDefault(u => u.Email == model.Email);
            if (existingUser != null)
                return false;

            // Create new user (⚠️ password stored as plain text for simplicity, 
            // in real apps you must hash it)
            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                FullName = model.FullName,
                OfferedSkill = model.OfferedSkill,
                NeededSkill = model.NeededSkill,
                PasswordHash = model.Password // simplified
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            return true;
        }

        public bool Login(LoginViewModel model)
        {
            // Check if user exists with given email & password
            var user = _context.Users
                .FirstOrDefault(u => u.Email == model.Email && u.PasswordHash == model.Password);

            return user != null;
        }
        public bool UserExists(string email)
        {
            return _context.Users.Any(u => u.Email == email);
        }


        public void Logout()
        {
            // For simplicity, we can skip real logout logic
            // In MVC, you can just clear session/cookies if you add them
        }
    }
}
