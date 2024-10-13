using System.ComponentModel.DataAnnotations;

namespace DbAuditWithEF.DatabaseTrigger;

/// <summary>
/// Faulty: crashes when doing a DB update due the trigger
/// </summary>
public class DbTriggerFaultyProduct
{
    public int Id { get; set; }
    [StringLength(100)] // For validation & DB
    [MaxLength(100)]    // Just for the DB
    public string Name { get; set; }

    public DateTime CreatedOn { get; set; }
    /// <summary>
    /// Uses only ValueGeneratedOnAdd and can be overwritten in code
    /// </summary>
    public string? CreatedBy { get; set; }
    /// <summary>
    /// Uses ValueGeneratedOnAdd and PropertySaveBehavior.Ignore
    /// so it cannot be overwritten in code
    /// </summary>
    public string? CreatedByOverwritesAreIgnored { get; set; }
    /// <summary>
    /// Uses ValueGeneratedOnAdd and PropertySaveBehavior.Throw
    /// so the developer is warned that this is not possible
    /// </summary>
    public string? CreatedByOverwritesThrow { get; set; }


    public DateTime? ModifiedOn { get; set; }
    /// <summary>
    /// Faulty configuration:
    /// Uses only ValueGeneratedOnUpdate:
    /// - Value can be set during insert
    /// - Crash on update because the record is updated with the trigger trg_UpdateFaultyAuditFields
    /// See: https://learn.microsoft.com/en-us/ef/core/what-is-new/ef-core-7.0/breaking-changes?tabs=v7#sqlserver-tables-with-triggers
    /// </summary>
    public string? ModifiedBy { get; set; }
}
