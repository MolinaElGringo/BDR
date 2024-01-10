using Microsoft.EntityFrameworkCore;

namespace SharedLibrary.Repository
{
    /// <summary>
    /// RepositoryEF is an implementation of IRepository interface that uses Entity Framework.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity that this repository works with.</typeparam>
    /// <typeparam name="TDataContext">The type of the DbContext that this repository uses.</typeparam>
    public class RepositoryEF<TEntity, TDataContext> : IRepository<TEntity>
        where TEntity : class
        where TDataContext : DbContext
    {
        protected readonly TDataContext context;
        protected DbSet<TEntity> dbSet;

        public RepositoryEF(TDataContext dataContext)
        {
            context = dataContext;
            context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            dbSet = context.Set<TEntity>();
        }

        /// <summary>
        /// Deletes the specified entity from the repository.
        /// </summary>
        /// <param name="entityToDelete">Entity to delete.</param>
        /// <returns>True if the operation was successful; otherwise, false.</returns>
        public virtual async Task<bool> Delete(TEntity entityToDelete)
        {
            if (context.Entry(entityToDelete).State == EntityState.Detached)
            {
                dbSet.Attach(entityToDelete);
            }
            dbSet.Remove(entityToDelete);
            return await context.SaveChangesAsync() >= 1;
        }

        /// <summary>
        /// Deletes an entity in the repository using its ID.
        /// </summary>
        /// <param name="id">ID of the entity to delete.</param>
        /// <returns>True if the operation was successful; otherwise, false.</returns>
        public virtual async Task<bool> Delete(object id)
        {
            TEntity? entityToDelete = await dbSet.FindAsync(id);
            if (entityToDelete is null) return false;
            return await Delete(entityToDelete);
        }

        /// <summary>
        /// Get method for retrieving entities according to a QueryFilter.
        /// </summary>
        /// <param name="filter">A function to test each element for a condition; only elements that pass the test are included in the returned collection.</param>
        /// <param name="orderBy">A function to order elements.</param>
        /// <param name="includeProperties">Properties to be included in the query result.</param>
        /// <returns>An IEnumerable of entities.</returns>
        public virtual async Task<IEnumerable<TEntity>> Get(QueryFilter<TEntity> queryFilter)
        {
            return queryFilter.GetFilteredList(dbSet);

        }

        /// <summary>
        /// Generic get method for retrieving entities.
        /// </summary>
        /// <param name="queryLinq"></param>
        /// <returns></returns>
        public virtual async Task<IEnumerable<TEntity>> Get(LinqQueryFilter<TEntity> linqQueryFilter)
        {
            return linqQueryFilter.GetFilteredList(dbSet);
        }

        /// <summary>
        /// Retrieves all entities from the repository.
        /// </summary>
        /// <returns>An IEnumerable of entities.</returns>
        public virtual async Task<IEnumerable<TEntity>> GetAll()
        {
            if (typeof(TEntity).Name.Contains("Search"))
            {
                // Use reflection to check if a corresponding EntitySearch function exists
                var methodName = typeof(TEntity).Name;
                var searchFunction = typeof(TDataContext).GetMethod(methodName);
                if (searchFunction != null)
                {
                    // If it exists, invoke it with null parameters (or adjust as needed)
                    var result = searchFunction.Invoke(context, new object[] { });

                    // Ensure result is IQueryable<TEntity>
                    if (result is IQueryable<TEntity> queryableResult)
                    {
                        return await queryableResult.ToListAsync();
                    }
                }
            }
            return context.Set<TEntity>().ToList();
        }

        /// <summary>
        /// Gets an entity using its ID.
        /// </summary>
        /// <param name="id">ID of the entity to retrieve.</param>
        /// <returns>The entity if found; otherwise, null.</returns>
        public virtual async Task<TEntity?> GetByID(object id) => dbSet.Find(id);

        /// <summary>
        /// Inserts a new entity into the repository.
        /// </summary>
        /// <param name="entity">Entity to insert.</param>
        /// <returns>The inserted entity.</returns>
        public virtual async Task<TEntity?> Insert(TEntity entity)
        {
            await dbSet.AddAsync(entity);
            await context.SaveChangesAsync();
            return entity;
        }

        /// <summary>
        /// Updates an existing entity in the repository.
        /// </summary>
        /// <param name="entityToUpdate">Entity to update.</param>
        /// <returns>The updated entity.</returns>
        public virtual async Task<TEntity?> Update(TEntity entityToUpdate)
        {
            var dbSet = context.Set<TEntity>();
            dbSet.Attach(entityToUpdate);
            context.Entry(entityToUpdate).State = EntityState.Modified;
            await context.SaveChangesAsync();
            return entityToUpdate;
        }
    }
}


