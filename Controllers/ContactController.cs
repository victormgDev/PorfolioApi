using Microsoft.AspNetCore.Mvc;
using PorfolioApi.Models;
using PortfolioApi.Services;

namespace PorfolioApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    
    public class ContactController : ControllerBase
    {
        private readonly IEmailService _emailService;

        public ContactController(IEmailService emailService)
        {
            _emailService = emailService;
        }

        [HttpPost]
        public async Task<IActionResult> SendContactForm([FromBody] ContactFormModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            //Validamos campos obligatorios
            if (string.IsNullOrEmpty(model.Name) ||
                string.IsNullOrEmpty(model.Email) ||
                string.IsNullOrEmpty(model.Subject) ||
                string.IsNullOrEmpty(model.Message))
            {
                return BadRequest("Todos los campos son obligatorios");
            }

            //Enviar Correos
            bool result = await _emailService.SendContactFormEmailsAsync(model);

            if (result)
            {
                return Ok(new {success = true, message = "Email enviado correctamente"});
            }
            else
            {
                return StatusCode(500, new { error = "Error al enviar los Correos email" });
            }
        }
        
    }
}

