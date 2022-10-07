namespace Auditing.Options;

public class AuditingOptions
{
    /// <summary>
    /// 是否开启
    /// </summary>
    public bool IsEnabled { get; set; }

    /// <summary>
    /// 记录保存时长,默认1天
    /// </summary>
    public TimeSpan RetainTime { get; set; } = TimeSpan.FromDays(1);

    /// <summary>
    /// 扩展子服务
    /// </summary>
    internal IList<IAuditingOptionsExtension> Extensions { get; } = new List<IAuditingOptionsExtension>(0);
    
    /// <summary>
    /// Registers an extension that will be executed when building services.
    /// </summary>
    /// <param name="extension"></param>
    public void RegisterExtension(IAuditingOptionsExtension extension)
    {
        if (extension == null)
        {
            throw new ArgumentNullException(nameof(extension));
        }

        Extensions.Add(extension);
    }
}