
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StockMarket.Api.Data;
using StockMarket.Api.Hubs;
using StockMarket.Api.Models;
using StockMarket.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<User, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddControllers();

builder.Services.AddSignalR();
builder.Services.AddSingleton<StockService>();
builder.Services.AddHostedService(provider =>
{
    var stockService = provider.GetService<StockService>();
    if (stockService == null)
    {
        throw new InvalidOperationException("StockService not found");
    }
    return stockService;
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", builder =>
    {
        builder.WithOrigins("http://localhost:4200")
               .AllowAnyMethod()
               .AllowAnyHeader()
               .AllowCredentials();
    });
});

var app = builder.Build();

app.UseHttpsRedirection();

app.UseCors("CorsPolicy");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapHub<StockHub>("/stockhub");
app.MapHub<ChatHub>("/chathub");

await SeedData.Initialize(app.Services);

app.Run();
