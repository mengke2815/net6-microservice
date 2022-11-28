var builder = WebApplication.CreateBuilder(args);
var basePath = AppContext.BaseDirectory;

#region ���������ļ�
var _config = new ConfigurationBuilder()
                 .SetBasePath(basePath)
                 .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                 .Build();
builder.Services.AddSingleton(new AppSettingsHelper(_config));
#endregion

#region ע�����ݿ�
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

#region ע��IdentityServer
builder.Services.AddIdentityServer()
       .AddDeveloperSigningCredential()
       .AddInMemoryApiResources(ApiConfig.GetApiResources)
       .AddInMemoryApiScopes(ApiConfig.ApiScopes) //4.0�汾��Ҫ��ӣ���Ȼ����ʱ��ʾinvalid_scope����
       .AddInMemoryClients(ApiConfig.GetClients())
       .AddInMemoryIdentityResources(ApiConfig.GetIdentityResources())//��Ӷ�OpenID Connect��֧��
       .AddResourceOwnerValidator<ResourceOwnerPasswordValidator>()
       .AddProfileService<ProfileService>();
#endregion

#region ��ʼ����־
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

//�Զ���url
builder.WebHost.UseUrls("http://*:5000");
var app = builder.Build();

//���IdentityServer�м����Pipeline
app.UseIdentityServer();

#region ע�����
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
        await context.Response.WriteAsync("IdentityServer4 ��Ȩ����������...");
    });
});

app.Run();