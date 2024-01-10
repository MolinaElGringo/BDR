namespace SharedLibrary.Repository
{
    /// <summary>
    /// The IRepository interface provides the standard operations to be performed on a data repository for a given type.
    /// </summary>
    /// <typeparam name="TEntity">Type of entity that this repository works with.</typeparam>
    public interface IRepository<TEntity> where TEntity : class
    {
        #region Methods

        /// <summary>
        /// Deletes the specified entity from the repository.
        /// </summary>
        /// <param name="entityToDelete">Entity to delete.</param>
        /// <returns>True if the operation was successful; otherwise, false.</returns>
        Task<bool> Delete(TEntity entityToDelete);

        /// <summary>
        /// Deletes an entity in the repository using its ID.
        /// </summary>
        /// <param name="id">ID of the entity to delete.</param>
        /// <returns>True if the operation was successful; otherwise, false.</returns>
        Task<bool> Delete(object id);

        /// <summary>
        /// Gets all entities from the repository.
        /// </summary>
        /// <returns>An IEnumerable of entities.</returns>
        Task<IEnumerable<TEntity>> GetAll();

        /// <summary>
        /// Gets an entity using its ID.
        /// </summary>
        /// <param name="id">ID of the entity to retrieve.</param>
        /// <returns>The entity if found; otherwise, null.</returns>
        Task<TEntity?> GetByID(object id);

        /// <summary>
        /// Gets entities based on a query filter.
        /// </summary>
        /// <param name="queryFilter">The query filter to use</param>
        /// <returns>An IEnumerable of filtered entities.</returns>
        Task<IEnumerable<TEntity>> Get(QueryFilter<TEntity> queryFilter);

        /// <summary>
        /// Gets entities based on a Expresion
        /// </summary>
        /// <param name="queryLinq">The expression to filter the entities</param>
        /// <returns>An IEnumerable of filtered entities./returns>
        Task<IEnumerable<TEntity>> Get(LinqQueryFilter<TEntity> linqQueryFilter);


        /// <summary>
        /// Inserts a new entity into the repository.
        /// </summary>
        /// <param name="entity">Entity to insert.</param>
        /// <returns>The inserted entity.</returns>
        Task<TEntity?> Insert(TEntity entity);

        /// <summary>
        /// Updates an existing entity in the repository.
        /// </summary>
        /// <param name="entityToUpdate">Entity to update.</param>
        /// <returns>The updated entity.</returns>
        Task<TEntity?> Update(TEntity entityToUpdate);

        #endregion
    }
}
