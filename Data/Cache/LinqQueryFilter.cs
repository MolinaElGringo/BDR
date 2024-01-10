using Microsoft.EntityFrameworkCore;
using Serialize.Linq.Serializers;
using System.Linq.Expressions;
using System.Reflection;
#nullable disable

namespace SharedLibrary.Repository
{
    /// <summary>
    /// Represents a LINQ query filter for entities of type TEntity. Allows for filtering,
    /// including specific properties, and ordering the results.
    /// </summary>
    /// <typeparam name="TEntity">The entity type to apply the filter to.</typeparam>
    public class LinqQueryFilter<TEntity> where TEntity : class
    {
        #region Constructors

        /// <summary>
        /// Default constructor for a new instance of the LinqQueryFilter class.
        /// </summary>
        public LinqQueryFilter() { }

        /// <summary>
        /// Initializes a new instance of the LinqQueryFilter class with a specific filter expression.
        /// </summary>
        /// <param name="expression">The LINQ expression for filtering.</param>
        public LinqQueryFilter(Expression<Func<TEntity, bool>> expression)
        {
            var serializer = new ExpressionSerializer(new JsonSerializer());
            LinqExpression = serializer.SerializeText(expression);
        }

        /// <summary>
        /// Initializes a new instance of the LinqQueryFilter class with a filter expression and a list of properties to include.
        /// </summary>
        /// <param name="expression">The LINQ expression for filtering.</param>
        /// <param name="includeProperties">List of property names to include in the query.</param>
        public LinqQueryFilter(Expression<Func<TEntity, bool>> expression, List<string> includeProperties)
        {
            var serializer = new ExpressionSerializer(new JsonSerializer());
            LinqExpression = serializer.SerializeText(expression);

            IncludePropertyNames = includeProperties;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the linq expression for filtering.
        /// </summary>
        public string LinqExpression { get; set; }

        /// <summary>
        /// If you want to return a subset of the properties, you can specify only
        /// the properties that you want to retrieve in the SELECT clause.
        /// Leave empty to return all columns
        /// </summary>
        public List<string> IncludePropertyNames { get; set; } = new List<string>();

        /// <summary>
        /// Specify the property to ORDER BY, if any 
        /// </summary>
        public string OrderByPropertyName { get; set; } = "";

        /// <summary>
        /// Set to true if you want to order DESCENDING
        /// </summary>
        public bool OrderByDescending { get; set; } = false;

        #endregion

        /// <summary>
        /// Applies the filter to the given queryable collection and returns the filtered list.
        /// </summary>
        /// <param name="query">The queryable collection to apply the filter to.</param>
        /// <returns>A collection of entities of type TEntity that match the filter criteria.</returns>
        public IEnumerable<TEntity> GetFilteredList(IQueryable<TEntity> query)
        {
            var serializer = new ExpressionSerializer(new JsonSerializer());
            var deserializedExpression = serializer.DeserializeText(LinqExpression) as Expression<Func<TEntity, bool>>;

            query = query.Where(deserializedExpression);

            foreach (var prop in IncludePropertyNames)
            {
                query = query.Include(prop);
            }

            if (OrderByPropertyName != "")
            {
                PropertyInfo prop = typeof(TEntity).GetProperty(OrderByPropertyName);
                if (prop != null)
                {
                    if (OrderByDescending)
                        query = query.OrderByDescending(x => prop.GetValue(x, null));
                    else
                        query = query.OrderBy(x => prop.GetValue(x, null));
                }
            }

            // executes and return the list
            var result =  query.ToList();
            return result;

        }
    }
}
