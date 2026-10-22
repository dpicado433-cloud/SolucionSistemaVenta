using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SistemaVenta.DAL.DBContext;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;
using SistemaVenta.DAL.Implementacion;
using SistemaVenta.DAL.Interfaces;
using SistemaVenta.BLL.Interfaces;
using SistemaVenta.BLL.Implementacion;
//using SistemaVenta.BLL.Implementacion;
//using SistemaVenta.BLL.Interfaces;


namespace SistemaVenta.IOC

{
    public static class Dependencia
    {
        public static void inyectarDependencia(this IServiceCollection services, IConfiguration configuration) {

            services.AddDbContext<DbventaContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("CadenaSQL"));
            });

            services.AddTransient(typeof(IGenerRepository<>), typeof(GenericRepository<>));
            services.AddScoped<IVentaRepository, VentaRepository>();


            services.AddScoped<ICorreoService, CorreoService>();
            services.AddScoped<IAppWritteService, AppWriteService>();


            services.AddScoped<IUtilidadesService, UtilidadesServices>();
            services.AddScoped<IRolService, RolService>();

            services.AddScoped<IUsuarioService, UsuarioService>();
        }
    }
}
