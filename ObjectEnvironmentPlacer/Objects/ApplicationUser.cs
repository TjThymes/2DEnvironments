using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace ObjectEnvironmentPlacer.Objects
{
    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set; } // ✅ Separate Full Name Field

        // ✅ Navigation Property: A user can have multiple environments
        public List<Environment2D> Environments { get; set; } = new List<Environment2D>();
    }
}
