// <copyright file="Extensions.cs" company="Teqniqly">
// Copyright (c) Teqniqly
// </copyright>

namespace SmartWineRack.Commands
{
    /// <summary>
    /// Extension methods.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Ensures the object is not null by throwing an exception if it is.
        /// </summary>
        /// <param name="o">The object to test for null.</param>
        /// <param name="paramName">The parameter name that appears in the exception message.</param>
        /// <exception cref="ArgumentNullException">The exception thrown if the object is null.</exception>
        public static void EnsureNotNull(this object? o, string paramName)
        {
            if (o == null)
            {
                throw new ArgumentNullException(paramName);
            }
        }

        /// <summary>
        /// Ensures the given key exists in the dictionary by throwing an exception if the key does not exist.
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="key">The key to test.</param>
        /// <exception cref="InvalidOperationException">The exception thrown if the key does not exist.</exception>
        public static void EnsureKey(this IDictionary<string, object> dictionary, string key)
        {
            if (!dictionary.ContainsKey(key))
            {
                throw new InvalidOperationException($"Required key missing - {key}.");
            }
        }
    }
}
