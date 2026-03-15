using DbAuditWithEF.ByInterceptor;
using DbAuditWithEF.Utils;

namespace DbAuditWithEF;

public class ByInterceptorTests
{
    [Fact]
    public async Task CreatedBy_IsSet()
    {
        await using var dbContext = new ByInterceptorContext(new CronJobUserProvider());
        var prod = new ByInterceptorProduct
        {
            Name = "By Interceptor",
        };
        await dbContext.Products.AddAsync(prod);
        await dbContext.SaveChangesAsync();
        Assert.Equal("CronJob", prod.Audit.CreatedBy);
    }

    [Fact]
    public async Task CreatedOn_IsSet()
    {
        await using var dbContext = new ByInterceptorContext(new CronJobUserProvider());
        var before = DateTime.Now;
        var prod = new ByInterceptorProduct { Name = "By Interceptor" };
        await dbContext.Products.AddAsync(prod);
        await dbContext.SaveChangesAsync();
        var after = DateTime.Now;

        Assert.InRange(prod.Audit.CreatedOn, before, after);
    }

    [Fact]
    public async Task ModifiedBy_Works()
    {
        await using var dbContext = new ByInterceptorContext(new CronJobUserProvider());
        var prod = new ByInterceptorProduct { Name = "By Interceptor" };
        await dbContext.Products.AddAsync(prod);
        await dbContext.SaveChangesAsync();
        Assert.Equal("CronJob", prod.Audit.CreatedBy);
        Assert.Null(prod.Audit.ModifiedBy);

        prod.Name = "Test";
        await dbContext.SaveChangesAsync();
        Assert.Equal("CronJob", prod.Audit.ModifiedBy);
    }

    [Fact]
    public async Task ModifiedOn_IsSet()
    {
        await using var dbContext = new ByInterceptorContext(new CronJobUserProvider());
        var prod = new ByInterceptorProduct { Name = "By Interceptor" };
        await dbContext.Products.AddAsync(prod);
        await dbContext.SaveChangesAsync();
        Assert.Null(prod.Audit.ModifiedOn);

        var before = DateTime.Now;
        prod.Name = "Test";
        await dbContext.SaveChangesAsync();
        var after = DateTime.Now;

        Assert.NotNull(prod.Audit.ModifiedOn);
        Assert.InRange(prod.Audit.ModifiedOn.Value, before, after);
    }

    [Fact]
    public void SyncSaveChanges_SetsAuditFields()
    {
        using var dbContext = new ByInterceptorContext(new CronJobUserProvider());
        var prod = new ByInterceptorProduct { Name = "By Interceptor" };
        dbContext.Products.Add(prod);
        dbContext.SaveChanges();

        Assert.Equal("CronJob", prod.Audit.CreatedBy);
        Assert.NotEqual(default, prod.Audit.CreatedOn);
    }

    [Fact]
    public async Task CreatedOn_NotOverwrittenOnUpdate()
    {
        await using var dbContext = new ByInterceptorContext(new CronJobUserProvider());
        var prod = new ByInterceptorProduct { Name = "By Interceptor" };
        await dbContext.Products.AddAsync(prod);
        await dbContext.SaveChangesAsync();
        var originalCreatedOn = prod.Audit.CreatedOn;
        var originalCreatedBy = prod.Audit.CreatedBy;

        prod.Name = "Updated";
        await dbContext.SaveChangesAsync();

        Assert.Equal(originalCreatedOn, prod.Audit.CreatedOn);
        Assert.Equal(originalCreatedBy, prod.Audit.CreatedBy);
    }
}
