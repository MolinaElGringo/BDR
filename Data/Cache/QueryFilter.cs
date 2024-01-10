using Microsoft.EntityFrameworkCore;
using System.Collections;
using System.Linq.Expressions;
using System.Reflection;
#nullable disable
namespace SharedLibrary.Repository
{

    /// <summary>
    /// A serializable filter. An alternative to trying to serialize and deserialize LINQ expressions,
    /// which are very finicky. This class uses standard types. 
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class QueryFilter<TEntity> where TEntity : class
    {
        #region Constructor

        /// <summary>
        /// Default ctor.
        /// </summary>
        public QueryFilter() { }

        #endregion

        #region Properties

        /// <summary>
        /// If you want to return a subset of the properties, you can specify only
        /// the properties that you want to retrieve in the SELECT clause.
        /// Leave empty to return all columns
        /// </summary>
        public List<string> IncludePropertyNames { get; set; } = new List<string>();

        /// <summary>
        /// Defines the property names and values in the WHERE clause
        /// </summary>
        public List<FilterProperty> FilterProperties { get; set; } = new List<FilterProperty>();

        /// <summary>
        /// Specify the property to ORDER BY, if any 
        /// </summary>
        public string OrderByPropertyName { get; set; } = "";

        /// <summary>
        /// Set to true if you want to order DESCENDING
        /// </summary>
        public bool OrderByDescending { get; set; } = false;

        public List<Expression<Func<TEntity, bool>>> CustomPredicates { get; set; } = new List<Expression<Func<TEntity, bool>>>();

        #endregion

        private static readonly Dictionary<FilterOperator, Func<object, object, bool>> ComparisonFunctions = new Dictionary<FilterOperator, Func<object, object, bool>>
        {
            { FilterOperator.Equals, (a, b) => a.Equals(b) },
            { FilterOperator.NotEquals, (a, b) => !a.Equals(b) },
            { FilterOperator.LessThan, (a, b) => Comparer.Default.Compare(a, b) < 0 },
            { FilterOperator.GreaterThan,(a, b) => Comparer.Default.Compare(a, b) > 0},
            { FilterOperator.LessThanOrEqual, (a, b) => Comparer.Default.Compare(a, b) < 0 },
            { FilterOperator.GreaterThanOrEqual, (a, b) => Comparer.Default.Compare(a, b) >= 0 },

        };

        public void AddCustomPredicate(Expression<Func<TEntity, bool>> predicate) => CustomPredicates.Add(predicate);


        /// <summary>
        /// A custome query that returns a list of entities with the current filter settings.
        /// </summary>
        /// <param name="allItems"></param>
        /// <returns></returns>
        public IEnumerable<TEntity> GetFilteredList(IEnumerable<TEntity> allItems) => FilterList(allItems.AsQueryable());
        public IEnumerable<TEntity> GetFilteredList(IQueryable<TEntity> allItems) => FilterList(allItems);

        private IEnumerable<TEntity> FilterList(IQueryable<TEntity> query)
        {
            foreach (var filterProperty in FilterProperties)
                query = ProcessProperty(query, filterProperty);

            foreach (var customPredicate in CustomPredicates)
                query = query.Where(customPredicate);

            query = IncludeProperties(query);
            query = ApplyOrdering(query);

            return query.ToList();
        }

        private IQueryable<TEntity> ApplyOrdering(IQueryable<TEntity> query)
        {
            if (!string.IsNullOrEmpty(OrderByPropertyName))
            {
                var propInfo = typeof(TEntity).GetProperty(OrderByPropertyName);
                if (propInfo != null)
                {
                    var param = Expression.Parameter(typeof(TEntity), "x");
                    var expression = Expression.Lambda<Func<TEntity, object>>(
                        Expression.Convert(Expression.Property(param, propInfo), typeof(object)), param);

                    query = OrderByDescending ? query.OrderByDescending(expression) : query.OrderBy(expression);
                }
            }

            return query;
        }

        private IQueryable<TEntity> IncludeProperties(IQueryable<TEntity> query)
        {
            foreach (var includeProperty in IncludePropertyNames)
                query = query.Include(includeProperty);

            return query;
        }

        private IQueryable<TEntity> ProcessProperty(IQueryable<TEntity> query, FilterProperty filterProperty)
        {
            var parameter = Expression.Parameter(typeof(TEntity), "entity");
            Expression condition = null;

            var property = typeof(TEntity).GetProperty(filterProperty.Name);
            if (property == null)
            {
                throw new ArgumentException($"Property {filterProperty.Name} not found in {typeof(TEntity).Name}");
            }

            var propertyAccess = Expression.Property(parameter, property);
            var targetValue = Expression.Constant(Convert.ChangeType(filterProperty.Value, property.PropertyType), property.PropertyType);

            switch (filterProperty.Operator)
            {
                case FilterOperator.Equals:
                    condition = Expression.Equal(propertyAccess, targetValue);
                    break;
                case FilterOperator.NotEquals:
                    condition = Expression.NotEqual(propertyAccess, targetValue);
                    break;
                case FilterOperator.LessThan:
                    condition = Expression.LessThan(propertyAccess, targetValue);
                    break;
                case FilterOperator.GreaterThan:
                    condition = Expression.GreaterThan(propertyAccess, targetValue);
                    break;
                case FilterOperator.LessThanOrEqual:
                    condition = Expression.LessThanOrEqual(propertyAccess, targetValue);
                    break;
                case FilterOperator.GreaterThanOrEqual:
                    condition = Expression.GreaterThanOrEqual(propertyAccess, targetValue);
                    break;
            }

            // Additional string-specific operations
            if (property.PropertyType == typeof(string))
            {
                var stringValue = Expression.Constant(filterProperty.Value.ToString());
                switch (filterProperty.Operator)
                {
                    case FilterOperator.StartsWith:
                        condition = Expression.Call(propertyAccess, typeof(string).GetMethod("StartsWith", new Type[] { typeof(string) }), stringValue);
                        break;
                    case FilterOperator.EndsWith:
                        condition = Expression.Call(propertyAccess, typeof(string).GetMethod("EndsWith", new Type[] { typeof(string) }), stringValue);
                        break;
                    case FilterOperator.Contains:
                        condition = Expression.Call(propertyAccess, typeof(string).GetMethod("Contains", new Type[] { typeof(string) }), stringValue);
                        break;
                }
            }

            if (condition is not null)
            {
                var lambda = Expression.Lambda<Func<TEntity, bool>>(condition, parameter);
                query = query.Where(lambda);
            }

            return query;
        }

    }
}