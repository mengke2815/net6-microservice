using IdentityServer4.AccessTokenValidation;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Provider.Consul;
using Ocelot.Provider.Polly;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication("Bearer")
                .AddIdentityServerAuthentication(options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.Authority = $"http://localhost:5000";
                    options.ApiName = "net6_microservice";
                    options.SupportedTokens = SupportedTokens.Both;
                });

builder.Services.AddOcelot(new ConfigurationBuilder()
                .AddJsonFile("ocelot.json").Build())
                .AddPolly()
                .AddConsul();

// Add services to the container.
builder.Services.AddControllers();

//�Զ���url
builder.WebHost.UseUrls("http://*:5001");

var app = builder.Build();

// Configure the HTTP request pipeline.
app.Map("/home", s =>
{
    s.Run(async context =>
    {
        context.Response.ContentType = "text/plain;charset=utf-8";
        await context.Response.WriteAsync("Ocelot ���ط���������...");
    });
});

app.UseOcelot().Wait();

app.Run();