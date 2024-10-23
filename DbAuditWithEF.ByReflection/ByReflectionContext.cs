using System.Reflection;
using DbAuditWithEF.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace DbAuditWithEF.ByReflection;

public class ByReflectionContext(IUserProvider userProvider) : DbContext
{
    public DbSet<ByReflectionProduct> Products { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(ConnectionStringBuilder.Get("ByReflection"));
    }

    public override int SaveChanges()
    {
        SetAuditFields();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new())
    {
        SetAuditFields();
        return base.SaveChangesAsync(cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configure all entities:
        var entities = modelBuilder.Model.GetEntityTypes()
            .Where(entityType => typeof(IAudit).IsAssignableFrom(entityType.ClrType))
            .ToArray();

        foreach (var entityType in entities)
        {
            modelBuilder.Entity(entityType.ClrType)
                .OwnsOne(typeof(Audit), nameof(IAudit.Audit), audit =>
                {
                    audit.Property(nameof(Audit.CreatedOn)).HasColumnName(nameof(Audit.CreatedOn)).HasField("_createdOn");
                    audit.Property(nameof(Audit.CreatedBy)).HasColumnName(nameof(Audit.CreatedBy)).HasField("_createdBy");
                    audit.Property(nameof(Audit.ModifiedOn)).HasColumnName(nameof(Audit.ModifiedOn)).HasField("_modifiedOn");
                    audit.Property(nameof(Audit.ModifiedBy)).HasColumnName(nameof(Audit.ModifiedBy)).HasField("_modifiedBy");
                });
        }
    }

    private void SetAuditFields()
    {
        var entries = ChangeTracker
            .Entries<IAudit>()
            .Where(e => e.State is EntityState.Added or EntityState.Modified);

        foreach (var entityEntry in entries)
        {
            Audit audit = entityEntry.Entity.Audit;
            // PERFORMANCE: you may want to put the .GetField() calls into static readonly fields!
            Type auditType = audit.GetType();
            if (entityEntry.State == EntityState.Added)
            {
                auditType
                    .GetField("_createdOn", BindingFlags.NonPublic | BindingFlags.Instance)!
                    .SetValue(audit, DateTime.Now);
                auditType
                    .GetField("_createdBy", BindingFlags.NonPublic | BindingFlags.Instance)!
                    .SetValue(audit, userProvider.UserName);
            }
            else
            {
                auditType
                    .GetField("_modifiedOn", BindingFlags.NonPublic | BindingFlags.Instance)!
                    .SetValue(audit, DateTime.Now);
                auditType
                    .GetField("_modifiedBy", BindingFlags.NonPublic | BindingFlags.Instance)!
                    .SetValue(audit, userProvider.UserName);
            }
        }
    }
}


/// <summary>
/// For EF Migrations
/// </summary>
public class ByReflectionContextFactory : IDesignTimeDbContextFactory<ByReflectionContext>
{
    public ByReflectionContext CreateDbContext(string[] args)
    {
        return new ByReflectionContext(new CronJobUserProvider());
    }
}
