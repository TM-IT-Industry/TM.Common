using TM.Common.Utils.Constants.Enums;

using Microsoft.EntityFrameworkCore;
namespace TM.Common.Utils
{
    public union SourceType<TEntity>(IQueryable<TEntity>, IEnumerable<TEntity>);

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

        /// <summary>
        /// Use only in c# 15 or latest
        /// Returns entities from the selected source whose keys do not appear in the other source.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entities in the source collections.</typeparam>
        /// <typeparam name="TKey">The type of the keys used to compare entities.</typeparam>
        /// <param name="leftSource">The left source of entities.</param>
        /// <param name="rightSource">The right source of entities.</param>
        /// <param name="keySelector">A function that selects a comparison key from an entity.</param>
        /// <param name="sourceType">The source to select unmatched entities from. Defaults to the left source.</param>
        /// <returns>A task representing the asynchronous operation. The task result contains entities from the selected source
        /// whose keys are absent from the other source.</returns>
        public static async Task<IList<TEntity>> TakeUnmatchDataAsync<TEntity, TKey>(
            SourceType<TEntity> leftSource,
            SourceType<TEntity> rightSource,
            Func<TEntity, TKey> keySelector,
            SourceType sourceType = SourceType.Left)
            where TEntity : class
        {
            var source = sourceType is SourceType.Left ? leftSource : rightSource;
            var target = sourceType is SourceType.Left ? rightSource : leftSource;
            var excludedKeys = await GetExcludedKeys(target, keySelector);

            return await FilterSource(source, excludedKeys, keySelector);
        }

        private static async Task<IList<TEntity>> FilterSource<TEntity, TKey>(
            SourceType<TEntity> sourceType, 
            HashSet<TKey> excludedKeys,
            Func<TEntity, TKey> keySelector)
        {
            return sourceType switch
            {
                IQueryable<TEntity> queryable => await queryable.Where(a => !excludedKeys.Contains(keySelector(a))).ToListAsync(),
                IEnumerable<TEntity> enumerable => await Task.FromResult(enumerable.Where(a => !excludedKeys.Contains(keySelector(a))).ToList()),
                _ => throw new ArgumentOutOfRangeException(nameof(sourceType))
            };
        }

        private static async Task<HashSet<TKey>> GetExcludedKeys<TEntity, TKey>(SourceType<TEntity> targetSourceType, Func<TEntity, TKey> keySelector)
        {
            return targetSourceType switch
            {
                IQueryable<TEntity> queryable => await Task.FromResult(queryable.Select(keySelector).ToHashSet<TKey>()),
                IEnumerable<TEntity> enumerable => await Task.FromResult(enumerable.Select(keySelector).ToHashSet<TKey>()),
                _ => throw new InvalidCastException("Target Source Type is unsupported for processing of excluded keys")
            };
        }
    }
}
