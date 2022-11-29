using CommonLibrary;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerUI;
using static CommonLibrary.AppBuilderExtensions;

var builder = WebApplication.CreateBuilder(args);
var basePath = AppContext.BaseDirectory;

#region ���������ļ�
var _config = new ConfigurationBuilder()
                 .SetBasePath(basePath)
                 .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                 .Build();
builder.Services.AddSingleton(new AppSettingsHelper(_config));
#endregion

#region �ӿڷ���
var groups = new List<Tuple<string, string>>
{
    //new Tuple<string, string>("Group1","����һ"),
    //new Tuple<string, string>("Group2","�����")
};
#endregion

#region ���swaggerע��
builder.Services.AddSwaggerGen(a =>
{
    a.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Api",
        Description = "Api�ӿ��ĵ�"
    });
    foreach (var item in groups)
    {
        a.SwaggerDoc(item.Item1, new OpenApiInfo { Version = item.Item1, Title = item.Item2, Description = $"{item.Item2}�ӿ��ĵ�" });
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

#region ��������֤
builder.Services.AddAuthentication("Bearer").AddIdentityServerAuthentication(options =>
{
    options.Authority = AppSettingsHelper.Get("IdentityServer:Authority");//����Identityserver����Ȩ��ַ
    options.RequireHttpsMetadata = false;//����Ҫhttps    
    options.ApiName = AppSettingsHelper.Get("IdentityServer:ApiName");//api��name����Ҫ��config��������ͬ
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

app.UseAuthentication();//��Ӽ�Ȩ��֤
app.UseAuthorization();


#region ����swaggerUI
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
    a.DefaultModelsExpandDepth(-1);//����ʾModels
});
#endregion

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

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
