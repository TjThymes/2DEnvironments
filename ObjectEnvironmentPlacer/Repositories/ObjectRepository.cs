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
    public class ObjectRepository : IObjectRepository
    {
        private readonly IDbConnection _dbConnection;

        public ObjectRepository(string sqlConnectionString)
        {
            _dbConnection = new SqlConnection(sqlConnectionString);
        }

        public async Task<IEnumerable<GameObject2D>> GetAllAsync()
        {
            string sql = "SELECT * FROM Objects2D"; // ✅ Correct table name
            return await _dbConnection.QueryAsync<GameObject2D>(sql);
        }

        public async Task<GameObject2D> GetByIdAsync(Guid id)
        {
            string sql = "SELECT * FROM Objects2D WHERE ID = @ID"; // ✅ Correct table name
            return await _dbConnection.QuerySingleOrDefaultAsync<GameObject2D>(sql, new { ID = id });
        }

        public async Task InsertAsync(GameObject2D obj)
        {
            string sql = @"INSERT INTO Objects2D (ID, EnvironmentID, PrefabID, PositionX, PositionY, ScaleX, ScaleY, RotationZ, SortingLayer) 
                           VALUES (@ID, @EnvironmentID, @PrefabID, @PositionX, @PositionY, @ScaleX, @ScaleY, @RotationZ, @SortingLayer)";

            obj.ID = Guid.NewGuid();
            await _dbConnection.ExecuteAsync(sql, obj);
        }

        public async Task UpdateAsync(GameObject2D obj)
        {
            string sql = @"UPDATE Objects2D SET 
                           EnvironmentID = @EnvironmentID, 
                           PrefabID = @PrefabID, 
                           PositionX = @PositionX, 
                           PositionY = @PositionY, 
                           ScaleX = @ScaleX, 
                           ScaleY = @ScaleY, 
                           RotationZ = @RotationZ, 
                           SortingLayer = @SortingLayer
                           WHERE ID = @ID";

            await _dbConnection.ExecuteAsync(sql, obj);
        }

        public async Task DeleteAsync(Guid id)
        {
            string sql = "DELETE FROM Objects2D WHERE ID = @ID"; // ✅ Correct table name
            await _dbConnection.ExecuteAsync(sql, new { ID = id });
        }
        public async Task SaveObjectToEnvironmentAsync(GameObject2D obj)
        {
            string sql = @"INSERT INTO Objects2D (ID, EnvironmentID, PrefabID, PositionX, PositionY, ScaleX, ScaleY, RotationZ, SortingLayer) 
                   VALUES (@ID, @EnvironmentID, @PrefabID, @PositionX, @PositionY, @ScaleX, @ScaleY, @RotationZ, @SortingLayer)";

            await _dbConnection.ExecuteAsync(sql, obj);
        }

    }
}
