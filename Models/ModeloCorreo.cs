using Microsoft.OpenApi.Any;
using static System.Runtime.InteropServices.JavaScript.JSType;

/// <summary>Represents a model for an email message.</summary>
namespace ms_notificaciones.Models
{
    public class ModeloCorreo
    {
        /// <summary>The email address of the recipient.</summary>
        public string? correoDestino { set; get; }
        /// <summary>The name of the recipient.</summary>
        public string? nombreDestino { set; get; }
        /// <summary>The subject of the email message.</summary>
        public string? asuntoCorreo { set; get; }
        /// <summary>The content of the email message.</summary>
        public string? contenidoCorreo { set; get; }
    }
}