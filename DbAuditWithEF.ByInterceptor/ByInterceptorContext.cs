using DbAuditWithEF.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace DbAuditWithEF.ByInterceptor;

public class ByInterceptorContext : DbContext
{
    private readonly AuditInterceptor _auditInterceptor;

    public ByInterceptorContext(IUserProvider userProvider)
    {
        _auditInterceptor = new AuditInterceptor(userProvider);
    }

    public DbSet<ByInterceptorProduct> Products { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder
            .UseSqlServer(ConnectionStringBuilder.Get("ByInterceptor"))
            .AddInterceptors(_auditInterceptor);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var entities = modelBuilder.Model.GetEntityTypes()
            .Where(entityType => typeof(IAudit).IsAssignableFrom(entityType.ClrType))
            .ToArray();

        foreach (var entityType in entities)
        {
            modelBuilder.Entity(entityType.ClrType)
                .OwnsOne(typeof(Audit), nameof(IAudit.Audit), audit =>
                {
                    audit.Property(nameof(Audit.CreatedOn)).HasColumnName(nameof(Audit.CreatedOn));
                    audit.Property(nameof(Audit.CreatedBy)).HasColumnName(nameof(Audit.CreatedBy));
                    audit.Property(nameof(Audit.ModifiedOn)).HasColumnName(nameof(Audit.ModifiedOn));
                    audit.Property(nameof(Audit.ModifiedBy)).HasColumnName(nameof(Audit.ModifiedBy));
                });
        }
    }
}

public class ByInterceptorContextFactory : IDesignTimeDbContextFactory<ByInterceptorContext>
{
    public ByInterceptorContext CreateDbContext(string[] args)
    {
        return new ByInterceptorContext(new CronJobUserProvider());
    }
}
