using Aplicacion.DTO;
using Aplicacion.Interfaz;
using Aplicacion.modelos;
using Aplicacion.Repositorio;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Aplicacion.Servicios
{
    public class RolesService : IRolesService
    {
        private readonly RolesRepositorio _rolesRepositorio;
        private readonly IMapper _mapper;
        public RolesService(RolesRepositorio rolesRepository, IMapper mapper)
        {
            _mapper = mapper;
            _rolesRepositorio = rolesRepository;

        }
        public async Task AddAsync(CreateRolesDTO roles)
        {
            // 1. Mapear el DTO a la entidad
            var nuevoRol = _mapper.Map<Roles>(roles);

            // 2. Calcular el siguiente id
            var todos = await _rolesRepositorio.GetAllasync();
            nuevoRol.id = todos.Any() ? todos.Max(r => r.id) + 1 : 1;

            // 3. Guardar
            await _rolesRepositorio.AddAsync(nuevoRol);
        }

        public async Task DeleteAsync(int id)
        {
            await _rolesRepositorio.DeletAsync(id);
        }

        public async Task<List<RolesDTO>> GetAllsync()
        {
            return _mapper.Map<List<RolesDTO>>(await _rolesRepositorio.GetAllasync());
        }

        public async Task<RolesDTO> GetByIdAsync(int id)
        {
            return _mapper.Map<RolesDTO>(await _rolesRepositorio.GetAsync(id));
        }

        public async Task UpdateAsync(UpdateRolesDTO roles)
        {
            await _rolesRepositorio.UpdateAsync(_mapper.Map<Roles>(roles));
        }
    }
}
