# 📬 PorFolioApi - API de Contacto para Portafolio

Esta es una API REST desarrollada en ASP.NET Core (.NET 9)** que permite enviar correos electrónicos desde el formulario de contacto de un portafolio personal. 
Pensada para ser usada junto con aplicaciones frontend como Angular, React u otras.

## 🚀 Características

- ✉️ Envío de correos desde el formulario de contacto
- 🔐 Protección CORS para peticiones desde el dominio del portafolio
- 🧪 Documentación y pruebas disponibles vía Swagger
- 🐳 Desplegable con Docker
- 🌐 Publicada en Railway

## 🛠 Tecnologías

- .NET 9
- ASP.NET Core Web API
- C#
- Swagger (Swashbuckle)
- Docker

## 📦 Instalación y ejecución local

1. Clona este repositorio:

```bash
git clone https://github.com/victormgDev/PorFolioApi.git
cd PorFolioApi
````
Configura tu archivo appsettings.json con las creedenciales de tu servidor SMTP
```bash
"EmailSettings": {
  "From": "tu-correo@ejemplo.com",
  "SmtpServer": "smtp.ejemplo.com",
  "Port": 587,
  "Username": "tu-usuario",
  "Password": "tu-contraseña"
}
````
Ejecuta la api
```bash
dotnet run
````
Construye la imagen con Docker para el despliegue en Railway, ya que esta plataforma no acepta .NET9, solo hasta .NET6
```bash
docker build -t porfolio-api .
docker run -d -p 8080:80 porfolio-api
````

