using GestionCitas.Common;
using GestionCitas.Mappings;
using GestionCitas.Models;
using Microsoft.Build.Framework;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

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
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        // La URL de tu servidor Keycloak
        options.Authority = builder.Configuration["Keycloak:Authority"];
        
        // Desactivamos el requisito de HTTPS porque en local (Docker) estamos usando HTTP
        options.RequireHttpsMetadata = false; 

        options.TokenValidationParameters = new TokenValidationParameters
        {
            // Validamos que el token venga de nuestro Keycloak
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["Keycloak:Authority"],
            
            // Por ahora desactivamos la validación de la audiencia para evitar bloqueos
            // mientras conectamos el frontend. Lo activaremos en producción.
            ValidateAudience = false,
            
            // Validamos que el token no haya caducado
            ValidateLifetime = true
        };
    });

// Añadimos también el servicio de Autorización
builder.Services.AddAuthorization();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();  
    
}
app.UseCors("PermitirReact");

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseAuthentication(); // 1º ¿Quién eres? (Valida el Token)
app.UseAuthorization();  // 2º ¿Tienes permiso? (Valida los Roles)

app.MapControllers();

app.Run();
