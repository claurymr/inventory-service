using FastEndpoints;
using InventoryService.Api.Extensions;
using InventoryService.Api.Middlewares;
using InventoryService.Infrastructure.Data;
using InventoryService.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddConfigSettings(builder.Configuration);
builder.Services.AddDbContext<InventoryServiceDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("InventoryServiceConnection")));
builder.Services.AddAuth(builder.Configuration);
builder.Services.AddAuthorizationBuilder()
    .AddPolicy("AdminOnly", policy => policy.RequireRole("admin"))
    .AddPolicy("AdminOrUser", policy => policy.RequireRole("admin", "user"));
builder.Services.AddRabbitMQ(builder.Configuration);
builder.Services.AddInventoryServiceServices();
builder.Services.AddFastEndpoints();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}
app.UseMiddleware<ErrorHandlerMiddleware>();
app.UseAuthentication();
app.UseAuthorization();
app.UseFastEndpoints();
app.MigrateDatabase();
app.Run();
