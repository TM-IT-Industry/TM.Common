using System;
using System.Collections.Generic;
using System.Text;

namespace TM.Common.Utils
{
    public sealed class Guard
    {
        public static void AgainstNull<T>(T value, string message)
        {
            if (value is null)
                throw new ArgumentNullException(message);
        }

        public static void AgainstNullOrEmpty(string value, string parameterName)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException($"Parameter '{parameterName}' cannot be null or empty.", parameterName);
        }
    }
}
