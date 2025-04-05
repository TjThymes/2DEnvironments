using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ObjectEnvironmentPlacer.Objects;
namespace ObjectEnvironmentPlacer.Interface
{

    public interface IObjectRepository
    {
        Task<IEnumerable<GameObject2D>> GetAllAsync(); // ✅ Get all objects
        Task<GameObject2D> GetByIdAsync(Guid id); // ✅ Get a single object by ID
        Task InsertAsync(GameObject2D obj); // ✅ Insert a new object
        Task UpdateAsync(GameObject2D obj); // ✅ Update an object
        Task DeleteAsync(Guid id); // ✅ Delete an object by ID
        Task SaveObjectToEnvironmentAsync(GameObject2D obj); // ✅ New method to save an object to an environment
    }
}