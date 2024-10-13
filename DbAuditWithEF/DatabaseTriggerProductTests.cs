using DbAuditWithEF.DatabaseTrigger;

namespace DbAuditWithEF;

public class DatabaseTriggerProductTests
{
    [Fact]
    public async Task ModifiedBy_ValueGeneratedOnUpdate_CanSetValueOnInsert()
    {
        await using var dbContext = new DbTriggerContext();
        var prod = new DbTriggerProduct()
        {
            Name = "Db Triggered - ModifiedBy On Insert",
            ModifiedBy = "SomeoneElse"
        };
        await dbContext.Products.AddAsync(prod);
        await dbContext.SaveChangesAsync();
        Assert.Equal("SomeoneElse", prod.ModifiedBy);
    }

    [Fact]
    public async Task ModifiedBy_ValueGeneratedOnAddOrUpdate_CanNotSetValueOnInsert()
    {
        await using var dbContext = new DbTriggerContext();
        var prod = new DbTriggerProduct()
        {
            Name = "Db Triggered - ModifiedBy On Insert -- Ignored",
            ModifiedByOverwritesAreIgnored = "SomeoneElse"
        };
        await dbContext.Products.AddAsync(prod);
        await dbContext.SaveChangesAsync();
        Assert.Null(prod.ModifiedByOverwritesAreIgnored);
    }



    [Fact]
    public async Task ModifiedBy_DefaultValueSql_IgnoredDoesNotUpdateTheDb()
    {
        await using var dbContext = new DbTriggerContext();
        var prod = new DbTriggerProduct()
        {
            Name = "Db Triggered - Will not be updated",
            ModifiedByOverwritesAreIgnored = "SomeoneElse"
        };
        await dbContext.Products.AddAsync(prod);
        await dbContext.SaveChangesAsync();
        Assert.Null(prod.ModifiedByOverwritesAreIgnored);

        // Ignored field: does not actually write changes to the DB!
        prod.ModifiedByOverwritesAreIgnored = "Another";
        await dbContext.SaveChangesAsync();
        Assert.Equal("Another", prod.ModifiedByOverwritesAreIgnored);
    }

    [Fact]
    public async Task ModifiedBy_DefaultValueSql_OverwritesAreIgnored()
    {
        await using var dbContext = new DbTriggerContext();
        var prod = new DbTriggerProduct()
        {
            Name = "Db Triggered - ToBeModifiedBy Ignored",
            ModifiedByOverwritesAreIgnored = "SomeoneElse"
        };
        await dbContext.Products.AddAsync(prod);
        await dbContext.SaveChangesAsync();
        Assert.Null(prod.ModifiedByOverwritesAreIgnored);

        // Also updating a non-ignored field so it does write to the DB
        prod.Name = "Db Triggered - ModifiedBy Ignored";
        prod.ModifiedByOverwritesAreIgnored = "Another";
        await dbContext.SaveChangesAsync();
        // But the ModifiedBy field is set by the trigger!
        Assert.Equal("sa", prod.ModifiedByOverwritesAreIgnored);
    }
}
