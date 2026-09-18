using Aplicacion.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aplicacion.Interfaz
{
    public interface IUsuarioRolesService
    {
        Task<List<UsuariosRolesDTO>> GetAllsync();
        Task<UsuariosRolesDTO> GetByIdAsync(int id);
        Task AddAsync(CreateUsuariosRolesDTO usuarioRole);
        Task DeleteAsync(int id);
        Task UpdateAsync(UpdateUsuariosRolesDTO usuarioRole);
    }
}
