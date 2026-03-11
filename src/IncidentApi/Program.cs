using IncidentApi.Data;
using Microsoft.EntityFrameworkCore;
using IncidentApi.Services;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

var builder = WebApplication.CreateBuilder(args);

// 1. Registro seguro de MongoDB (Evita errores si el contenedor se reinicia)
try 
{
    BsonSerializer.RegisterSerializer(new ObjectSerializer(type => true));
} 
catch { /* Ya registrado */ }

// 2. SQL Server con Estrategia de Reintento (Resiliencia para Docker)
builder.Services.AddDbContext<IncidentDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer"),
    sqlOptions => sqlOptions.EnableRetryOnFailure(
        maxRetryCount: 5, 
        maxRetryDelay: TimeSpan.FromSeconds(10), 
        errorNumbersToAdd: null)));

// 3. Registro de Servicios e Infraestructura
builder.Services.AddSingleton<MongoDbService>();
builder.Services.AddHttpClient();
builder.Services.AddScoped<IIncidentService, IncidentService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 4. Configuración de CORS (Permisivo para facilitar la conexión con Next.js)
builder.Services.AddCors(options => {
    options.AddDefaultPolicy(p => p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

var app = builder.Build();

// --- PIPELINE DE EJECUCIÓN (El orden aquí es vital) ---

// EL CORS SIEMPRE PRIMERO: Para que los errores 500 no bloqueen el navegador
app.UseCors(); 

// Swagger fuera del bloque 'IsDevelopment' para que funcione en Docker
app.UseSwagger();
app.UseSwaggerUI(c => {
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Incident API v1");
    c.RoutePrefix = string.Empty; // Swagger cargará directamente en http://localhost:5000/
});

app.UseAuthorization();
app.MapControllers();

// 5. AUTO-INICIALIZACIÓN: Creación de tablas al arrancar
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<IncidentDbContext>();
        context.Database.EnsureCreated(); 
        Console.WriteLine("--> SQL Server: Base de datos lista.");
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "--> SQL Server: Fallo en la conexión inicial.");
    }
}

app.Run();