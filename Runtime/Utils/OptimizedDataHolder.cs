namespace BioluminescentGames.Utils
{
    /// <summary>
    /// Sets a boolean value based on if the value is null.
    /// Should only be used if the value can be null, and works best, if you are getting the value more than you are setting the value.
    /// </summary>
    /// <typeparam name="T">The type of the value, has to be nullable</typeparam>

    public class OptimizedDataHolder<T> where T : class, new()
    {
        /// <summary>
        /// The value of the holder
        /// </summary>
        public T Value
        {
            get => Get();
            set => Set(value);
        }

        /// <summary>
        /// True if the Value is null
        /// </summary>
        public bool ValueIsNull { get; private set; }

        /// <summary>
        /// True if the value isn't null
        /// </summary>
        public bool ValueIsNotNull { get; private set; }

        private T value;

        /// <summary>
        /// Less expensive way of setting the value to null.
        /// </summary>
        public void DeleteValue()
        {
            value = null;
            ValueIsNull = true;
            ValueIsNotNull = false;
        }

        /// <summary>
        /// Set the value
        /// </summary>
        /// <param name="value">The value to set it to</param>
        public void Set(T value)
        {
            Set(value, value == null);
        }

        /// <summary>
        /// Sets the value if you already know if it is null
        /// </summary>
        /// <param name="value">The value to set it to</param>
        /// <param name="isNull">If the value is null</param>
        public void Set(T value, bool isNull)
        {
            this.value = value;
            ValueIsNull = isNull;
            ValueIsNotNull = !ValueIsNull;
        }

        /// <summary>
        /// Get the value
        /// </summary>
        /// <returns>The value the holder contains</returns>
        public T Get()
        {
            return value;
        }

        /// <summary>
        /// Easy way to make an OptimizedDataHolder with a value
        /// </summary>
        /// <param name="value">Starting value</param>
        public OptimizedDataHolder(T value)
        {
            Set(value);
        }

        /// <summary>
        /// Create an OptimizedDataHolder without a starting value
        /// </summary>
        public OptimizedDataHolder()
        {
            DeleteValue();
        }

        /// <summary>
        /// Gets the value or creates the default object.
        /// </summary>
        /// <returns>The value</returns>
        public T GetOrCreate()
        {
            if (ValueIsNull)
                Set(new T(), false);
            return Value;
        }

        /// <summary>
        /// Creates the object with the default constructor
        /// </summary>
        /// <returns>The created object</returns>
        public T CreateDefault()
        {
            Set(new T(), false);
            return Value;
        }
    }
}