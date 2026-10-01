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
    public class DepartamentoService : IDepartaentoService
    {
        private readonly DepartaRepositorio _departaRepositorio;
        private readonly SucursalRepositorio _sucursalRepositorio;
        private readonly IMapper _mapper;

        public DepartamentoService(
            DepartaRepositorio departaRepository,
            SucursalRepositorio sucursalRepositorio,
            IMapper mapper)
        {
            _mapper = mapper;
            _departaRepositorio = departaRepository;
            _sucursalRepositorio = sucursalRepositorio;
        }

        public async Task AddAsync(CreateDepartamentoDTO departamento)
        {
            var nuevoDepartamento = _mapper.Map<Departamento>(departamento);
            var todos = await _departaRepositorio.GetAllasync();
            nuevoDepartamento.id = todos.Any() ? todos.Max(d => d.id) + 1 : 1;
            await _departaRepositorio.AddAsync(nuevoDepartamento);
        }

        public async Task DeleteAsync(int id)
        {
            await _departaRepositorio.DeletAsync(id);
        }

        public async Task<List<DepartamentoDTO>> GetAllsync()
        {
            var departamentos = await _departaRepositorio.GetAllasync();
            var sucursales = await _sucursalRepositorio.GetAllasync();

            return departamentos.Select(d =>
            {
                var sucursal = sucursales.FirstOrDefault(s => s.id == d.id_Sucursal);
                return new DepartamentoDTO(
                    d.id,
                    d.Nombre,           
                    d.Descripcion,      
                    d.id_Sucursal,
                    sucursal?.Nombre    
                );
            }).ToList();
        }

        public async Task<DepartamentoDTO> GetByIdAsync(int id)
        {
            var departamento = await _departaRepositorio.GetAsync(id);
            if (departamento == null) return null;

            var sucursales = await _sucursalRepositorio.GetAllasync();
            var sucursal = sucursales.FirstOrDefault(s => s.id == departamento.id_Sucursal);

            return new DepartamentoDTO(
                departamento.id,
                departamento.Nombre,           
                departamento.Descripcion,      
                departamento.id_Sucursal,
                sucursal?.Nombre               
            );
        }

        public async Task UpdateAsync(UpdateDepartamentoDTO departamento)
        {
            await _departaRepositorio.UpdateAsync(_mapper.Map<Departamento>(departamento));
        }
    }
}