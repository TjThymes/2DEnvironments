using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ObjectEnvironmentPlacer.Objects;

namespace ObjectEnvironmentPlacer.Interface
{
    public interface IEnvironment2DRepository
    {
        Task<Environment2D> InsertAsync(string name, string? description, int width, int height);
        Task<IEnumerable<Environment2D>> GetAllAsync();
        Task<Environment2D> GetByIdAsync(Guid id);
        Task DeleteAsync(Guid id);
        Task<Environment2D?> GetByIdWithObjectsAsync(Guid id);
        Task<List<Environment2D>> GetByUserIdAsync(string userId);
    }
}
