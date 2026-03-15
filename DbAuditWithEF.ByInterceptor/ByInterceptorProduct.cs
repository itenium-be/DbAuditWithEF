using System.ComponentModel.DataAnnotations;

namespace DbAuditWithEF.ByInterceptor;

public class ByInterceptorProduct : IAudit
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
    public DateTime CreatedOn { get; private set; }
    [MaxLength(100)]
    public string CreatedBy { get; private set; } = "";

    public DateTime? ModifiedOn { get; private set; }
    [MaxLength(100)]
    public string? ModifiedBy { get; private set; }
}
