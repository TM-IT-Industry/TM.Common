using Microsoft.EntityFrameworkCore;
using TM.Common.EntityFrameworkCore.Enums;

namespace TM.Common.Models.DataAccessObjects
{
    internal record Entity<TEntity> where TEntity : class
    {
        internal Entity(DbSet<TEntity> rows, TrackType trackType = TrackType.Default)
        {
            Rows = rows ?? throw new ArgumentNullException(nameof(rows));
            Track = trackType == TrackType.IdentityResolution ? rows.AsNoTrackingWithIdentityResolution() : rows.AsNoTracking();
        }

        internal DbSet<TEntity> Rows { get; private set; }
        internal IQueryable<TEntity> Track { get; private set; }
    }
}
