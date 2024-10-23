using DbAuditWithEF.ByEF;

namespace DbAuditWithEF;

public class ByEFTests
{
    [Fact]
    public async Task CreatedBy_IsSet()
    {
        await using var dbContext = new ByEFContext(new CronJobUserProvider());
        var prod = new ByEFProduct
        {
            Name = "By EF",
            Audit =
            {
                CreatedBy = "SomeoneElse"
            }
        };
        await dbContext.Products.AddAsync(prod);
        await dbContext.SaveChangesAsync();
        Assert.Equal("CronJob", prod.Audit.CreatedBy);
    }

    [Fact]
    public async Task GlobalAuditConfiguration_Works()
    {
        await using var dbContext = new ByEFContext(new CronJobUserProvider());
        var client = new ByEFClient
        {
            Name = "By EF",
        };
        await dbContext.Clients.AddAsync(client);
        await dbContext.SaveChangesAsync();
        Assert.Equal("CronJob", client.Audit.CreatedBy);
    }

    [Fact]
    public async Task GlobalAuditConfiguration_Updates_Works()
    {
        await using var dbContext = new ByEFContext(new CronJobUserProvider());
        var client = new ByEFClient
        {
            Name = "By EF",
        };
        await dbContext.Clients.AddAsync(client);
        await dbContext.SaveChangesAsync();
        Assert.Equal("CronJob", client.Audit.CreatedBy);
        Assert.Null(client.Audit.ModifiedBy);

        client.Name = "Test";
        await dbContext.SaveChangesAsync();
        Assert.Equal("CronJob", client.Audit.ModifiedBy);
    }

    [Fact]
    public async Task AuditFields_CannotBeOverwritten()
    {
        await using var dbContext = new ByEFContext(new CronJobUserProvider());
        var client = new ByEFClient
        {
            Name = "By EF",
            Audit =
            {
                CreatedBy = "ByWhom",
                ModifiedBy = "ModBy"
            }
        };
        await dbContext.Clients.AddAsync(client);
        await dbContext.SaveChangesAsync();
        Assert.Equal("CronJob", client.Audit.CreatedBy);
        Assert.Null(client.Audit.ModifiedBy);

        client.Name = "Test";
        client.Audit.CreatedBy = "ChangedAdd";
        await dbContext.SaveChangesAsync();
        Assert.Equal("CronJob", client.Audit.ModifiedBy);
        Assert.Equal("CronJob", client.Audit.CreatedBy);
    }
}
