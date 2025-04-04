using PortfolioApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Registrar el servicio de email
builder.Services.AddScoped<IEmailService, EmailService>();

// Configurar CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp", policy =>
    {
        var allowAnyOrigin = builder.Configuration.GetSection("Cors:AllowAnyOrigin").Get<string[]>();
        if (allowAnyOrigin != null && allowAnyOrigin.Length > 0)
        {
            policy.WithOrigins(allowAnyOrigin).AllowAnyMethod().AllowAnyHeader();
        }
        else
        {
            // En producción, incluir también la URL de tu aplicación Angular desplegada
            policy.WithOrigins(
                "http://localhost:4200",
                "https://www.victormontesgarrido.com" // Añade aquí la URL de tu frontend desplegado
            ).AllowAnyMethod().AllowAnyHeader().AllowCredentials();
        }
    });
});

var app = builder.Build();

// Configurar para que la aplicación funcione tanto en desarrollo como en producción
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    // En entornos de producción, podemos seguir ofreciendo Swagger para debugging
    // pero deberíamos considerar protegerlo o deshabilitarlo en producción final
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Considera omitir esta línea en producción si Railway solo maneja HTTP
// O verifica si el entorno es de desarrollo para decidir
if (app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

// Usar Cors
app.UseCors("AllowAngularApp");
app.UseAuthorization();
app.MapControllers();

app.Run();