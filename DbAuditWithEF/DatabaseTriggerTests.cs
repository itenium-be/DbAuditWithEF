using DbAuditWithEF.DatabaseTrigger;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace DbAuditWithEF;

public class DatabaseTriggerTests
{
    [Fact]
    public async Task CreatedBy_IsSetByDefaultValueSql()
    {
        await using var dbContext = new DbTriggerContext();
        var prod = new DbTriggerProduct()
        {
            Name = "Db Triggered",
        };

        EntityEntry<DbTriggerProduct> added = await dbContext.Products.AddAsync(prod);
        Assert.Null(added.Entity.CreatedBy);
        Assert.Null(prod.CreatedBy);
        Assert.Same(added.Entity, prod);

        await dbContext.SaveChangesAsync();
        Assert.Equal("sa", added.Entity.CreatedBy);
        Assert.Equal("sa", prod.CreatedBy);

        DbTriggerProduct inserted = await dbContext.Products.OrderBy(x => x.Id).LastAsync(x => x.Name == "Db Triggered");
        Assert.Equal("sa", inserted.CreatedBy);
        Assert.Same(prod, inserted);
    }

    [Fact]
    public async Task CreatedBy_DefaultValueSql_CanBeOverwritten()
    {
        await using var dbContext = new DbTriggerContext();
        var prod = new DbTriggerProduct()
        {
            Name = "Db Triggered - Overwritten",
            CreatedBy = "SomeoneElse"
        };
        await dbContext.Products.AddAsync(prod);
        await dbContext.SaveChangesAsync();

        DbTriggerProduct inserted = await dbContext.Products.OrderBy(x => x.Id).LastAsync(x => x.Name == "Db Triggered - Overwritten");
        Assert.Equal("SomeoneElse", inserted.CreatedBy);
    }
}
