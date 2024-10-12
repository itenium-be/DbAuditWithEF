using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using DbAuditWithEF.Utils;

namespace DbAuditWithEF.DatabaseTrigger;

public class DbTriggerContext : DbContext
{
    public DbSet<DbTriggerProduct> Products { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(ConnectionStringBuilder.Get("DatabaseTrigger"));
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DbTriggerProduct>()
            .Property(e => e.CreatedOn)
            .HasDefaultValueSql("GETDATE()")
            .ValueGeneratedOnAdd();

        modelBuilder.Entity<DbTriggerProduct>()
            .Property(e => e.CreatedBy)
            .HasMaxLength(100)
            .HasDefaultValueSql("SYSTEM_USER")
            .ValueGeneratedOnAdd();
    }
}

public class DbTriggerProduct
{
    public int Id { get; set; }
    [StringLength(100)]
    public string Name { get; set; }

    public DateTime CreatedOn { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? ModifiedOn { get; set; }
    [MaxLength(100)]
    public string? ModifiedBy { get; set; }
}
