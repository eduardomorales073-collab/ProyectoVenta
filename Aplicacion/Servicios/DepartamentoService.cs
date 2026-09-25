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
        private readonly IMapper _mapper;
        public DepartamentoService(DepartaRepositorio departaRepository, IMapper mapper)
        {
            _mapper = mapper;
            _departaRepositorio = departaRepository;

        }
        public async Task AddAsync(CreateDepartamentoDTO departamento)
        {
            // 1. Mapear el DTO a la entidad
            var nuevoDepartamento = _mapper.Map<Departamento>(departamento);

            // 2. Calcular el siguiente id
            var todos = await _departaRepositorio.GetAllasync();
            nuevoDepartamento.id = todos.Any() ? todos.Max(d => d.id) + 1 : 1;

            // 3. Guardar
            await _departaRepositorio.AddAsync(nuevoDepartamento);
        }

        public async Task DeleteAsync(int id)
        {
            await _departaRepositorio.DeletAsync(id);
        }

        public async Task<List<DepartamentoDTO>> GetAllsync()
        {
            return _mapper.Map<List<DepartamentoDTO>>(await _departaRepositorio.GetAllasync());
        }

        public async Task<DepartamentoDTO> GetByIdAsync(int id)
        {
            return _mapper.Map<DepartamentoDTO>(await _departaRepositorio.GetAsync(id));
        }

        public async Task UpdateAsync(UpdateDepartamentoDTO departamento)
        {
            await _departaRepositorio.UpdateAsync(_mapper.Map<Departamento>(departamento));
        }
    }
}
