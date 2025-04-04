using PortfolioApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Registrar el servicio de email
builder.Services.AddScoped<IEmailService, EmailService>();

// Configurar CORS de forma explícita
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp", policy =>
    {
        // En producción, permitir el acceso solo desde los orígenes específicos
        policy.WithOrigins(
                "http://localhost:4200", 
                "https://www.victormontesgarrido.com" // URL del frontend en producción
            )
            .AllowAnyMethod() // Permitir cualquier método HTTP
            .AllowAnyHeader() // Permitir cualquier cabecera
            .AllowCredentials(); // Permitir credenciales (si las necesitas)
    });
});

var app = builder.Build();

// Configurar para que la aplicación funcione tanto en desarrollo como en producción
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Usar HTTPS solo en desarrollo (en producción, Railway maneja HTTPS)
if (app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

// Usar CORS
app.UseCors("AllowAngularApp");

app.UseAuthorization();
app.MapControllers();

app.Run();