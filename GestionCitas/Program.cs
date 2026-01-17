using GestionCitas.Common;
using GestionCitas.Mappings;
using GestionCitas.Models;
using Microsoft.Build.Framework;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddScoped<ICurrentTenantService, CurrentTenantService>();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddDbContext<GestionCitasContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("MiConexion")));

builder.Services.AddAutoMapper(cfg => cfg.AddProfile<MappingProfile>());
builder.Services.AddCors(options =>
{
    options.AddPolicy("PermitirReact",
        policy =>
        {
            policy.WithOrigins("http://localhost:5173") // Puerto por defecto de Vite
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();  
    
}
app.UseCors("PermitirReact");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
