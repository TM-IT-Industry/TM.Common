using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace TM.Common.Utils;

public static class LinqExtensions
{
    /// <summary>
    /// Returns elements from <paramref name="source"/> whose selected keys
    /// are not present in <paramref name="existing"/>.
    /// </summary>
    /// <remarks>
    /// The result preserves the order and duplicates of
    /// <paramref name="source"/>.
    /// </remarks>
    public static IEnumerable<TEntity> ExceptByKey<TEntity, TKey>(
        this IEnumerable<TEntity> source,
        IEnumerable<TEntity> existing,
        Func<TEntity, TKey> keySelector,
        IEqualityComparer<TKey>? comparer = null)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(existing);
        ArgumentNullException.ThrowIfNull(keySelector);

        var existingKeys = existing
            .Select(keySelector)
            .ToHashSet(comparer);

        return source.Where(item => !existingKeys.Contains(keySelector(item)));
    }

    /// <summary>
    /// Asynchronously returns candidates whose selected keys are not present
    /// in the existing database query.
    /// </summary>
    /// <remarks>
    /// Candidate order and duplicates are preserved.
    ///
    /// Existing keys are retrieved in batches to avoid executing one query
    /// per candidate and to prevent excessively large SQL IN clauses.
    /// </remarks>
    public static async Task<IReadOnlyList<TEntity>> ExceptByKeyAsync<TEntity, TKey>(
        this IEnumerable<TEntity> candidates,
        IQueryable<TEntity> existing,
        Expression<Func<TEntity, TKey>> keySelector,
        int batchSize = 1_000,
        IEqualityComparer<TKey>? comparer = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(candidates);
        ArgumentNullException.ThrowIfNull(existing);
        ArgumentNullException.ThrowIfNull(keySelector);

        ArgumentOutOfRangeException.ThrowIfLessThan(batchSize, 1);

        var items = candidates as TEntity[] ?? candidates.ToArray();

        if (items.Length == 0)
        {
            return Array.Empty<TEntity>();
        }

        var getKey = keySelector.Compile();

        var candidateKeys = items
            .Select(getKey)
            .Distinct(comparer)
            .ToArray();

        var existingKeys = new HashSet<TKey>(comparer);

        foreach (var keyBatch in candidateKeys.Chunk(batchSize))
        {
            cancellationToken.ThrowIfCancellationRequested();

            var predicate = BuildContainsPredicate(
                keySelector,
                keyBatch);

            var matchedKeys = await existing
                .Where(predicate)
                .Select(keySelector)
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);

            existingKeys.UnionWith(matchedKeys);
        }

        return items
            .Where(item => !existingKeys.Contains(getKey(item)))
            .ToList();
    }

    private static Expression<Func<TEntity, bool>> BuildContainsPredicate<TEntity, TKey>(Expression<Func<TEntity, TKey>> keySelector, TKey[] keys)
    {
        var containsExpression = Expression.Call(
            typeof(Enumerable),
            nameof(Enumerable.Contains),
            [typeof(TKey)],
            Expression.Constant(keys),
            keySelector.Body);

        return Expression.Lambda<Func<TEntity, bool>>(
            containsExpression,
            keySelector.Parameters);
    }
}