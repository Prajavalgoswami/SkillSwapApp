
using Microsoft.AspNetCore.Identity;

namespace SkillSwapApp.Models
{
    public class ApplicationUser : IdentityUser
    {
        // Extra fields for your project
        public string FullName { get; set; }
        public string OfferedSkill { get; set; }
        public string NeededSkill { get; set; }
    }
}
