using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Appwrite;
using Appwrite.Services;

namespace SistemaVenta.BLL.Interfaces
{
    public interface IAppWritteService
    {
        Task<string> SubirStorage(Stream StreamArchivo, string CarpetaDestino, string NombreArchivo);
        Task<string> EliminarStorage(string CarpetaDestino, string NombreArchivo);

        Task<bool> CrearUsuarioAppwrite(string correo, string password, string nombre);

    }
}
