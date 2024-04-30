using Microsoft.EntityFrameworkCore;
using UserManagement.DataAccess;
using UserManagement.DataAccess.Repositories;
using UserManagement.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add settings
var dataAccessSettings = builder.Configuration.GetRequiredSection(nameof(DataAccessSettings)).Get<DataAccessSettings>()
    ?? throw new InvalidOperationException($"{nameof(DataAccessSettings)} not set");
builder.Services.AddSingleton(dataAccessSettings);

// Add DB Context
builder.Services.AddDbContext<DataContext>(options =>
{
    options.UseSqlServer(dataAccessSettings.ConnectionString);
}, ServiceLifetime.Transient);

// Add injections
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IPermissionRepository, PermissionRepository>();
builder.Services.AddScoped<IUserPermissionsService, UserPermissionsService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    scope.ServiceProvider.GetRequiredService<DataContext>().Database.Migrate();
}

app.Run();
