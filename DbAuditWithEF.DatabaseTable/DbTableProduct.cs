using System.ComponentModel.DataAnnotations;

namespace DbAuditWithEF.DatabaseTable;

public class DbTableProduct
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public DateTime? ModifiedOn { get; set; }
    public string? ModifiedBy { get; set; }
}

public class DbAudit
{
    public int Id { get; set; }
    [MaxLength(100)]
    public string TableName { get; set; } = null!;
    public string TableIds { get; set; } = null!;
    public ActionType ActionType { get; set; }
    public string? OldValues { get; set; }
    public string? NewValues { get; set; }
    public DateTime ModifiedOn { get; set; }
    [MaxLength(100)]
    public string ModifiedBy { get; set; } = null!;
}

public enum ActionType
{
    U,
    I,
    D,
}
