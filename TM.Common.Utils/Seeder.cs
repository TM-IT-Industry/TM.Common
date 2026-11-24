using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using TM.Common.Utils.Interfaces;
namespace TM.Common.Utils
{
    /// <summary>
    /// Provides a base implementation for seeding data into the database.
    /// </summary>
    /// <typeparam name="TDbContext">Database Context</typeparam>
    /// <typeparam name="TEntity">Entity or Table Type</typeparam>
    /// <typeparam name="TKey">Key Selector Type</typeparam>
    public abstract class Seeder<TDbContext, TEntity, TKey>
        : ISeeder
        where TDbContext : DbContext
        where TEntity : class
    {
        private readonly IReadOnlyCollection<TEntity> data;
        private readonly Expression<Func<TEntity, TKey>> keySelector;
        private readonly TDbContext dbContext;
        private readonly IQueryable<TEntity> existings;
        private readonly Action<TDbContext>? callBack;
        private readonly Func<TDbContext, Task>? callBackAsync;

        /// <summary>
        /// Initializes a new seeder with the entities to seed, database context, and key selector.
        /// <list type="bullet">
        ///     <item>
        ///     <description>data: The rows / records to seed.</description>
        ///     </item>
        ///     <item>
        ///         <description>dbContext: The database context used to persist the entities.</description>
        ///     </item>
        ///     <item>
        ///         <description>keySelector: The function that identifies the key of an entity.</description>
        ///     </item>
        /// </list>
        /// </summary>
        /// <param name="data">The rows / records to seed.</param>
        /// <param name="dbContext">The database context used to persist the entities.</param>
        /// <param name="keySelector">The function that identifies the key of an entity.</param>
        protected Seeder(
            IEnumerable<TEntity> data, 
            TDbContext dbContext, 
            Expression<Func<TEntity, TKey>> keySelector,
            Action<TDbContext>? callBack = null,
            Func<TDbContext, Task>? callBackAsync = null)
        {
            Guard.AgainstNull(data, "Seeder data is Empty");
            Guard.AgainstNull(dbContext, "Seeder Database Context is Empty");
            Guard.AgainstNull(keySelector, "Seeder Key Selector is Empty");

            this.data = data.ToArray();
            this.dbContext = dbContext;
            this.keySelector = keySelector;
            this.callBack = callBack;
            this.callBackAsync = callBackAsync;

            existings = dbContext.Set<TEntity>();
        }

        /// <summary>
        /// Seed without async
        /// </summary>
        public void Seed()
        {
            var _keySelector = keySelector.Compile();
            var unmatchData = data.ExceptByKey(existings, _keySelector);
            if (!unmatchData.Any()) return;

            if (callBack is not null)
            {
                callBack.Invoke(dbContext);
                return;
            }

            dbContext.AddRange(unmatchData);
        }

        /// <summary>
        /// Seed the database with asynchronous
        /// </summary>
        /// <returns></returns>
        public async Task SeedAsync(CancellationToken cancellationToken = default)
        {
            var unmatchData = await data.ExceptByKeyAsync(existings, keySelector, cancellationToken: cancellationToken);
            if (!unmatchData.Any()) return;

            if (callBackAsync is not null)
            {
                await callBackAsync.Invoke(dbContext);
                return;
            }

            await dbContext.AddRangeAsync(unmatchData, cancellationToken);
        }

        public virtual void Run(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Seed();
            dbContext.SaveChanges();
        }

        public virtual async Task RunAsync(CancellationToken cancellationToken)
        {
            await SeedAsync(cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
