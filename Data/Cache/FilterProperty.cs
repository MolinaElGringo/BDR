namespace SharedLibrary.Repository
{
    /// <summary>
    /// Defines a property for the WHERE clause
    /// </summary>
    public class FilterProperty
    {
        /// <summary>
        /// The name of the property
        /// </summary>
        public string Name { get; set; } = "";

        /// <summary>
        /// The value of the property
        /// </summary>
        public string Value { get; set; } = "";

        /// <summary>
        /// Defines how to compare the column values (StartsWith, Contains, Equals, etc.)
        /// </summary>
        public FilterOperator Operator { get; set; }

        /// <summary>
        /// If true, the comparison will be case sensitive
        /// </summary>
        public bool CaseSensitive { get; set; } = false;
    }
}