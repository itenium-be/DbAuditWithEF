using DbAuditWithEF.Utils;
using Microsoft.EntityFrameworkCore;

namespace DbAuditWithEF.DatabaseTable;

public class DbTableContext : DbContext
{
    public DbSet<DbTableProduct> Products { get; set; }
    public DbSet<DbAudit> Audit { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(ConnectionStringBuilder.Get("DatabaseTable"));
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DbTableProduct>()
            .ToTable(tb => tb.UseSqlOutputClause(false));

        modelBuilder.Entity<DbAudit>()
            .Property(p => p.ModifiedOn)
            .HasDefaultValueSql("GETDATE()");

        modelBuilder.Entity<DbAudit>()
            .Property(p => p.ModifiedBy)
            .HasDefaultValueSql("SYSTEM_USER");

        modelBuilder.Entity<DbAudit>()
            .Property(p => p.NewValues)
            .HasColumnType("xml");

        modelBuilder.Entity<DbAudit>()
            .Property(p => p.OldValues)
            .HasColumnType("xml");

        modelBuilder.Entity<DbAudit>()
            .Property(p => p.ActionType)
            .HasConversion<string>();
    }
}
