using Auditing.Storage.Mysql;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

//添加审计服务
builder.Services.AddAuditing(x =>
{
    x.IsEnabled = true;
    x.RetainTime = TimeSpan.FromDays(1);
    x.UseMySql(m =>
    {
        m.ConnectionString = "server=192.168.10.66;user=root;database='unity3d';port=3306;password=qwe@123!;SslMode=None";
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseAuthorization();

//启用审计服务
app.UseAuditing();
app.MapControllers();

app.Run();
