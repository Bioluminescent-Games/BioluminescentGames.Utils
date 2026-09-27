using System;
using System.Diagnostics.CodeAnalysis;

namespace BioluminescentGames.Utils.Runtime
{
    public class CachedValue<T>
    {
        private readonly Func<T> _createFunc;

        private bool _hasValue;
        private T _value;

        /// <summary>
        /// Get the cached value if it exists, otherwise create it based on the Create Function.
        /// </summary>
        public T Value => Get();
        
        public CachedValue([NotNull] Func<T> createFunc, bool lazy = true)
        {
            _createFunc = createFunc;
            
            if (!lazy)
                CreateValue();
        }

        /// <summary>
        /// Forget the cached value so next time Get() is called it will create a new value.
        /// </summary>
        public void Forget()
        {
            _hasValue = false;
            _value = default;
        }

        /// <summary>
        /// Get the cached value if it exists, otherwise create it based on the Create Function.
        /// </summary>
        public T Get()
        {
            if (_hasValue)
                return _value;
            CreateValue();
            return _value;
        }

        /// <summary>
        /// Forget the cached value and recreate the value instantly.
        /// </summary>
        public void CreateValue()
        {
            _hasValue = true;
            _value = _createFunc();
        }
    }
}
