using System.Collections.Generic;
using System.Threading.Tasks;
using ObjectEnvironmentPlacer.Objects;

namespace ObjectEnvironmentPlacer.Interface
{
    public interface IUserRepository
    {
        Task<ApplicationUser> RegisterAsync(string username, string email, string password);
        Task<ApplicationUser> GetByIdAsync(string id);
        Task<IEnumerable<ApplicationUser>> GetAllAsync();
        Task<bool> UpdateAsync(ApplicationUser user);
        Task<bool> DeleteAsync(string id);
    }
}
