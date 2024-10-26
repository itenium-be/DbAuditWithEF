using DbAuditWithEF.DatabaseTable;
using Microsoft.EntityFrameworkCore;

namespace DbAuditWithEF;

public class DatabaseTableTests
{
    [Fact]
    public async Task Trigger_CreatesAuditInsertRecord()
    {
        await using var dbContext = new DbTableContext();
        var prod = new DbTableProduct()
        {
            Name = "Name",
        };
        await dbContext.Products.AddAsync(prod);
        await dbContext.SaveChangesAsync();
        Assert.Null(prod.ModifiedBy);

        var audit = await dbContext.Audit.OrderBy(x => x.Id).LastAsync();
        Assert.Equal("Products", audit.TableName);
        Assert.Equal(prod.Id.ToString(), audit.TableIds);
        Assert.Null(audit.OldValues);
        Assert.Equal("sa", audit.ModifiedBy);
        Assert.Equal(ActionType.I, audit.ActionType);
        Assert.Equal($"<row><Id>{prod.Id}</Id><Name>Name</Name></row>", audit.NewValues);
    }

    [Fact]
    public async Task Trigger_CreatesAuditUpdateRecord()
    {
        await using var dbContext = new DbTableContext();
        var prod = new DbTableProduct()
        {
            Name = "Old Name",
        };
        await dbContext.Products.AddAsync(prod);
        await dbContext.SaveChangesAsync();

        prod.Name = "New Name";
        await dbContext.SaveChangesAsync();

        var audit = await dbContext.Audit.OrderBy(x => x.Id).LastAsync();
        Assert.Equal("Products", audit.TableName);
        Assert.Equal(prod.Id.ToString(), audit.TableIds);
        Assert.Equal("sa", audit.ModifiedBy);
        Assert.Equal(ActionType.U, audit.ActionType);
        Assert.Equal($"<row><Id>{prod.Id}</Id><Name>Old Name</Name></row>", audit.OldValues);
        Assert.Equal($"<row><Id>{prod.Id}</Id><Name>New Name</Name></row>", audit.NewValues);
    }

    [Fact]
    public async Task Trigger_CreatesAuditDeleteRecord()
    {
        await using var dbContext = new DbTableContext();
        var prod = new DbTableProduct()
        {
            Name = "To delete",
        };
        await dbContext.Products.AddAsync(prod);
        await dbContext.SaveChangesAsync();

        dbContext.Products.Remove(prod);
        await dbContext.SaveChangesAsync();

        var audit = await dbContext.Audit.OrderBy(x => x.Id).LastAsync();
        Assert.Equal("Products", audit.TableName);
        Assert.Equal(prod.Id.ToString(), audit.TableIds);
        Assert.Equal("sa", audit.ModifiedBy);
        Assert.Equal(ActionType.D, audit.ActionType);
        Assert.Equal($"<row><Id>{prod.Id}</Id><Name>To delete</Name></row>", audit.OldValues);
        Assert.Null(audit.NewValues);
    }

    [Fact]
    public async Task Trigger_UpdateStatement_CreatesSingleAuditRecord()
    {
        await using var dbContext = new DbTableContext();
        var prod1 = new DbTableProduct() { Name = "Name1" };
        var prod2 = new DbTableProduct() { Name = "Name2" };
        await dbContext.Products.AddAsync(prod1);
        await dbContext.Products.AddAsync(prod2);
        await dbContext.SaveChangesAsync();

        await dbContext.Database.ExecuteSqlAsync($"UPDATE Products SET Name=Name+' updated' WHERE Id In ({prod1.Id}, {prod2.Id})");

        var audit = await dbContext.Audit.OrderBy(x => x.Id).LastAsync();
        Assert.Equal("Products", audit.TableName);
        Assert.Equal($"{prod1.Id},{prod2.Id}", audit.TableIds);
        Assert.Equal(ActionType.U, audit.ActionType);
    }
}
