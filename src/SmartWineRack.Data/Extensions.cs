using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartWineRack.Data
{
    public static class Utilities
    {
        public static T EnsureNonNullGet<T>(T value, string paramName)
        {
            if (value == null)
            {
                throw new ArgumentNullException($"{paramName} cannot be null.");
            }

            return value;
        }
    }
}
