using Microsoft.AspNetCore.Mvc;
using System.IO;

using AutoMapper;
using Newtonsoft.Json;
using SistemaVenta.AplicacionWeb.Models.ViewModels;
using SistemaVenta.AplicacionWeb.Utilidades.Response;
using SistemaVenta.BLL.Interfaces;
using SistemaVenta.Entity;
using System.Reflection.Emit;


namespace SistemaVenta.AplicacionWeb.Controllers
{
    public class UsuarioController : Controller
    {

        private readonly IUsuarioService _usuarioServicio;
        private readonly IRolService _rolServicio;
        private readonly IMapper _mapper;
        private readonly IAppWritteService _appWritteService;

        public string UrlPlantillaCorreo { get; private set; }

        public UsuarioController(IUsuarioService usuarioServicio, 
            IRolService rolServicio,
            IMapper mapper,
            IAppWritteService appWritteService
            )
        {
            _usuarioServicio = usuarioServicio;
            _rolServicio = rolServicio;
            _mapper = mapper;
            _appWritteService = appWritteService;

        }


        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ListaRoles()
        {
            List<VMRol> vmListaRoles = _mapper.Map<List<VMRol>>(await _rolServicio.Lista());
            return StatusCode(StatusCodes.Status200OK, vmListaRoles);
        }

        [HttpGet]
        public async Task<IActionResult> Lista()
        {
            List<VMUsuario> vmUsuarioLista = _mapper.Map<List<VMUsuario>>(await _usuarioServicio.Lista());
            return StatusCode(StatusCodes.Status200OK, new { data = vmUsuarioLista });
        }

        [HttpPost]
        public async Task<IActionResult> Crear([FromForm] IFormFile foto, [FromForm] string modelo)
        {
            GenericResponse<VMUsuario> gResponse = new GenericResponse<VMUsuario>();
            string claveTemporal = "Temp" + Guid.NewGuid().ToString("N").Substring(0, 6) + "*1";

            try
            {
                VMUsuario vmUsuario = JsonConvert.DeserializeObject<VMUsuario>(modelo);

                bool creadoEnAppwrite = await _appWritteService.CrearUsuarioAppwrite(
                    vmUsuario.Correo,
                    claveTemporal,
                    vmUsuario.Nombre
                );

                if (!creadoEnAppwrite)
                {
                    gResponse.Estado = false;
                    gResponse.Mensaje = "No se pudo registrar el usuario en Appwrite.";
                    return StatusCode(StatusCodes.Status200OK, gResponse);
                }

                string nombreFoto = "";
                Stream fotoStream = null;

                if (foto != null)
                {
                    string folderDestino = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "img", "usuarios");
                    if (!Directory.Exists(folderDestino)) Directory.CreateDirectory(folderDestino);

                    string nombreEnCodigo = Guid.NewGuid().ToString("N");
                    string extension = Path.GetExtension(foto.FileName);
                    nombreFoto = string.Concat(nombreEnCodigo, extension);
                    string rutaCompleta = Path.Combine(folderDestino, nombreFoto);

                    using (var stream = new FileStream(rutaCompleta, FileMode.Create))
                    {
                        await foto.CopyToAsync(stream);
                    }

                    vmUsuario.UrlFoto = $"/img/usuarios/{nombreFoto}";
                    fotoStream = foto.OpenReadStream();
                }
                string UrlPlantillaCorreo = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Plantillas", "plantilla_correo.html");

                Usuario? usuario_creado = null;
                try
                {
                    usuario_creado = await _usuarioServicio.Crear(
                        _mapper.Map<Usuario>(vmUsuario),
                        fotoStream,
                        nombreFoto,
                        UrlPlantillaCorreo
                    );
                }
                catch (Exception exSmtp)
                {
                    System.Diagnostics.Debug.WriteLine("Aviso SMTP ignorado: " + exSmtp.Message);
                }

                if (usuario_creado != null)
                {
                    vmUsuario = _mapper.Map<VMUsuario>(usuario_creado);
                }

                gResponse.Estado = true;
                gResponse.objeto = vmUsuario;
            }
            catch (Exception ex)
            {
                gResponse.Estado = false;
                gResponse.Mensaje = "Error detallado: " + (ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }

            return StatusCode(StatusCodes.Status200OK, gResponse);
        }

        [HttpPut]
        public async Task<IActionResult> Editar([FromForm] IFormFile foto, [FromForm] string modelo)
        {


            GenericResponse<VMUsuario> gResponse = new GenericResponse<VMUsuario>();

            try
            {
                VMUsuario vmUsuario = JsonConvert.DeserializeObject<VMUsuario>(modelo);

                string nombreFoto = "";
                Stream fotoStream = null;

                if (foto != null)
                {
                    string nombre_en_codigo = Guid.NewGuid().ToString("N");
                    string extension = Path.GetExtension(foto.FileName);
                    nombreFoto = string.Concat(nombre_en_codigo, extension);
                    fotoStream = foto.OpenReadStream();
                }

                Usuario usuario_editado = await _usuarioServicio.Editar(_mapper.Map<Usuario>(vmUsuario), fotoStream, nombreFoto );

                vmUsuario = _mapper.Map<VMUsuario>(usuario_editado);

                gResponse.Estado = true;
                gResponse.objeto = vmUsuario;
            }
            catch (Exception ex)
            {
                gResponse.Estado = true;
                gResponse.Mensaje = ex.Message;
            }

            return StatusCode(StatusCodes.Status200OK, gResponse);

        }
        [HttpDelete]
        public async Task<IActionResult> Eliminar(int IdUsuario) {
            GenericResponse<string> gResponse = new GenericResponse<string>();

            try
            {
                gResponse.Estado = await _usuarioServicio.Eliminar(IdUsuario);
            }
            catch (Exception ex) {
                gResponse.Estado = false;
                gResponse.Mensaje =ex.Message;
            }
            return StatusCode(StatusCodes.Status200OK, gResponse);
        }


    }
}
