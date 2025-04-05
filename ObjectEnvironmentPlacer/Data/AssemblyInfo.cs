using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ObjectEnvironmentPlacer.Objects;

namespace ObjectEnvironmentPlacer.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser> // ? Fixes the error
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Environment2D> Environments { get; set; } // ? Ensure Environments is included
        public DbSet<GameObject2D> GameObjects { get; set; }
    }
}
