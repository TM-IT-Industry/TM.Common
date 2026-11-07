namespace TM.Common.Utils
{
    public sealed class Guard
    {
        public static void AgainstNull<T>(T value, string message)
        {
            if (value is null)
                throw new ArgumentNullException(message);
        }

        public static void AgainstNullOrEmpty(string value, string referenceName)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new NullReferenceException($"Reference '{referenceName}' cannot be null or empty.");
        }

        public static void ThrowInvalidOperationIf(bool condition, string message)
        {
            if (condition)
                throw new InvalidOperationException(message);
        }
    }
}
