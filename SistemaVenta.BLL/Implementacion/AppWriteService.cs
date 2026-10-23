using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Appwrite;
using Appwrite.Models;
using Appwrite.Services;
using SistemaVenta.BLL.Interfaces;
using SistemaVenta.DAL.Interfaces;
using SistemaVenta.Entity;

namespace SistemaVenta.BLL.Implementacion
{
    public class AppWriteService : IAppWritteService
    {
        private readonly IGenerRepository<Configuracion> _repositorio;

        public AppWriteService(IGenerRepository<Configuracion> repositorio)
        {
            _repositorio = repositorio;
        }

        public async Task<bool> CrearUsuarioAppwrite(string correo, string password, string nombre)
        {
            try
            {
                IQueryable<Configuracion> query = await _repositorio.Consultar(c => c.Recurso.Equals("Appwrite_Storage"));
                Dictionary<string, string> Config = query.ToDictionary(c => c.Propiedad, c => c.Valor);

                var client = new Client()
                    .SetEndpoint(Config["endpoint"])
                    .SetProject(Config["project_id"])
                    .SetKey(Config["api_key"]);

                var users = new Users(client);

                var nuevoUsuario = await users.Create(
                    userId: ID.Unique(),
                    email: correo,
                    password: password,
                    name: nombre
                );

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al crear usuario en Appwrite: {ex.Message}");
            }
        }

        public async Task<string> EliminarStorage(string CarpetaDestino, string NombreArchivo)
        {
            try
            {
                IQueryable<Configuracion> query = await _repositorio.Consultar(c => c.Recurso.Equals("Appwrite_Storage"));
                Dictionary<string, string> Config = query.ToDictionary(keySelector: c => c.Propiedad, elementSelector: c => c.Valor);

                var client = new Client()
                .SetEndpoint(Config["endpoint"])
                .SetProject(Config["project_id"]);

                var account = new Appwrite.Services.Account(client);
                await account.CreateEmailPasswordSession(
                    email: Config["email"],
                    password: Config["clave"]
                );

                var storage = new Storage(client);

                await storage.DeleteFile(
                    bucketId: CarpetaDestino,
                    fileId: NombreArchivo
                );

                return "Eliminado con éxito";
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al eliminar: {ex.Message}");
            }
        }

        public async Task<string> SubirStorage(Stream StreamArchivo, string CarpetaDestino, string NombreArchivo)
        {
            string UrlImagen = "";

            try
            {
                IQueryable<Configuracion> query = await _repositorio.Consultar(c => c.Recurso.Equals("Appwrite_Storage"));
                Dictionary<string, string> Config = query.ToDictionary(keySelector: c => c.Propiedad, elementSelector: c => c.Valor);

                var client = new Client()
                    .SetEndpoint(Config["endpoint"])
                    .SetProject(Config["project_id"]);

                var account = new Appwrite.Services.Account(client);
                await account.CreateEmailPasswordSession(
                    email: Config["email"],
                    password: Config["clave"]
                );

                var storage = new Storage(client);
                var fileResult = await storage.CreateFile(
                    bucketId: CarpetaDestino,
                    fileId: ID.Unique(),
                    file: InputFile.FromStream(StreamArchivo, NombreArchivo, mimeType: "image/jpeg")
                );

                var fileUrl = storage.GetFileView(
                    bucketId: CarpetaDestino,
                    fileId: fileResult.Id
                );

                UrlImagen = fileUrl.ToString();
            }
            catch (Exception ex)
            {
                UrlImagen = "";
                throw new Exception($"Error al subir archivo: {ex.Message}");
            }

            return UrlImagen;
        }
        public async Task<bool> ActualizarPasswordAppwrite(string userId, string nuevaPassword)
        {
            try
            {
                IQueryable<Configuracion> query = await _repositorio.Consultar(c => c.Recurso.Equals("Appwrite_Storage"));
                Dictionary<string, string> Config = query.ToDictionary(c => c.Propiedad, c => c.Valor);

                var client = new Client()
                    .SetEndpoint(Config["endpoint"])
                    .SetProject(Config["project_id"])
                    .SetKey(Config["api_key"]);

                var users = new Users(client);

                await users.UpdatePassword(
                    userId: userId,
                    password: nuevaPassword
                );

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al actualizar contraseña en Appwrite: {ex.Message}");
            }
        }

        Task IAppWritteService.ActualizarPasswordAppwrite(string userId, string nuevaPassword)
        {
            return ActualizarPasswordAppwrite(userId, nuevaPassword);
        }
    }
}
