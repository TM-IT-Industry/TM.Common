using TM.Common.Utils.Constants.Enums;

namespace TM.Common.Utils
{
    public static class Linq
    {
        public static IEnumerable<TEntity> TakeUnmatchData<TEntity, TKey>(
            IEnumerable<TEntity> leftSource, 
            IEnumerable<TEntity> rightSource, 
            Func<TEntity, TKey> keySelector,
            SourceType sourceType = SourceType.Left) 
            where TEntity : class
        {
            var source = sourceType is SourceType.Left ? leftSource : rightSource;
            var target = sourceType is SourceType.Left ? rightSource : leftSource;

            var excludedKeys = new HashSet<TKey>(target.Select(keySelector));

            return source.Where(a => !excludedKeys.Contains(keySelector(a)));
        }
    }
}
