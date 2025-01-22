using Domain.Interfaces;
using Infrastructure.Context;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Service.Services;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IStoreRepository, StoreRepository>();
builder.Services.AddScoped<IStoreService, StoreService>();
builder.Services.AddScoped<ISchedulingRepository, SchedulingRepository>();
builder.Services.AddScoped<ISchedulingService, SchedulingService>();
builder.Services.AddScoped<ILogRepository, LogRepository>();
builder.Services.AddScoped<ILogService, LogService>();

// Obt�m o caminho do assembly principal (pasta de instala��o)
string appInstallationPath = Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location);

// Caminho do banco de dados (relativo � pasta de instala��o)
string databaseFilePath = Path.Combine(appInstallationPath, "Database", "Database.db");

// Verifica se o banco existe
if (!File.Exists(databaseFilePath))
{
    throw new FileNotFoundException("O banco de dados n�o foi encontrado no caminho especificado.", databaseFilePath);
}
// Gera a string de conex�o com o caminho din�mico
string conectionDefault = $"Data Source={databaseFilePath}";
builder.Services.AddDbContext<SendAppContext>(options =>
    options.UseSqlite(conectionDefault));

var app = builder.Build();

using(var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<SendAppContext>();
    context.Database.Migrate();

}
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
