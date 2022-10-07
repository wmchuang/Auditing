## 审计模块

*通过注册中间件自动记录请求记录，同时记录请求的总执行时间,以毫秒为单位. 可以用来观察应用程序的性能。
支持Logger（nLog+es）、MySql、MongoDB三种类型的数据仓储持久化,
同时支持自定义数据保存时长,定期自动删除历史记录。*

---
### 使用用法
````csharp
#region 审计服务

services.AddAuditingService(x =>
{
    x.IsEnabled = Convert.ToBoolean(Configuration["AuditingIsEnabled"]);
    x.RetainTime = TimeSpan.FromMinutes(1);
    // x.UseMySql(Configuration["ConnectionSetting"]);
   x.UseMongoDB(Configuration["AuditingMongoDBConnectionSetting"]);
});

#endregion
````

```csharp
app.UseAuditing();
```
### 参考资料
>[ABP审计日志](https://docs.abp.io/zh-Hans/abp/latest/Audit-Logging)
>
> [BackgroundService](https://learn.microsoft.com/zh-cn/dotnet/architecture/microservices/multi-container-microservice-net-applications/background-tasks-with-ihostedservice)
>
> [CAP关于发布和订阅消息记录存储的实现](https://github.com/dotnetcore/CAP)