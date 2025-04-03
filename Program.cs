
using PortfolioApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Registrar el servicio de email
builder.Services.AddScoped<IEmailService, EmailService>();

//Configurar CORS
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
            policy.WithOrigins("http://localhost:4200").AllowAnyMethod().AllowAnyHeader();

        }
    });
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    // app.MapOpenApi();
}

app.UseHttpsRedirection();

//Usar Cors
app.UseCors("AllowAngularApp");
app.UseAuthorization();
app.MapControllers();


app.Run();

