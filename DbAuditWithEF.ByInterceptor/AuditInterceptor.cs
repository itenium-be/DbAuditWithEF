using System.Reflection;
using DbAuditWithEF.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace DbAuditWithEF.ByInterceptor;

public class AuditInterceptor(IUserProvider userProvider) : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        SetAuditFields(eventData.Context!);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        SetAuditFields(eventData.Context!);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void SetAuditFields(DbContext context)
    {
        var entries = context.ChangeTracker
            .Entries<IAudit>()
            .Where(e => e.State is EntityState.Added or EntityState.Modified);

        foreach (var entityEntry in entries)
        {
            Audit audit = entityEntry.Entity.Audit;
            Type auditType = audit.GetType();
            if (entityEntry.State == EntityState.Added)
            {
                auditType.GetProperty(nameof(Audit.CreatedOn))!.SetValue(audit, DateTime.Now);
                auditType.GetProperty(nameof(Audit.CreatedBy))!.SetValue(audit, userProvider.UserName);
            }
            else
            {
                auditType.GetProperty(nameof(Audit.ModifiedOn))!.SetValue(audit, DateTime.Now);
                auditType.GetProperty(nameof(Audit.ModifiedBy))!.SetValue(audit, userProvider.UserName);
            }
        }
    }
}
