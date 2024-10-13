using DbAuditWithEF.DatabaseTrigger;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace DbAuditWithEF;

public class DatabaseTriggerFaultyProductTests
{
    [Fact]
    public async Task CreatedBy_IsSetByDefaultValueSql()
    {
        await using var dbContext = new DbTriggerContext();
        var prod = new DbTriggerFaultyProduct()
        {
            Name = "Db Triggered",
        };

        EntityEntry<DbTriggerFaultyProduct> added = await dbContext.FaultyProducts.AddAsync(prod);
        Assert.Null(added.Entity.CreatedBy);
        Assert.Null(prod.CreatedBy);
        Assert.Same(added.Entity, prod);

        await dbContext.SaveChangesAsync();
        Assert.Equal("sa", added.Entity.CreatedBy);
        Assert.Equal("sa", prod.CreatedBy);

        DbTriggerFaultyProduct inserted = await dbContext.FaultyProducts.OrderBy(x => x.Id).LastAsync(x => x.Name == "Db Triggered");
        Assert.Equal("sa", inserted.CreatedBy);
        Assert.Same(prod, inserted);
    }

    [Fact]
    public async Task CreatedBy_DefaultValueSql_CanBeOverwritten()
    {
        await using var dbContext = new DbTriggerContext();
        var prod = new DbTriggerFaultyProduct()
        {
            Name = "Db Triggered - Overwritten",
            CreatedBy = "SomeoneElse"
        };
        await dbContext.FaultyProducts.AddAsync(prod);
        await dbContext.SaveChangesAsync();

        DbTriggerFaultyProduct inserted = await dbContext.FaultyProducts.OrderBy(x => x.Id).LastAsync(x => x.Name == "Db Triggered - Overwritten");
        Assert.Equal("SomeoneElse", inserted.CreatedBy);
    }

    [Fact]
    public async Task CreatedBy_DefaultValueSql_OverwritesAreIgnored()
    {
        await using var dbContext = new DbTriggerContext();
        var prod = new DbTriggerFaultyProduct()
        {
            Name = "Db Triggered - Ignored",
            CreatedByOverwritesAreIgnored = "SomeoneElse"
        };
        await dbContext.FaultyProducts.AddAsync(prod);
        Assert.Equal("SomeoneElse", prod.CreatedByOverwritesAreIgnored);

        await dbContext.SaveChangesAsync();
        Assert.Equal("sa", prod.CreatedByOverwritesAreIgnored);

        DbTriggerFaultyProduct inserted = await dbContext.FaultyProducts.OrderBy(x => x.Id).LastAsync(x => x.Name == "Db Triggered - Ignored");
        Assert.Equal("sa", inserted.CreatedByOverwritesAreIgnored);
    }

    [Fact]
    public async Task CreatedBy_DefaultValueSql_OverwritesThrow()
    {
        await using var dbContext = new DbTriggerContext();
        var prod = new DbTriggerFaultyProduct()
        {
            Name = "Db Triggered - Ignored",
            CreatedByOverwritesThrow = "SomeoneElse"
        };
        await dbContext.FaultyProducts.AddAsync(prod);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => dbContext.SaveChangesAsync());
        const string msg = "The property 'DbTriggerFaultyProduct.CreatedByOverwritesThrow' is defined as read-only before "
            + "it has been saved, but its value has been set to something other than a temporary or default value.";
        Assert.Equal(msg, ex.Message);
    }

    [Fact]
    public async Task ModifiedBy_DefaultValueSql_WithOnUpdate_CanBeSetDuringInsert()
    {
        await using (var dbContext = new DbTriggerContext())
        {
            var prod = new DbTriggerFaultyProduct()
            {
                Name = "Db Triggered - ToBeModified",
                ModifiedBy = "NotNull"
            };
            await dbContext.FaultyProducts.AddAsync(prod);
            await dbContext.SaveChangesAsync();
            Assert.Equal("NotNull", prod.ModifiedBy);
        }

        await using (var dbContext = new DbTriggerContext())
        {
            var inserted = await dbContext.FaultyProducts.OrderBy(x => x.Id).LastAsync(x => x.Name == "Db Triggered - ToBeModified");
            inserted.Name = "Db Triggered - Modified";
            var ex = await Assert.ThrowsAsync<DbUpdateException>(() => dbContext.SaveChangesAsync());
            Assert.StartsWith("Could not save changes because the target table has database triggers. Please configure your table accordingly", ex.Message);
        }
    }
}
