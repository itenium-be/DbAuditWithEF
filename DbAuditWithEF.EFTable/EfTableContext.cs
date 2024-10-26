using DbAuditWithEF.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Text.Json;

namespace DbAuditWithEF.EFTable;

public class EfTableContext(IUserProvider userProvider) : DbContext
{
    public DbSet<EfTableProduct> Products { get; set; }
    public DbSet<EfAudit> Audit { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(ConnectionStringBuilder.Get("EfTable"));
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<EfAudit>()
            .Property(p => p.ActionType)
            .HasConversion<string>();
    }

    public override int SaveChanges()
    {
        ChangedEntity[] changes = GetChanges();
        int rowsAffected = base.SaveChanges();
        SaveAuditRecords(changes);
        return rowsAffected;
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new())
    {
        ChangedEntity[] changes = GetChanges();
        int rowsAffected = await base.SaveChangesAsync(cancellationToken);
        SaveAuditRecords(changes);
        return rowsAffected;
    }

    private ChangedEntity[] GetChanges()
    {
        return ChangeTracker.Entries<IId>()
            .Where(e => e.State is EntityState.Added or EntityState.Modified or EntityState.Deleted)
            .Select(e => new ChangedEntity(e))
            .ToArray();
    }

    private void SaveAuditRecords(ChangedEntity[] changes)
    {
        foreach (ChangedEntity change in changes)
        {
            var audit = new EfAudit()
            {
                ModifiedBy = userProvider.UserName,
                ActionType = change.State,
                ModifiedOn = DateTime.Now,
                TableId = change.EntityEntry.Entity.Id,
                TableName = change.EntityEntry.Metadata.GetTableName() ?? change.EntityEntry.Entity.GetType().Name,
            };

            switch (change.State)
            {
                case EntityState.Added:
                    audit.NewValues = JsonSerializer.Serialize((object)change.EntityEntry.Entity);
                    break;

                case EntityState.Deleted:
                    audit.OldValues = JsonSerializer.Serialize(change.EntityEntry.OriginalValues.ToObject());
                    break;

                case EntityState.Modified:
                    audit.OldValues = change.Modifications.oldValues;
                    audit.NewValues = change.Modifications.newValues;
                    break;
            }

            Audit.Add(audit);
        }

        base.SaveChanges();
    }

    private class ChangedEntity
    {
        public EntityEntry<IId> EntityEntry { get; }
        public EntityState State { get; }
        public (string oldValues, string newValues) Modifications { get; }

        public ChangedEntity(EntityEntry<IId> entityEntry)
        {
            EntityEntry = entityEntry;
            State = entityEntry.State;

            if (State == EntityState.Modified)
            {
                string oldValues = GetValues(entityEntry, true);
                string newValues = GetValues(entityEntry, false);
                Modifications = (oldValues, newValues);
            }
        }

        private static string GetValues(EntityEntry entry, bool originalValues)
        {
            var properties = entry.Properties
                .Where(p => p.IsModified)
                .ToDictionary(p => p.Metadata.Name, p => originalValues ? p.OriginalValue : p.CurrentValue);
            return JsonSerializer.Serialize(properties);
        }
    }
}

/// <summary>
/// For EF Migrations
/// </summary>
public class ByReflectionContextFactory : IDesignTimeDbContextFactory<EfTableContext>
{
    public EfTableContext CreateDbContext(string[] args)
    {
        return new EfTableContext(new CronJobUserProvider());
    }
}
