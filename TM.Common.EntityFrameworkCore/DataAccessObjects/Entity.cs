using Microsoft.EntityFrameworkCore;
using TM.Common.Constants.Enums;

namespace TM.Common.Models.EntityFrameworkCore.DataAccessObjects
{
    /// <summary>
    /// Provides access to an entity's DbSet and preconfigured no-tracking query source.
    /// Intended for use within the repository layer.
    /// </summary>
    /// <typeparam name="TEntity">The entity type managed by Entity Framework Core.</typeparam>
    public record Entity<TEntity> where TEntity : class
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Entity{TEntity}"/> class.
        /// </summary>
        /// <param name="rows">
        /// The original DbSet used for tracked queries and database modifications.
        /// </param>
        /// <param name="trackType">
        /// Specifies the tracking behavior used when configuring the untracked query source.
        /// Defaults to <see cref="TrackType.Default"/>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="rows"/> is null.
        /// </exception>
        public Entity(
            DbSet<TEntity> rows,
            TrackType trackType = TrackType.Default)
        {
            Source = rows ?? throw new ArgumentNullException(nameof(rows));

            Untracked = trackType == TrackType.IdentityResolution
                ? rows.AsNoTrackingWithIdentityResolution()
                : rows.AsNoTracking();
        }

        /// <summary>
        /// Gets the original <see cref="DbSet{TEntity}"/>.
        /// Use this property for tracked queries and database operations
        /// such as adding, updating, and removing entities.
        /// </summary>
        public DbSet<TEntity> Source { get; private set; }

        /// <summary>
        /// Gets a preconfigured no-tracking query source.
        /// Use this property for read-only operations where change tracking
        /// is not required.
        /// </summary>
        /// <remarks>
        /// Uses <see cref="EntityFrameworkQueryableExtensions.AsNoTracking{TEntity}(IQueryable{TEntity})"/>
        /// by default, or <see cref="EntityFrameworkQueryableExtensions.AsNoTrackingWithIdentityResolution{TEntity}(IQueryable{TEntity})"/>
        /// when <see cref="TrackType.IdentityResolution"/> is specified.
        /// </remarks>
        public IQueryable<TEntity> Untracked { get; private set; }
    }
}
