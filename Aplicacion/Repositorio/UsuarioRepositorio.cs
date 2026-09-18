using Aplicacion.modelos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aplicacion.Repositorio
{
    public interface UsuarioRepositorio
    {
        Task<List<Usuarios>> GetAllasync();
        Task<Usuarios> GetAsync(int id);
        Task DeletAsync(int id);
        Task AddAsync(Usuarios usuarios);
        Task UpdateAsync(Usuarios usuarios);

    }
}
