using ObjectEnvironmentPlacer.Objects;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ObjectEnvironmentPlacer.Interface
{
    public interface IPlayerEnvironmentRepository
    {
        Task AddPlayerToEnvironment(string playerId, Guid environmentId);
        Task RemovePlayerFromEnvironment(string playerId, Guid environmentId);
        Task<List<ApplicationUser>> GetPlayersInEnvironment(Guid environmentId);

        Task<List<Guid>> GetEnvironmentIdsByUserIdAsync(string userId);



    }
}
