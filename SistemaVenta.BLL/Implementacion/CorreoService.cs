using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Appwrite.Models;
using SistemaVenta.BLL.Interfaces;
using SistemaVenta.DAL.Interfaces;
using SistemaVenta.Entity;

namespace SistemaVenta.BLL.Implementacion
{
    public class CorreoService : ICorreoService
    {
        private readonly IGenerRepository<Configuracion> _repositorio;
        private string correoDestino;
        private object passwordTemporaldOEnlace;

        public CorreoService()
        {
        }

        public CorreoService(IGenerRepository<Configuracion> repositorio)
        {
            _repositorio = repositorio;
        }

        public string Asunto { get; private set; }
        public string Mensaje { get; private set; }
        public string CorreoDestino { get; private set; }

        public async Task<bool> EnviarCorreo(string CorreoDestino, string Asunto, string Mensaje)
        {
            try
            {
                IQueryable<Configuracion> query = await _repositorio.Consultar(c => c.Recurso.Equals("ServicioCorreo"));

                Dictionary<string, string> Config = query.ToDictionary(keySelector: c => c.Propiedad, elementSelector: c => c.Valor);

                var credenciales = new NetworkCredential(Config["correo"], Config["clave"]);

                var correo = new MailMessage()
                {
                    From = new MailAddress(Config["correo"], Config["alias"]),
                    Subject = Asunto,
                    Body = Mensaje,
                    IsBodyHtml = true
                };

                correo.To.Add(new MailAddress(CorreoDestino));

                var clienteServidor = new SmtpClient()
                {
                    Host = Config["host"],
                    Port = int.Parse(Config["puerto"]),
                    Credentials = credenciales,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    EnableSsl = true
                };

                clienteServidor.Send(correo);
                return true;
            }
            catch
            {
                return false;

            }
        }

        public async Task<bool> EnviarCorreo(string email, string passwordTemporal)
        {
            try
            {
                IQueryable<Configuracion> query = await _repositorio.Consultar(c => c.Recurso.Equals("ServicioCorreo"));

                Dictionary<string, string> Config = query.ToDictionary(keySelector: c => c.Propiedad, elementSelector: c => c.Valor);

                var credenciales = new NetworkCredential(Config["correo"], Config["clave"]);

                var correo = new MailMessage()
                {
                    From = new MailAddress(Config["correo"], Config["alias"]),
                    Subject = Asunto,
                    Body = Mensaje,
                    IsBodyHtml = true
                };

                correo.To.Add(new MailAddress(CorreoDestino));

                var clienteServidor = new SmtpClient()
                {
                    Host = Config["host"],
                    Port = int.Parse(Config["puerto"]),
                    Credentials = credenciales,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    EnableSsl = true
                };

                clienteServidor.Send(correo);
                return true;
            }
            catch
            {
                return false;

            }
        }
        async Task<bool> ICorreoService.EnviarCorreoRestauracion(string email, string passwordTemporal)
        {
            try
            {
                string smtpHost = "smtp.gmail.com";
                int smtpPort = 587;
                string emisorEmail = "dpicado433@gmail.com";
                string emisorPassword = "suhbmtqxmvqq efsx";

                MailMessage mensaje = new MailMessage();
                mensaje.From = new MailAddress(emisorEmail, "Sistema de Ventas");
                mensaje.To.Add(correoDestino);
                mensaje.Subject = "Bienvenido a Mi Tienda Online - Credenciales de Acceso";
                mensaje.Body = $"Hola,\n\nSe ha creado tu cuenta exitosamente en el sistema.\n" +
                               $"Tu contraseña temporal es: {passwordTemporaldOEnlace}\n\n" +
                               $"Por favor, ingresa al sistema y cambia tu contraseña por motivos de seguridad.";
                mensaje.IsBodyHtml = false;

                using (SmtpClient smtpClient = new SmtpClient(smtpHost, smtpPort))
                {
                    smtpClient.Credentials = new NetworkCredential(emisorEmail, emisorPassword);
                    smtpClient.EnableSsl = true;
                    smtpClient.Send(mensaje);
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al enviar correo: {ex.Message}");
                return false;
            }
        }

    }
}
