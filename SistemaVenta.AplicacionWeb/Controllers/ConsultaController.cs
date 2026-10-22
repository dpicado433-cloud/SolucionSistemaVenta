using Microsoft.AspNetCore.Mvc;

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
    }
}
