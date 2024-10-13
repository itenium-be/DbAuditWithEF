namespace DbAuditWithEF.DatabaseTrigger;

/// <summary>
/// Does not crash on update because of Trigger due configuration:
/// Requires the slower legacy SQL:
/// tb.UseSqlOutputClause(false)
///
/// <see cref="DbTriggerFaultyProduct.ModifiedBy"/> comment.
/// </summary>
public class DbTriggerProduct
{
    public int Id { get; set; }
    public string Name { get; set; } = "";

    public DateTime CreatedOn { get; set; }
    public string? CreatedBy { get; set; }


    public DateTime? ModifiedOn { get; set; }
    /// <summary>
    /// Configured with ValueGeneratedOnUpdate:
    /// Can set a value on Insert
    /// </summary>
    public string? ModifiedBy { get; set; }
    /// <summary>
    /// Configured with ValueGeneratedOnAddOrUpdate:
    /// Cannot set a value on Insert
    /// </summary>
    public string? ModifiedByOverwritesAreIgnored { get; set; }
}
