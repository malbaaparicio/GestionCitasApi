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
builder.Services.AddHttpContextAccessor();
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

builder.Services.AddAuthentication(options =>
{
    // Obligamos a .NET a usar Bearer para todo por defecto
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.Authority = builder.Configuration["Keycloak:Authority"];
    options.RequireHttpsMetadata = false;

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["Keycloak:Authority"],
        ValidateAudience = false,
        ValidateLifetime = true
    };

    options.Events = new JwtBearerEvents
    {
        // NUEVO: Este evento salta en cuanto llega la petición HTTP
        OnMessageReceived = context =>
        {
            var authHeader = context.Request.Headers["Authorization"].ToString();
            Console.WriteLine($"\n CABECERA RECIBIDA EN .NET: {(string.IsNullOrEmpty(authHeader) ? "NINGUNA" : authHeader.Substring(0, Math.Min(20, authHeader.Length)) + "...")}\n");
            return Task.CompletedTask;
        },
        OnAuthenticationFailed = context =>
        {
            Console.WriteLine(" FALLO DE AUTENTICACIÓN: " + context.Exception.Message);
            return Task.CompletedTask;
        },
        OnChallenge = context =>
        {
            Console.WriteLine($" CHALLENGE: {context.Error}, {context.ErrorDescription}");
            return Task.CompletedTask;
        }
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
app.UseRouting();
app.UseCors("PermitirReact");

app.UseHttpsRedirection();

app.UseAuthentication(); // 1º ¿Quién eres? (Valida el Token)
app.UseAuthorization();  // 2º ¿Tienes permiso? (Valida los Roles)

app.MapControllers();

app.Run();
