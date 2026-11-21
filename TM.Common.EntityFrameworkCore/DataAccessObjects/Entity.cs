using Microsoft.EntityFrameworkCore;
using TM.Common.Constants.Enums;

namespace TM.Common.Models.EntityFrameworkCore.DataAccessObjects
{
    public record Entity<TEntity> where TEntity : class
    {
        public Entity(DbSet<TEntity> rows, TrackType trackType = TrackType.Default)
        {
            Rows = rows ?? throw new ArgumentNullException(nameof(rows));
            Track = trackType == TrackType.IdentityResolution ? rows.AsNoTrackingWithIdentityResolution() : rows.AsNoTracking();
        }

        public DbSet<TEntity> Rows { get; private set; }
        public IQueryable<TEntity> Track { get; private set; }
    }
}
