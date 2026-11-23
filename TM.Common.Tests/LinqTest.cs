using Microsoft.EntityFrameworkCore;
using TM.Common.Utils.Constants.Enums;
using TM.Common.Utils;

namespace TM.Common.Tests;

public class LinqTest
{
    [Fact]
    public void TakeUnmatchData_HandlesBothSourceDirections()
    {
        var left = new[] { new Item("A"), new Item("B") };
        var right = new[] { new Item("B"), new Item("C") };

        Assert.Equal(
            new[] { "A" },
            Linq.TakeUnmatchData(left, right, x => x.Code).Select(x => x.Code));

        Assert.Equal(
            new[] { "C" },
            Linq.TakeUnmatchData(left, right, x => x.Code, SourceType.Right)
                .Select(x => x.Code));
    }

    [Fact]
    public async Task TakeUnmatchDataAsync_FiltersCandidatesAgainstDatabase()
    {
        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseSqlite("Data Source=:memory:")
            .Options;

        await using var db = new TestDbContext(options);
        await db.Database.OpenConnectionAsync();
        await db.Database.EnsureCreatedAsync();

        db.Rows.Add(new Row { Code = "A" });
        await db.SaveChangesAsync();

        var candidates = new[]
        {
            new Row { Code = "A" },
            new Row { Code = "B" }
        };

        var result = await Linq.TakeUnmatchDataAsync(
            candidates, db.Rows, row => row.Code);

        Assert.Equal(new[] { "B" }, result.Select(row => row.Code));
    }

    private sealed record Item(string Code);

    public sealed class Row
    {
        public int Id { get; set; }
        public string Code { get; set; } = "";
    }

    public sealed class TestDbContext(DbContextOptions<TestDbContext> options)
        : DbContext(options)
    {
        public DbSet<Row> Rows => Set<Row>();
    }
}