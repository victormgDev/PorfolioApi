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
            _smtpServer = configuration["EmailSettings:SmtpServer"];
            _smtpPort = int.Parse(configuration["EmailSettings:SmtpPort"]);
            _smtpUsername = configuration["EmailSettings:SmtpUsername"];
            _smtpPassword = configuration["EmailSettings:SmtpPassword"];
            _adminEmail = configuration["EmailSettings:AdminEmail"];
        }

        public async Task<bool> SendContactFormEmailsAsync(ContactFormModel model)
        {
            try
            {
                // Enviar correo al administrador
                var adminMessage = new MimeMessage();
                adminMessage.From.Add(new MailboxAddress("Formulario de Contacto", _smtpUsername));
                adminMessage.To.Add(new MailboxAddress("Admin", _adminEmail));
                adminMessage.Subject = $"Nuevo mensaje de contacto: {model.Subject}";

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
                userMessage.From.Add(new MailboxAddress("Victor Montes", _smtpUsername));
                userMessage.To.Add(new MailboxAddress(model.Name, model.Email));
                userMessage.Subject = "Gracias por contactar con Victor Montes";

                var userBodyBuilder = new BodyBuilder
                {
                    HtmlBody = $@"
                        <h2>¡Gracias por ponerte en contacto!</h2>
                        <p>Hola {model.Name},</p>
                        <p>He recibido tu mensaje, responderé lo antes posible.</p>
                        <p>A continuación encontrarás una copia de tu mensaje:</p>
                        <p><strong>Asunto:</strong> {model.Subject}</p>
                        <p><strong>Mensaje:</strong></p>
                        <p>{model.Message}</p>
                        <br>
                        <p>Un cordial saludo.</p>
                        <p>Victor Montes</p>
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
            catch
            {
                return false;
            }
        }
    }
}