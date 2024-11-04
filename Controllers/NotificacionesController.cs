using Microsoft.AspNetCore.Mvc;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Threading.Tasks;
using ms_notificaciones.Models;


[ApiController]
[Route("[controller]")]
public class NotificacionesController : ControllerBase
{
    [Route("correo")]
    [HttpPost]
   public async Task<IActionResult> EnviarCorreo(ModeloCorreo datos)
   {
            var apiKey = Environment.GetEnvironmentVariable("SENGRID_API_KEY");
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress(Environment.GetEnvironmentVariable("EMAIL_FROM"),Environment.GetEnvironmentVariable("NAME_FROM"));
            var subject = datos.asuntoCorreo;
            var to = new EmailAddress(datos.correoDestino, datos.nombreDestino);
            var plainTextContent = "plain text content";
            var htmlContent = datos.contenidoCorreo;
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            msg.SetTemplateId(Environment.GetEnvironmentVariable("WELCOME_SENGRID_TEMPLATE"));
            msg.SetTemplateData(new{
                name=datos.nombreDestino
            });
            var response = await client.SendEmailAsync(msg);
            
            if(response.StatusCode == System.Net.HttpStatusCode.Accepted)
            {
                return Ok("Correo enviado correctamente" + datos.correoDestino);
            }
            else
            {
                var responseBody = await response.Body.ReadAsStringAsync();
                return BadRequest($"No se pudo enviar el correo a la dirección: {datos.correoDestino}. Código de estado: {response.StatusCode}. Error: {responseBody}");
            }
   }
}