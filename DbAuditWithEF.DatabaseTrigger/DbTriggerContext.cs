using Microsoft.EntityFrameworkCore;
using DbAuditWithEF.Utils;
using Microsoft.EntityFrameworkCore.Metadata;

namespace DbAuditWithEF.DatabaseTrigger;

public class DbTriggerContext : DbContext
{
    public DbSet<DbTriggerFaultyProduct> FaultyProducts { get; set; }
    public DbSet<DbTriggerProduct> Products { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(ConnectionStringBuilder.Get("DatabaseTrigger"));
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ConfigureFaultyProducts(modelBuilder);
        ConfigureProducts(modelBuilder);
    }

    private static void ConfigureProducts(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DbTriggerProduct>()
            .ToTable(tb => tb.UseSqlOutputClause(false));

        modelBuilder.Entity<DbTriggerProduct>()
            .Property(e => e.CreatedOn)
            .HasDefaultValueSql("GETDATE()")
            .ValueGeneratedOnAdd()
            .Metadata.SetBeforeSaveBehavior(PropertySaveBehavior.Ignore);

        modelBuilder.Entity<DbTriggerProduct>()
            .Property(e => e.CreatedBy)
            .HasMaxLength(100)
            .HasDefaultValueSql("SYSTEM_USER")
            .ValueGeneratedOnAdd()
            .Metadata.SetBeforeSaveBehavior(PropertySaveBehavior.Ignore);


        modelBuilder.Entity<DbTriggerProduct>()
            .Property(e => e.ModifiedOn)
            .ValueGeneratedOnUpdate()
            .Metadata.SetBeforeSaveBehavior(PropertySaveBehavior.Ignore);

        modelBuilder.Entity<DbTriggerProduct>()
            .Property(e => e.ModifiedBy)
            .HasMaxLength(100)
            .ValueGeneratedOnUpdate()
            .Metadata.SetBeforeSaveBehavior(PropertySaveBehavior.Ignore);

        modelBuilder.Entity<DbTriggerProduct>()
            .Property(e => e.ModifiedByOverwritesAreIgnored)
            .HasMaxLength(100)
            .ValueGeneratedOnAddOrUpdate()
            .Metadata.SetBeforeSaveBehavior(PropertySaveBehavior.Ignore);
    }

    private static void ConfigureFaultyProducts(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DbTriggerFaultyProduct>()
            .Property(e => e.CreatedOn)
            .HasDefaultValueSql("GETDATE()")
            .ValueGeneratedOnAdd();

        modelBuilder.Entity<DbTriggerFaultyProduct>()
            .Property(e => e.CreatedBy)
            .HasMaxLength(100)
            .HasDefaultValueSql("SYSTEM_USER")
            .ValueGeneratedOnAdd();

        modelBuilder.Entity<DbTriggerFaultyProduct>()
            .Property(e => e.CreatedByOverwritesAreIgnored)
            .HasMaxLength(100)
            .HasDefaultValueSql("SYSTEM_USER")
            .ValueGeneratedOnAdd()
            .Metadata.SetBeforeSaveBehavior(PropertySaveBehavior.Ignore);

        modelBuilder.Entity<DbTriggerFaultyProduct>()
            .Property(e => e.CreatedByOverwritesThrow)
            .HasMaxLength(100)
            .HasDefaultValueSql("SYSTEM_USER")
            .ValueGeneratedOnAdd()
            .Metadata.SetBeforeSaveBehavior(PropertySaveBehavior.Throw);



        modelBuilder.Entity<DbTriggerFaultyProduct>()
            .Property(e => e.ModifiedOn)
            .ValueGeneratedOnUpdate();

        modelBuilder.Entity<DbTriggerFaultyProduct>()
            .Property(e => e.ModifiedBy)
            .HasMaxLength(100)
            .ValueGeneratedOnUpdate();
    }
}
