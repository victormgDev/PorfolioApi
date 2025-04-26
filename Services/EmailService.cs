using MimeKit;
using MailKit.Net.Smtp;
using MailKit.Security;
using System.Threading.Tasks;
using PorfolioApi.Models;

namespace PortfolioApi.Services
{
    public interface IEmailService
    {
        Task<bool> SendContactFormEmailsAsync(ContactFormModel model);
    }

    public class EmailService : IEmailService
    {
        private readonly string _smtpServer;
        private readonly int _smtpPort;
        private readonly string _smtpUsername;
        private readonly string _smtpPassword;
        private readonly string _adminEmail;

        public EmailService(IConfiguration configuration)
        {
            // Intentar obtener de las variables de entorno primero, luego de la configuración
            _smtpServer = Environment.GetEnvironmentVariable("EMAIL_SMTP_SERVER") 
                ?? configuration["EmailSettings:SmtpServer"];
                
            // Parsear el puerto, con valor predeterminado 587 si hay error
            if (!int.TryParse(Environment.GetEnvironmentVariable("EMAIL_SMTP_PORT"), out _smtpPort))
            {
                if (!int.TryParse(configuration["EmailSettings:SmtpPort"], out _smtpPort))
                {
                    _smtpPort = 587; // Valor predeterminado
                }
            }
            
            _smtpUsername = Environment.GetEnvironmentVariable("EMAIL_USERNAME") 
                ?? configuration["EmailSettings:SmtpUsername"];
                
            _smtpPassword = Environment.GetEnvironmentVariable("EMAIL_PASSWORD") 
                ?? configuration["EmailSettings:SmtpPassword"];
                
            _adminEmail = Environment.GetEnvironmentVariable("ADMIN_EMAIL") 
                ?? configuration["EmailSettings:AdminEmail"];
        }

        public async Task<bool> SendContactFormEmailsAsync(ContactFormModel model)
        {
            try
            {
                // El resto del método permanece igual
                // Enviar correo al administrador
                var adminMessage = new MimeMessage();
                adminMessage.From.Add(new MailboxAddress("PortFolio Víctor", _smtpUsername));
                adminMessage.To.Add(new MailboxAddress("Admin", _adminEmail));
                adminMessage.Subject = $"Nuevo mensaje de {model.Name}: {model.Subject}";
                adminMessage.ReplyTo.Add(new MailboxAddress(model.Name, model.Email));

                var adminBodyBuilder = new BodyBuilder
                {
                    HtmlBody = $@"
                        <h2>Has recibido un nuevo mensaje de contacto</h2>
                        <p><strong>Nombre:</strong> {model.Name}</p>
                        <p><strong>Email:</strong> {model.Email}</p>
                        <p><strong>Asunto:</strong> {model.Subject}</p>
                        <p><strong>Mensaje:</strong></p>
                        <p>{model.Message}</p>
                    "
                };

                adminMessage.Body = adminBodyBuilder.ToMessageBody();

                // Enviar correo al usuario
                var userMessage = new MimeMessage();
                userMessage.From.Add(new MailboxAddress("PortFolio Victor", _smtpUsername));
                userMessage.To.Add(new MailboxAddress(model.Name, model.Email));
                userMessage.Subject = "Gracias por contactar con Victor Montes Garrido";

                var userBodyBuilder = new BodyBuilder
                {
                    HtmlBody = $@"                    
                        <h2><span style=""color:#0DCAF0FF"">¡Gracias por ponerte en contacto!</span></h2>
                        <p>Hola {model.Name},</p>
                        <p>He recibido tu mensaje, responderé lo antes posible.</p>
                        <p>A continuación encontrarás una copia de tu mensaje:</p>
                        <p>Un cordial saludo.</p>
                        <p>Victor Montes Garrido</p>  
                        <br>
                        <p>-----------------------------------------</p>
                        <p><strong>Asunto:</strong> {model.Subject}</p>
                        <p><strong>Mensaje:</strong></p>
                        <p>{model.Message}</p>
                        <br>
                                            
                        <a href=""https://www.victormontesgarrido.com"" target=""_blank"">
                        <img style=""width:auto; height:80px"" src=""https://www.victormontesgarrido.com/assets/logoMail.png"" />
                        www.victormontesgarrido.com
                        </a>                                    
                    "
                };

                userMessage.Body = userBodyBuilder.ToMessageBody();

                using (var client = new SmtpClient())
                {
                    await client.ConnectAsync(_smtpServer, _smtpPort, SecureSocketOptions.StartTls);
                    await client.AuthenticateAsync(_smtpUsername, _smtpPassword);

                    // Enviar ambos correos
                    await client.SendAsync(adminMessage);
                    await client.SendAsync(userMessage);

                    await client.DisconnectAsync(true);
                }

                return true;
            }
            catch (Exception ex)
            {
                // Para depuración en entorno de desarrollo, considera registrar la excepción
                Console.WriteLine($"Error enviando email: {ex.Message}");
                return false;
            }
        }
    }
}