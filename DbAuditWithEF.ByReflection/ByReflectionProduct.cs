using System.ComponentModel.DataAnnotations;

namespace DbAuditWithEF.ByReflection;

public class ByReflectionProduct : IAudit
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
    private DateTime _createdOn = default!;
    private string _createdBy = "";
    private DateTime? _modifiedOn = null;
    private string? _modifiedBy = null;

    public DateTime CreatedOn => _createdOn;
    [MaxLength(100)]
    public string CreatedBy => _createdBy;

    public DateTime? ModifiedOn => _modifiedOn;
    [MaxLength(100)]
    public string? ModifiedBy => _modifiedBy;
}
