using Dapper;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using ObjectEnvironmentPlacer.Objects;
using ObjectEnvironmentPlacer.Interface;

namespace ObjectEnvironmentPlacer.Repositories
{
    public class PlayerEnvironmentRepository : IPlayerEnvironmentRepository
    {
        private readonly IDbConnection _dbConnection;

        public PlayerEnvironmentRepository(string connectionString)
        {
            _dbConnection = new SqlConnection(connectionString);
        }

        public async Task AddPlayerToEnvironment(string playerId, Guid environmentId) // ✅ PlayerID is now a string
        {
            string sql = "INSERT INTO PlayerEnvironment (PlayerID, EnvironmentID) VALUES (@PlayerID, @EnvironmentID)";
            await _dbConnection.ExecuteAsync(sql, new { PlayerID = playerId, EnvironmentID = environmentId });
        }

        public async Task RemovePlayerFromEnvironment(string playerId, Guid environmentId)
        {
            string sql = "DELETE FROM PlayerEnvironment WHERE PlayerID = @PlayerID AND EnvironmentID = @EnvironmentID";
            await _dbConnection.ExecuteAsync(sql, new { PlayerID = playerId, EnvironmentID = environmentId });
        }

        public async Task<List<ApplicationUser>> GetPlayersInEnvironment(Guid environmentId)
        {
            string sql = @"
                SELECT u.* FROM AspNetUsers u
                JOIN PlayerEnvironment pe ON u.Id = pe.PlayerID
                WHERE pe.EnvironmentID = @EnvironmentID";

            return (await _dbConnection.QueryAsync<ApplicationUser>(sql, new { EnvironmentID = environmentId })).AsList();
        }


        public async Task<List<Guid>> GetEnvironmentIdsByUserIdAsync(string userId)
        {
            string sql = @"
            SELECT pe.EnvironmentID 
            FROM PlayerEnvironment pe
            WHERE pe.PlayerID = @UserID";

            return (await _dbConnection.QueryAsync<Guid>(sql, new { UserID = userId })).AsList();
        }
    }
}
