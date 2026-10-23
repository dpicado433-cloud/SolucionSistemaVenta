using Microsoft.AspNetCore.Mvc;
using SistemaVenta.BLL.Interfaces;

namespace SistemaVenta.AplicacionWeb.Controllers
{
    public class ConsultaController : Controller
    {
        public IActionResult EnviarClave(string correo = "", string clave = "")
        {
            ViewData["Correo"] = correo;
            ViewData["Clave"] = clave;
            ViewData["Url"] = $"{this.Request.Scheme}://{this.Request.Host}";

            return View();
        }




        public IActionResult RestablecerContraseña(string clave)
        {
            ViewData["Clave"] = clave;

            return View();
        }
        private readonly IAppWritteService _AppWriteService;

        public ConsultaController(IAppWritteService appWriteService)
        {
            _AppWriteService = appWriteService;
        }

        [HttpGet]
        public IActionResult RestablecerContraseña(string userId, string secret)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(secret))
            {
                ViewBag.Error = "Enlace inválido o expirado.";
                return View();
            }

            ViewBag.UserId = userId;
            ViewBag.Secret = secret;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RestablecerContraseña(string userId, string secret, string nuevaPassword, string confirmarPassword)
        {
            if (nuevaPassword != confirmarPassword)
            {
                ViewBag.Error = "Las contraseñas no coinciden.";
                ViewBag.UserId = userId;
                ViewBag.Secret = secret;
                return View();
            }

            try
            {
                await _AppWriteService.ActualizarPasswordAppwrite(userId, nuevaPassword);

                ViewBag.Exito = "¡Contraseña actualizada con éxito! Ya puedes iniciar sesión.";
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Error: {ex.Message}";
                ViewBag.UserId = userId;
                ViewBag.Secret = secret;
                return View();
            }
        }


    }
}
