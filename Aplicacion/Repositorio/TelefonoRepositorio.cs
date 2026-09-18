using Aplicacion.modelos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aplicacion.Repositorio
{
    public interface TelefonoRepositorio
    {
        Task<List<Telefono>> GetAllasync();
        Task<Telefono> GetAsync(int id);
        Task DeletAsync(int id);
        Task AddAsync(Telefono telefono);
        Task UpdateAsync(Telefono telefono);
    }
}
