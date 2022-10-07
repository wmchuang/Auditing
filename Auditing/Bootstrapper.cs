﻿using Auditing.Options;
using Auditing.Storage;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Auditing;

/// <summary>
/// 审计后台服务
/// </summary>
public class Bootstrapper : BackgroundService
{
    private readonly ILogger<Bootstrapper> _logger;
    private readonly IOptions<AuditingOptions> _options;
    private readonly IStorageInitializer _storage;
    private readonly IAuditingStore _auditingStore;

    private const int ItemBatch = 1000;
    private readonly TimeSpan _waitingInterval = TimeSpan.FromMinutes(5);
    private readonly TimeSpan _delay = TimeSpan.FromSeconds(1);

    public Bootstrapper(ILogger<Bootstrapper> logger, IOptions<AuditingOptions> options, IStorageInitializer storage, IAuditingStore auditingStore)
    {
        _logger = logger;
        _options = options;
        _storage = storage;
        _auditingStore = auditingStore;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        //初始化存储库
        await _storage.InitializeAsync(stoppingToken);
        
        var retainTime = _options.Value.RetainTime;
        while (!stoppingToken.IsCancellationRequested)
        {
            int deletedCount;
            var time = DateTime.Now;

            // 获取需要删除的时间界限
            var timeout = time.AddSeconds(retainTime.TotalSeconds * -1);
            do
            {
                //删除过期的记录
                deletedCount = await _auditingStore.DeleteExpiresAsync(timeout, ItemBatch, stoppingToken);

                if (deletedCount != 0)
                {
                    await WaitAsync(_delay, stoppingToken);
                    stoppingToken.ThrowIfCancellationRequested();
                }

                _logger.LogInformation($"删除{timeout:yyyy-MM-dd HH:mm:ss}之前的审计日志{deletedCount}条");
            } while (deletedCount != 0);

            // 等待一段时间
            await WaitAsync(_waitingInterval, stoppingToken);
        }
    }

    private Task WaitAsync(TimeSpan timeout, CancellationToken stoppingToken)
    {
        return Task.Delay(timeout, stoppingToken);
    }
}