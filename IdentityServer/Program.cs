var builder = WebApplication.CreateBuilder(args);
var basePath = AppContext.BaseDirectory;

#region 引入配置文件
var _config = new ConfigurationBuilder()
                 .SetBasePath(basePath)
                 .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                 .Build();
builder.Services.AddSingleton(new AppSettingsHelper(_config));
#endregion

#region 注入数据库
var dbtype = DbType.SqlServer;
if (AppSettingsHelper.Get("SugarConnectDBType", true) == "mysql")
{
    dbtype = DbType.MySql;
}
builder.Services.AddScoped(options =>
{
    return new SqlSugarScope(new List<ConnectionConfig>()
    {
        new ConnectionConfig() { ConfigId = 1, ConnectionString = AppSettingsHelper.Get("SugarConnectString", true), DbType = dbtype, IsAutoCloseConnection = true }
    });
});
#endregion

#region 注入IdentityServer
builder.Services.AddIdentityServer()
       .AddDeveloperSigningCredential()
       .AddInMemoryApiResources(ApiConfig.GetApiResources)
       .AddInMemoryApiScopes(ApiConfig.ApiScopes) //4.0版本需要添加，不然调用时提示invalid_scope错误
       .AddInMemoryClients(ApiConfig.GetClients())
       .AddInMemoryIdentityResources(ApiConfig.GetIdentityResources())//添加对OpenID Connect的支持
       .AddResourceOwnerValidator<ResourceOwnerPasswordValidator>()
       .AddProfileService<ProfileService>();
#endregion

#region 初始化日志
builder.Host.UseSerilog((builderContext, config) =>
{
    config
    .MinimumLevel.Warning()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File(Path.Combine("Logs", @"Log.txt"), rollingInterval: RollingInterval.Day);
});
#endregion

// Add services to the container.
builder.Services.AddControllers();

//自定义url
builder.WebHost.UseUrls("http://*:5000");
var app = builder.Build();

//添加IdentityServer中间件到Pipeline
app.UseIdentityServer();

#region 注册服务
app.RegisterConsul(app.Lifetime, new ServiceEntity
{
    IP = AppSettingsHelper.Get("Service:IP"),
    Port = Convert.ToInt32(AppSettingsHelper.Get("Service:Port")),
    ServiceName = AppSettingsHelper.Get("Service:Name"),
    ConsulIP = AppSettingsHelper.Get("Consul:IP"),
    ConsulPort = Convert.ToInt32(AppSettingsHelper.Get("Consul:Port"))
});
#endregion

// Configure the HTTP request pipeline.
app.Map("/home", s =>
{
    s.Run(async context =>
    {
        context.Response.ContentType = "text/plain;charset=utf-8";
        await context.Response.WriteAsync("IdentityServer4 鉴权服务已启动...");
    });
});

app.Run();