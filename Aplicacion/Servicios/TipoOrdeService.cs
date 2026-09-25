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
    public class TipoOrdeService : ITipoOrdenService
    {
        private readonly TipoOrRepositorio _tipoOrRepositorio;
        private readonly IMapper _mapper;
        public TipoOrdeService(TipoOrRepositorio tipoOrRepository, IMapper mapper)
        {
            _mapper = mapper;
            _tipoOrRepositorio = tipoOrRepository;

        }
        public async Task AddAsync(CreateTipoOrdenDTO tipoOrden)
        {
            // 1. Mapear el DTO a la entidad
            var nuevoTipo = _mapper.Map<Tipo_Orden>(tipoOrden);

            // 2. Calcular el siguiente id
            var todos = await _tipoOrRepositorio.GetAllasync();
            nuevoTipo.id = todos.Any() ? todos.Max(t => t.id) + 1 : 1;

            // 3. Guardar
            await _tipoOrRepositorio.AddAsync(nuevoTipo);
        }

        public async Task DeleteAsync(int id)
        {
            await _tipoOrRepositorio.DeletAsync(id);
        }

        public async Task<List<TipoOrdenDTO>> GetAllsync()
        {
            return _mapper.Map<List<TipoOrdenDTO>>(await _tipoOrRepositorio.GetAllasync());
        }

        public async Task<TipoOrdenDTO> GetByIdAsync(int id)
        {
            return _mapper.Map<TipoOrdenDTO>(await _tipoOrRepositorio.GetAsync(id));
        }

        public async Task UpdateAsync(UpdateTipoOrdenDTO tipoOrden)
        {
            await _tipoOrRepositorio.UpdateAsync(_mapper.Map<Tipo_Orden>(tipoOrden));
        }
    }
}
