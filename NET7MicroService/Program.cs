using CommonLibrary;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerUI;
using static CommonLibrary.AppBuilderExtensions;

var builder = WebApplication.CreateBuilder(args);
var basePath = AppContext.BaseDirectory;

#region 引入配置文件
var _config = new ConfigurationBuilder()
                 .SetBasePath(basePath)
                 .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                 .Build();
builder.Services.AddSingleton(new AppSettingsHelper(_config));
#endregion

#region 接口分组
var groups = new List<Tuple<string, string>>
{
    //new Tuple<string, string>("Group1","分组一"),
    //new Tuple<string, string>("Group2","分组二")
};
#endregion

#region 添加swagger注释
builder.Services.AddSwaggerGen(a =>
{
    a.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Api",
        Description = "Api接口文档"
    });
    foreach (var item in groups)
    {
        a.SwaggerDoc(item.Item1, new OpenApiInfo { Version = item.Item1, Title = item.Item2, Description = $"{item.Item2}接口文档" });
    }
    a.IncludeXmlComments(Path.Combine(basePath, "NET7MicroService.xml"), true);
    a.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Value: Bearer {token}",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    a.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
      {
        new OpenApiSecurityScheme
        {
          Reference = new OpenApiReference
          {
            Type = ReferenceType.SecurityScheme,
            Id = "Bearer"
          }, Scheme = "oauth2", Name = "Bearer", In = ParameterLocation.Header }, new List<string>()
        }
    });
});
#endregion

#region 添加身份认证
builder.Services.AddAuthentication("Bearer").AddIdentityServerAuthentication(options =>
{
    options.Authority = AppSettingsHelper.Get("IdentityServer:Authority");//配置Identityserver的授权地址
    options.RequireHttpsMetadata = false;//不需要https    
    options.ApiName = AppSettingsHelper.Get("IdentityServer:ApiName");//api的name，需要和config的名称相同
});
#endregion

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();//添加鉴权认证
app.UseAuthorization();


#region 启用swaggerUI
app.UseSwagger();
app.UseSwaggerUI(a =>
{
    a.SwaggerEndpoint($"/swagger/v1/swagger.json", "V1 Docs");
    foreach (var item in groups)
    {
        a.SwaggerEndpoint($"/swagger/{item.Item1}/swagger.json", item.Item2);
    }
    a.RoutePrefix = string.Empty;
    a.DocExpansion(DocExpansion.None);
    a.DefaultModelsExpandDepth(-1);//不显示Models
});
#endregion

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

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
