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
    [Route("correo_bienvenida")]
    [HttpPost]
   public async Task<IActionResult> EnviarCorreoBienvenida(ModeloCorreo datos)
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
                return Ok("Correo enviado correctamente " + datos.correoDestino);
            }
            else
            {
                var responseBody = await response.Body.ReadAsStringAsync();
                return BadRequest($"No se pudo enviar el correo a la dirección: {datos.correoDestino}. Código de estado: {response.StatusCode}. Error: {responseBody}");
            }
   }
    [Route("correo-recuperacion-clave")]
    [HttpPost]
    public async Task<ActionResult> EnviarCorreoRecuperacionClave(ModeloCorreo datos)
    {
        SendGridMessage msg = this.crearMensajeBase(datos);
        var apiKey = Environment.GetEnvironmentVariable("SENGRID_API_KEY");
        var client = new SendGridClient(apiKey);

        msg.SetTemplateId(Environment.GetEnvironmentVariable("WELCOME_SENDGRID_TEMPLATE_ID"));
        var response = await client.SendEmailAsync(msg);
        msg.SetTemplateData(new
        {
            name = datos.nombreDestino,
            message = "password recovery"
        });

        if (response.StatusCode == System.Net.HttpStatusCode.Accepted)
        {
            return Ok("Correo sent successfully to: " + datos.correoDestino);

        }
        else
        {
            return BadRequest("Correo  sent failed: " + datos.correoDestino);
        }
    }


    [Route("enviar-correo-2fa")]
    [HttpPost]
    public async Task<ActionResult> EnviarCorreo2fa(ModeloCorreo datos)
    {
        var apiKey = Environment.GetEnvironmentVariable("SENGRID_API_KEY");
        var templateId = Environment.GetEnvironmentVariable("TwoFA_SENDGRID_TEMPLATE_ID");
        var client = new SendGridClient(apiKey);

        SendGridMessage msg = this.crearMensajeBase(datos);
        msg.SetTemplateId(Environment.GetEnvironmentVariable("TwoFA_SENDGRID_TEMPLATE_ID"));
        msg.SetTemplateData(new
        {
            name = datos.nombreDestino,
            message = datos.contenidoCorreo
        });
        var response = await client.SendEmailAsync(msg);
        if (response.StatusCode == System.Net.HttpStatusCode.Accepted)
        {
            return Ok("Correo enviado a la dirección " + datos.correoDestino);
        }
        else
        {
            return BadRequest("Error enviando el mensaje a la dirección: " + datos.correoDestino);
        }
    }

    [Route("enviar-certificado")]
    [HttpPost]
    public async Task<ActionResult> EnviarCertificado(ModeloCorreo datos)
    {
        var apiKey = Environment.GetEnvironmentVariable("SENGRID_API_KEY");
        var templateId = Environment.GetEnvironmentVariable("CERTIFICATE_SENDGRID_TEMPLATE_ID");
        var client = new SendGridClient(apiKey);

        SendGridMessage msg = this.crearMensajeBase(datos);
        msg.SetTemplateId(Environment.GetEnvironmentVariable("CERTIFICATE_SENDGRID_TEMPLATE_ID"));
        msg.SetTemplateData(new
        {
            name = datos.nombreDestino,
            message = datos.contenidoCorreo
        });
        var response = await client.SendEmailAsync(msg);
        if (response.StatusCode == System.Net.HttpStatusCode.Accepted)
        {
            return Ok("Correo enviado a la dirección " + datos.correoDestino);
        }
        else
        {
            return BadRequest("Error enviando el mensaje a la dirección: " + datos.correoDestino);
        }
    }

    [Route("hash-validacion-usuario")]
    [HttpPost]
    public async Task<ActionResult> EnviarHashValidacionUsuario(ModeloCorreo datos)
    {
        var apiKey = Environment.GetEnvironmentVariable("SENGRID_API_KEY");
        var templateId = Environment.GetEnvironmentVariable("HASH_SENDGRID_TEMPLATE_ID");
        var client = new SendGridClient(apiKey);

        SendGridMessage msg = this.crearMensajeBase(datos);
        msg.SetTemplateId("d-1b68b49ac95a458db871b01cb92e472b");
        msg.SetTemplateData(new
        {
            nombre = datos.nombreDestino,
            mensaje = datos.contenidoCorreo,
            asunto = datos.asuntoCorreo
        });
        var response = await client.SendEmailAsync(msg);
        if (response.StatusCode == System.Net.HttpStatusCode.Accepted)
        {
            return Ok("Correo enviado a la dirección " + datos.correoDestino);
        }
        else
        {
            return BadRequest("Error enviando el mensaje a la dirección: " + datos.correoDestino);
        }
    }
    private SendGridMessage crearMensajeBase(ModeloCorreo datos)
    {
        var apiKey = Environment.GetEnvironmentVariable("SENGRID_API_KEY");

        if (string.IsNullOrEmpty(apiKey))
        {
            throw new Exception("La clave de API de SendGrid no se ha configurado correctamente.");
        }

        var client = new SendGridClient(apiKey);
        var from = new EmailAddress(Environment.GetEnvironmentVariable("EMAIL_FROM"),Environment.GetEnvironmentVariable("NAME_FROM"));
        var subject = datos.asuntoCorreo;
        var to = new EmailAddress(datos.correoDestino, datos.nombreDestino);
        var plainTextContent = "plain text content";
        var htmlContent = datos.contenidoCorreo;
        var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
        return msg;
    }
}