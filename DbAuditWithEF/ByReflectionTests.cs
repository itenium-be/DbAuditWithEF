using DbAuditWithEF.ByReflection;
using DbAuditWithEF.Utils;

namespace DbAuditWithEF;

public class ByReflectionTests
{
    [Fact]
    public async Task CreatedBy_IsSet()
    {
        await using var dbContext = new ByReflectionContext(new CronJobUserProvider());
        var prod = new ByReflectionProduct
        {
            Name = "By Reflection",
        };
        await dbContext.Products.AddAsync(prod);
        await dbContext.SaveChangesAsync();
        Assert.Equal("CronJob", prod.Audit.CreatedBy);
    }

    [Fact]
    public async Task ModifiedBy_Works()
    {
        await using var dbContext = new ByReflectionContext(new CronJobUserProvider());
        var client = new ByReflectionProduct()
        {
            Name = "By Reflection",
        };
        await dbContext.Products.AddAsync(client);
        await dbContext.SaveChangesAsync();
        Assert.Equal("CronJob", client.Audit.CreatedBy);
        Assert.Null(client.Audit.ModifiedBy);

        client.Name = "Test";
        await dbContext.SaveChangesAsync();
        Assert.Equal("CronJob", client.Audit.ModifiedBy);
    }
}
