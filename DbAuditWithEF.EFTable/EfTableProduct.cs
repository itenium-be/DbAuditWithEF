using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace DbAuditWithEF.EFTable;

public interface IId
{
    int Id { get; set; }
}

public class EfTableProduct : IId
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
}

public class EfAudit
{
    public int Id { get; set; }
    [MaxLength(100)]
    public string TableName { get; set; }
    public int TableId { get; set; }
    public EntityState ActionType { get; set; }
    public string? OldValues { get; set; }
    public string? NewValues { get; set; }
    public DateTime ModifiedOn { get; set; }
    [MaxLength(100)]
    public string ModifiedBy { get; set; }
}
