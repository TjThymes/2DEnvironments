using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ObjectEnvironmentPlacer.Objects;
using ObjectEnvironmentPlacer.Data;
using ObjectEnvironmentPlacer.Interface;

namespace ObjectEnvironmentPlacer.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _dbContext;

        public UserRepository(UserManager<ApplicationUser> userManager, ApplicationDbContext dbContext)
        {
            _userManager = userManager;
            _dbContext = dbContext;
        }


    public async Task<ApplicationUser> RegisterAsync(string username, string email, string password)
{

        var user = new ApplicationUser
        {
            UserName = username,  
            Email = email,        
            Name = username       
        };

        var result = await _userManager.CreateAsync(user, password);
        return result.Succeeded ? user : null;
}


        // ✅ Get User By ID
        public async Task<ApplicationUser> GetByIdAsync(string id)
        {
            return await _dbContext.Users.FindAsync(id);
        }

        // ✅ Get All Users
        public async Task<IEnumerable<ApplicationUser>> GetAllAsync()
        {
            return await _dbContext.Users.ToListAsync();
        }

        // ✅ Update User
        public async Task<bool> UpdateAsync(ApplicationUser user)
        {
            _dbContext.Users.Update(user);
            return await _dbContext.SaveChangesAsync() > 0;
        }

        // ✅ Delete User
        public async Task<bool> DeleteAsync(string id)
        {
            var user = await _dbContext.Users.FindAsync(id);
            if (user == null) return false;

            _dbContext.Users.Remove(user);
            return await _dbContext.SaveChangesAsync() > 0;
        }
        public async Task<bool> UserExistsAsync(string username)
        {
            return await _dbContext.Users.AnyAsync(u => u.UserName == username);
        }

    }
}
