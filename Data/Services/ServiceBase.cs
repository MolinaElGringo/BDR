using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using System.Threading.Tasks.Dataflow;
using Dapper;
using Microsoft.AspNetCore.Mvc.TagHelpers.Cache;
using Npgsql;

namespace BlazinRoleGame.Datads
{
    public class ServiceBase<Titem> : ISupportCRUD<Titem> where Titem : class
    {
        private readonly string _connectionString;
        public ServiceBase(IConfiguration configuration)
        {
            _connectionString = configuration["DefaultConectionString"] ?? throw new ArgumentNullException(nameof(configuration), "Configuration cannot be null");
        }

        /// <summary>
        /// Add a new entity to the database
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<List<Titem>> AddAsync(Titem entity)
        {

            var type = entity.GetType();
            var className = type.Name;
            var primaryKeyProperty = type.GetProperties().FirstOrDefault(p => p.GetCustomAttributes(typeof(KeyAttribute), false).Any());

            var propNameValue = type.GetProperties()
                .Where(prop => prop != primaryKeyProperty)
                .ToDictionary(prop => prop.Name, p => p.GetValue(entity));

            var properties = string.Join(", ", propNameValue.Keys.Select(propName => $"\"{propName}\""));
            var values = string.Join(", ", propNameValue.Keys.Select(propName => $"@{propName}"));


            if (primaryKeyProperty == null)
            {
                throw new Exception("No primary key found");
            }

            var query = $"INSERT INTO public.\"{className}\" ({properties}) values ({values}) returning \"{primaryKeyProperty.Name}\";";
            return await ExecuteDbOperation(async conn =>
            {
                int result = await conn.ExecuteScalarAsync<int>(query, param: propNameValue);
                entity.GetType().GetProperty(primaryKeyProperty.Name).SetValue(entity, result);
                return new List<Titem> {entity};
            });
        }

        /// <summary>
        /// Delete an entity from the database
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<List<Titem>> DeleteAsync(Titem entity)
        {
            var type = entity.GetType();
            var className = type.Name;
            var primaryKeyProperty = type.GetProperties().FirstOrDefault(p => p.GetCustomAttributes(typeof(KeyAttribute), false).Any());

            if (primaryKeyProperty == null)
                throw new Exception("No primary key found");

            var query = $"DELETE FROM public.\"{className}\" WHERE \"{primaryKeyProperty.Name}\" = {primaryKeyProperty.GetValue(entity)};";

            return await ExecuteDbOperation(async conn =>
            {
                var rowsAffected = await conn.ExecuteAsync(query, param: primaryKeyProperty);
                return rowsAffected > 0 ? new List<Titem> { entity } : null;
            });
        }

        /// <summary>
        /// Get all entities from the database
        /// </summary>
        /// <returns></returns>
        public async Task<List<Titem>> GetAllAsync()
        {
            var query = $"SELECT * FROM public.\"{typeof(Titem).Name}\";";
            return await ExecuteDbOperation(async conn =>
            {
                var list = await conn.QueryAsync<Titem>(query);
                return list.ToList();
            });
        }

        /// <summary>
        /// Get an entity by its primary key
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<List<Titem>> GetById(Titem entity)
        {
            var type = entity.GetType();
            var className = type.Name;

            var primaryKeyProperty = type.GetProperties().FirstOrDefault(p => p.GetCustomAttributes(typeof(KeyAttribute), false).Any());

            if (primaryKeyProperty == null)
                throw new Exception("No primary key found");

            var query = "SELECT * FROM public.\"{className}\" WHERE \"{primaryKeyProperty.Name}\" = @{primaryKeyProperty.Name};";

            return await ExecuteDbOperation(async conn =>
            {
                var getEntityId = await conn.ExecuteAsync(query, param: primaryKeyProperty);
                if (getEntityId > 0)
                    return new List<Titem> { entity };
                else
                    return null;
            });
        }

        /// <summary>
        /// Update an entity in the database
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<List<Titem>> UpdateAsync(Titem entity)
        {
            var type = entity.GetType();
            var className = type.Name;
            var primaryKeyProperty = type.GetProperties().FirstOrDefault(p => p.GetCustomAttributes(typeof(KeyAttribute), false).Any());

            if (primaryKeyProperty == null)
            {
                throw new Exception("No primary key found");
            }

            var propNameValue = type.GetProperties()
                                .ToDictionary(prop => prop.Name, p => p.GetValue(entity));


            var query = $"UPDATE public.\"{className}\" SET ";

            foreach (var prop in propNameValue)
            {
                if (prop.Key == primaryKeyProperty.Name)
                    continue;

                if (prop.Key != propNameValue.Last().Key)
                    query += $"\"{prop.Key}\" = @{prop.Key}, ";
                else
                    query += $"\"{prop.Key}\" = @{prop.Key} ";
            }

            query += $"WHERE \"{primaryKeyProperty.Name}\" = @{primaryKeyProperty.Name};";

            return await ExecuteDbOperation(async conn =>
            {
                await conn.ExecuteAsync(query, param: propNameValue);
                return new List<Titem> { entity };
            });
        }

        /// <summary>
        /// Execute a database operation for a list of items
        /// </summary>
        /// <param name="operation"></param>
        /// <returns></returns>
        protected virtual async Task<List<Titem>> ExecuteDbOperation(Func<NpgsqlConnection, Task<List<Titem>>> operation)
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                try
                {
                    return await operation(conn);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    throw;
                }
                finally
                {
                    if (conn.State == System.Data.ConnectionState.Open)
                    {
                        conn.Close();
                    }
                }
            }
        }
    }

}