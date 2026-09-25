using Aplicacion.modelos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Aplicacion.Repositorio
{
    public abstract class UsuarioRepositorio
    {
        public abstract Task<List<Usuarios>> GetAllasync();
        public abstract Task<Usuarios> GetAsync(int id);
        public abstract Task AddAsync(Usuarios usuarios);
        public abstract Task UpdateAsync(Usuarios usuarios);
        public abstract Task DeletAsync(int id);
        public abstract Task<Permisos?> ObtenerPermisosDelUsuarioAsync(int idUsuario);
        public abstract Task<bool> ActualizarPermisosAsync(
    int idPermiso, bool crear, bool leer, bool actualizar, bool borrar, DateTime fecha);
        public abstract Task<bool> ExisteEmailAsync(string email, int? excludeId = null);
    }
}