using FastEndpoints;
using InventoryService.Api.Extensions;
using InventoryService.Infrastructure.Data;
using InventoryService.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddFastEndpoints();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddInventoryServiceServices();
builder.Services.AddDbContext<InventoryServiceDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("InventoryServiceConnection")));
builder.Services.AddRabbitMQ(builder.Configuration);
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

app.UseFastEndpoints();
app.MigrateDatabase();
app.Run();
