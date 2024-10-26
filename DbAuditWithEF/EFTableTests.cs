using DbAuditWithEF.DatabaseTable;
using DbAuditWithEF.EFTable;
using DbAuditWithEF.Utils;
using Microsoft.EntityFrameworkCore;

namespace DbAuditWithEF;

public class EFTableTests
{
    [Fact]
    public async Task CreatesAuditInsertRecord()
    {
        await using var dbContext = new EfTableContext(new CronJobUserProvider());
        var prod = new EfTableProduct()
        {
            Name = "Name",
        };
        await dbContext.Products.AddAsync(prod);
        await dbContext.SaveChangesAsync();

        var audit = await dbContext.Audit.OrderBy(x => x.Id).LastAsync();
        Assert.Equal("Products", audit.TableName);
        Assert.Equal(prod.Id, audit.TableId);
        Assert.Null(audit.OldValues);
        Assert.Equal("CronJob", audit.ModifiedBy);
        Assert.Equal(EntityState.Added, audit.ActionType);
        Assert.Equal($"{{\"Id\":{prod.Id},\"Name\":\"Name\"}}", audit.NewValues);
    }

    [Fact]
    public async Task CreatesAuditUpdateRecord()
    {
        await using var dbContext = new EfTableContext(new CronJobUserProvider());
        var prod = new EfTableProduct()
        {
            Name = "Old Name",
        };
        await dbContext.Products.AddAsync(prod);
        await dbContext.SaveChangesAsync();

        prod.Name = "New Name";
        await dbContext.SaveChangesAsync();

        var audit = await dbContext.Audit.OrderBy(x => x.Id).LastAsync();
        Assert.Equal("Products", audit.TableName);
        Assert.Equal(prod.Id, audit.TableId);
        Assert.Equal("CronJob", audit.ModifiedBy);
        Assert.Equal(EntityState.Modified, audit.ActionType);
        Assert.Equal("{\"Name\":\"Old Name\"}", audit.OldValues);
        Assert.Equal("{\"Name\":\"New Name\"}", audit.NewValues);
    }

    [Fact]
    public async Task CreatesAuditDeleteRecord()
    {
        await using var dbContext = new EfTableContext(new CronJobUserProvider());
        var prod = new EfTableProduct()
        {
            Name = "To delete",
        };
        await dbContext.Products.AddAsync(prod);
        await dbContext.SaveChangesAsync();

        dbContext.Products.Remove(prod);
        await dbContext.SaveChangesAsync();

        var audit = await dbContext.Audit.OrderBy(x => x.Id).LastAsync();
        Assert.Equal("Products", audit.TableName);
        Assert.Equal(prod.Id, audit.TableId);
        Assert.Equal("CronJob", audit.ModifiedBy);
        Assert.Equal(EntityState.Deleted, audit.ActionType);
        Assert.Equal($"{{\"Id\":{prod.Id},\"Name\":\"To delete\"}}", audit.OldValues);
        Assert.Null(audit.NewValues);
    }
}
