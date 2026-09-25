using Aplicacion.modelos;
using Aplicacion.Repositorio;
using Infraestructura.Datos;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Infraestructura.Repositorio
{
    public class UsuarRepositorio : UsuarioRepositorio
    {
        private readonly AplicacionDBContexto _context;

        public UsuarRepositorio(AplicacionDBContexto context)
        {
            _context = context;
        }

        public override async Task AddAsync(Usuarios usuarios)
        {
            await _context.Usuarios.AddAsync(usuarios);
            await _context.SaveChangesAsync();
        }

        public override async Task DeletAsync(int id)
        {
            var usuarios = await _context.Usuarios.FindAsync(id);
            if (usuarios != null)
            {
                _context.Usuarios.Remove(usuarios);
                await _context.SaveChangesAsync();
            }
        }

        public override async Task<List<Usuarios>> GetAllasync()
        {
            return await _context.Usuarios.ToListAsync();
        }

        public override async Task<Usuarios> GetAsync(int id)
        {
            return await _context.Usuarios.FindAsync(id);
        }

        public override async Task UpdateAsync(Usuarios usuarios)
        {
            _context.Usuarios.Update(usuarios);
            await _context.SaveChangesAsync();
        }

        // ← NUEVO MÉTODO
        public override async Task<Permisos?> ObtenerPermisosDelUsuarioAsync(int idUsuario)
        {
            // 1. Obtener el usuario
            var usuario = await _context.Usuarios.FindAsync(idUsuario);
            if (usuario == null) return null;

            // 2. Obtener el Rol_Permiso del rol del usuario
            var rolPermiso = await _context.Rol_Permiso
                .FirstOrDefaultAsync(rp => rp.id_Rol == usuario.id_Rol);
            if (rolPermiso == null) return null;

            // 3. Obtener los permisos CRUD
            var permisos = await _context.Permisos
                .FirstOrDefaultAsync(p => p.id == rolPermiso.id_Permiso);

            return permisos;
        }
        public override async Task<bool> ActualizarPermisosAsync(
    int idPermiso, bool crear, bool leer, bool actualizar, bool borrar, DateTime fecha)
        {
            // 1. Buscar el registro Permisos por su id
            var permisos = await _context.Permisos.FindAsync(idPermiso);
            if (permisos == null) return false;

            // 2. Actualizar
            permisos.Crear = crear;
            permisos.Leer = leer;
            permisos.Actualizar = actualizar;
            permisos.Borrar = borrar;
            permisos.Fecha = fecha;

            await _context.SaveChangesAsync();
            return true;
        }

        public override async Task<bool> ExisteEmailAsync(string email, int? excludeId = null)
        {
            var query = _context.Usuarios.Where(u => u.email == email);
            if (excludeId.HasValue)
                query = query.Where(u => u.id != excludeId.Value);
            return await query.AnyAsync();
        }

    }
}