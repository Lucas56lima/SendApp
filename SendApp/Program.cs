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

// Obtém o caminho do assembly principal (pasta de instalação)
string appInstallationPath = Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location);

// Caminho do banco de dados (relativo à pasta de instalação)
string databaseFilePath = Path.Combine(appInstallationPath, "Database", "Database.db");

// Verifica se o banco existe
if (!File.Exists(databaseFilePath))
{
    throw new FileNotFoundException("O banco de dados não foi encontrado no caminho especificado.", databaseFilePath);
}
// Gera a string de conexão com o caminho dinâmico
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
