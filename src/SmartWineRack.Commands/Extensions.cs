// <copyright file="Extensions.cs" company="Teqniqly">
// Copyright (c) Teqniqly
// </copyright>

namespace SmartWineRack.Commands
{
    public static class Extensions
    {
        public static void EnsureNotNull(this object? o, string paramName)
        {
            if (o == null)
            {
                throw new ArgumentNullException(paramName);
            }
        }

        public static void EnsureKey(this IDictionary<string, object> dictionary, string key)
        {
            if (!dictionary.ContainsKey(key))
            {
                throw new InvalidOperationException($"Required key missing - {key}.");
            }
        }
    }
}
