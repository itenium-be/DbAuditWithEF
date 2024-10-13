using DbAuditWithEF.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace DbAuditWithEF.ByEF;

public class ByEFContext(IUserProvider userProvider) : DbContext
{
    public DbSet<ByEFProduct> Products { get; set; }
    public DbSet<ByEFClient> Clients { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(ConnectionStringBuilder.Get("ByEF"));
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
        // Configure a single entity:
        //modelBuilder.Entity<ByEFProduct>()
        //    .OwnsOne(e => e.Audit, audit =>
        //    {
        //        // Default column name would be Audit_CreatedOn etc
        //        audit.Property(a => a.CreatedOn).HasColumnName(nameof(Audit.CreatedOn));
        //        audit.Property(a => a.CreatedBy).HasColumnName(nameof(Audit.CreatedBy));
        //        audit.Property(a => a.ModifiedOn).HasColumnName(nameof(Audit.ModifiedOn));
        //        audit.Property(a => a.ModifiedBy).HasColumnName(nameof(Audit.ModifiedBy));
        //    });


        // Configure all entities:
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

    private void SetAuditFields()
    {
        var entries = ChangeTracker
            .Entries<IAudit>()
            .Where(e => e.State is EntityState.Added or EntityState.Modified);

        foreach (var entityEntry in entries)
        {
            Audit audit = entityEntry.Entity.Audit;
            if (entityEntry.State == EntityState.Added)
            {
                audit.CreatedOn = DateTime.Now;
                audit.CreatedBy = userProvider.UserName;
            }
            else
            {
                audit.ModifiedOn = DateTime.Now;
                audit.ModifiedBy = userProvider.UserName;
            }
        }
    }
}

/// <summary>
/// For EF Migrations
/// 
/// Alternatively you can add a parameterless ctor for migrations
/// EF Migrations can handle a ctor with a DbContextOptionsBuilder parameter
/// </summary>
public class ByEFContextFactory : IDesignTimeDbContextFactory<ByEFContext>
{
    public ByEFContext CreateDbContext(string[] args)
    {
        return new ByEFContext(new CronJobUserProvider());
    }
}
