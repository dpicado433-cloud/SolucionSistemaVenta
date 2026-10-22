using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SistemaVenta.DAL.DBContext;
using SistemaVenta.DAL.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;


namespace SistemaVenta.DAL.Implementacion
{
    public class GenericRepository<TEntity> : IGenerRepository<TEntity> where TEntity : class
    {

        private readonly DbventaContext _dbConext;

        public GenericRepository(DbventaContext dbContext)
        {
            _dbConext = dbContext;
        }

        public async Task<TEntity> Obtener(Expression<Func<TEntity, bool>> filtro)
        {
            try
            {
                TEntity entidad = await _dbConext.Set<TEntity>().FirstOrDefaultAsync(filtro);
                return entidad;
            }
            catch
            {
                throw;
            }
        }
        public async Task<TEntity> Crear(TEntity entidad)
        {
            try
            {
                _dbConext.Set<TEntity>().Add(entidad);
                await _dbConext.SaveChangesAsync();
                return entidad;
            }
            catch
            {
                throw;
            }
        }
        public async Task<bool> Editar(TEntity entidad)
        {
            try
            {
                _dbConext.Set<TEntity>().Update(entidad);
                await _dbConext.SaveChangesAsync();
                return true;
            }
            catch
            {
                throw;
            }
        }
        public async Task<bool> Eliminar(TEntity entidad)
        {
            try
            {
                _dbConext.Remove(entidad);
                await _dbConext.SaveChangesAsync();
                return true;
            }
            catch
            {
                throw;
            }
        }
        public Task<IQueryable<TEntity>> Consultar(Expression<Func<TEntity, bool>> filtro = null)
        {
            IQueryable<TEntity> queryEntidad = filtro == null ? _dbConext.Set<TEntity>() : _dbConext.Set<TEntity>().Where(filtro);
            return Task.FromResult(queryEntidad);
        }
    }
}
