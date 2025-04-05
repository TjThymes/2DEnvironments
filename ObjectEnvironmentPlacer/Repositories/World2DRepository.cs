    using ObjectEnvironmentPlacer.Objects;
    using Dapper;
    using Microsoft.Data.SqlClient;
    using System;
    using System.Data;
    using System.Threading.Tasks;
using ObjectEnvironmentPlacer.Interface;

namespace ObjectEnvironmentPlacer.Repositories
    {
        public class Environment2DRepository : IEnvironment2DRepository
        {
            private readonly IDbConnection _dbConnection;

            public Environment2DRepository(string connectionString)
            {
                _dbConnection = new SqlConnection(connectionString);
            }

            public async Task<Environment2D> InsertAsync(string name, string? description)
            {
                if (string.IsNullOrWhiteSpace(name))
                {
                    throw new ArgumentException("Environment Name cannot be empty.");
                }

                var newId = Guid.NewGuid(); 

                string sql = @"INSERT INTO Environment2D (ID, Name, Description) VALUES (@ID, @Name, @Description)";
                await _dbConnection.ExecuteAsync(sql, new { ID = newId, Name = name, Description = description });

                return new Environment2D { ID = newId, Name = name, Description = description };
            }
            public async Task<IEnumerable<Environment2D>> GetAllAsync()
            {
                string sql = "SELECT * FROM Environment2D";
                return await _dbConnection.QueryAsync<Environment2D>(sql);
            }
            public async Task DeleteAsync(Guid id)
            {
                string sql = "DELETE FROM Environment2D WHERE ID = @ID";
                await _dbConnection.ExecuteAsync(sql, new { ID = id });
            }
            public async Task<Environment2D> GetByIdAsync(Guid id)
            {
                string sql = "SELECT * FROM Environment2D WHERE ID = @ID";
                return await _dbConnection.QuerySingleOrDefaultAsync<Environment2D>(sql, new { ID = id });
            }
            public async Task<Environment2D?> GetByIdWithObjectsAsync(Guid id)
            {
                string sql = @"
            SELECT e.*, g.*
            FROM Environment2D e
            LEFT JOIN Objects2D g ON e.ID = g.EnvironmentID
            WHERE e.ID = @ID";

                var environmentDictionary = new Dictionary<Guid, Environment2D>();

                var result = await _dbConnection.QueryAsync<Environment2D, GameObject2D, Environment2D>(
                    sql,
                    (env, obj) =>
                    {
                        if (!environmentDictionary.TryGetValue(env.ID, out var currentEnv))
                        {
                            currentEnv = env;
                            currentEnv.Objects = new List<GameObject2D>();
                            environmentDictionary.Add(env.ID, currentEnv);
                        }

                        if (obj != null)
                        {
                            currentEnv.Objects.Add(obj);
                        }

                        return currentEnv;
                    },
                    new { ID = id },
                    splitOn: "ID"
                );

                return environmentDictionary.Values.FirstOrDefault();
            }
        }
    }
