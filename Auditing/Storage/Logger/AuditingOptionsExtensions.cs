using Auditing.Options;

namespace Auditing.Storage.Logger;

public static class AuditingOptionsExtensions
{
    public static AuditingOptions UseLogger(this AuditingOptions options)
    {
        options.RegisterExtension(new LoggerAuditingOptionsExtension());
        return options;
    }
}