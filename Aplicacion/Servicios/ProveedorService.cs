using Aplicacion.DTO;
using Aplicacion.Interfaz;
using Aplicacion.modelos;
using Aplicacion.Repositorio;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aplicacion.Servicios
{
    public class ProveedorService : IProveedorService
    {
        private readonly ProveedorRepositorio _proveedorRepositorio;
        private readonly IMapper _mapper;
        public ProveedorService(ProveedorRepositorio proveedorRepository, IMapper mapper)
        {
            _mapper = mapper;
            _proveedorRepositorio = proveedorRepository;

        }
        public async Task AddAsync(CreateProveedorDTO proveedor)
        {
            await _proveedorRepositorio.AddAsync(_mapper.Map<Proveedor>(proveedor));
        }

        public async Task DeleteAsync(int id)
        {
            await _proveedorRepositorio.DeletAsync(id);
        }

        public async Task<List<ProveedorDTO>> GetAllsync()
        {
            return _mapper.Map<List<ProveedorDTO>>(await _proveedorRepositorio.GetAllasync());
        }

        public async Task<ProveedorDTO> GetByIdAsync(int id)
        {
            return _mapper.Map<ProveedorDTO>(await _proveedorRepositorio.GetAsync(id));
        }

        public async Task UpdateAsync(UpdateProveedorDTO proveedor)
        {
            await _proveedorRepositorio.UpdateAsync(_mapper.Map<Proveedor>(proveedor));
        }
    }
}
