using System.ComponentModel.DataAnnotations;

namespace DbAuditWithEF.ByEF;

public class ByEFProduct : IAudit
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public Audit Audit { get; } = new();
}

public class ByEFClient : IAudit
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public Audit Audit { get; } = new();
}

public interface IAudit
{
    Audit Audit { get; }
}

public class Audit
{
    public DateTime CreatedOn { get; set; }
    [MaxLength(100)]
    public string CreatedBy { get; set; } = "";

    public DateTime? ModifiedOn { get; set; }
    [MaxLength(100)]
    public string? ModifiedBy { get; set; }
}
