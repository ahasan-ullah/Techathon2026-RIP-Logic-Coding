using BLL.Interfaces;
using BLL.Services;
using DAL.Data;
using DAL.Interfaces;
using DAL.Repositories;
using Microsoft.EntityFrameworkCore;
using OfficeMonitor.API.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//database
builder.Services.AddDbContext<OfficeMonitorContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

//repos
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

//services
builder.Services.AddScoped<IDeviceService, DeviceService>();
builder.Services.AddScoped<IPowerCalculationService, PowerCalculationService>();
builder.Services.AddScoped<IAlertService, AlertService>();

//apis
builder.Services.AddSignalR();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
app.MapHub<DeviceHub>("/hub/devices");

app.Run();
