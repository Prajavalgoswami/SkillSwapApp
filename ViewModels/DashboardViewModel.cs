using SkillSwapApp.Models;
using System.Collections.Generic;

namespace SkillSwapApp.ViewModels
{
    public class DashboardViewModel
    {
        public ApplicationUser User { get; set; }
        public IEnumerable<Skill> Skills { get; set; }
    }
}
