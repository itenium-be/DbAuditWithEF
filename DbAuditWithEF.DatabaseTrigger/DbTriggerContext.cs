using Microsoft.EntityFrameworkCore;
using System.Data.Common;
using DbAuditWithEF.Utils;

namespace DbAuditWithEF.DatabaseTrigger;

public class DbTriggerContext : DbContext
{
    public DbSet<Product> Products { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(ConnectionStringBuilder.Get("DatabaseTrigger"));
    }
}

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; }

    public DateTime CreatedOn { get; set; }
    public string CreatedBy { get; set; }
    public DateTime ModifiedOn { get; set; }
    public string ModifiedBy { get; set; }
}
