using Aplicacion.DTO;
using Aplicacion.Interfaz;
using Aplicacion.modelos;
using Aplicacion.Repositorio;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;                  // ← AÑADIR
using System.Text;
using System.Threading.Tasks;

namespace Aplicacion.Servicios
{
    public class SucursalService : ISucursalService
    {
        private readonly SucursalRepositorio _sucursalRepositorio;
        private readonly IMapper _mapper;

        public SucursalService(SucursalRepositorio sucursalRepository, IMapper mapper)
        {
            _mapper = mapper;
            _sucursalRepositorio = sucursalRepository;
        }

        public async Task AddAsync(CreateSucursalDTO sucursal)
        {
            // 1. Mapear el DTO a la entidad
            var nuevaSucursal = _mapper.Map<Sucursal>(sucursal);

            // 2. Calcular el siguiente id
            var todos = await _sucursalRepositorio.GetAllasync();
            nuevaSucursal.id = todos.Any() ? todos.Max(s => s.id) + 1 : 1;

            // 3. Guardar
            await _sucursalRepositorio.AddAsync(nuevaSucursal);
        }

        public async Task DeleteAsync(int id)
        {
            await _sucursalRepositorio.DeletAsync(id);
        }

        public async Task<List<SucursalDTO>> GetAllsync()
        {
            return _mapper.Map<List<SucursalDTO>>(await _sucursalRepositorio.GetAllasync());
        }

        public async Task<SucursalDTO> GetByIdAsync(int id)
        {
            return _mapper.Map<SucursalDTO>(await _sucursalRepositorio.GetAsync(id));
        }

        public async Task UpdateAsync(UpdateSucursalDTO sucursal)
        {
            await _sucursalRepositorio.UpdateAsync(_mapper.Map<Sucursal>(sucursal));
        }
    }
}