using Aplicacion.DTO;
using Aplicacion.Interfaz;
using Aplicacion.modelos;
using Aplicacion.Repositorio;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplicacion.Servicios
{
    public class OrdenCompraService : IOrdComService
    {
        private readonly OrdComRepositorio _ordenCompraRepositorio;
        private readonly IMapper _mapper;

        public OrdenCompraService(OrdComRepositorio ordenCompraRepository, IMapper mapper)
        {
            _mapper = mapper;
            _ordenCompraRepositorio = ordenCompraRepository;
        }

        public async Task AddAsync(CreateOrdenCompraDTO orden)
        {
            var nuevaOrden = _mapper.Map<Orden_Compra>(orden);

            // Calcular id manualmente
            var todos = await _ordenCompraRepositorio.GetAllasync();
            nuevaOrden.id = todos.Any() ? todos.Max(o => o.id) + 1 : 1;

            // Si no viene fecha_limite_ofertas, usar Fecha_Limite
            if (!nuevaOrden.fecha_limite_ofertas.HasValue)
            {
                nuevaOrden.fecha_limite_ofertas = nuevaOrden.Fecha_Limite;
            }

            await _ordenCompraRepositorio.AddAsync(nuevaOrden);
        }

        public async Task DeleteAsync(int id)
        {
            await _ordenCompraRepositorio.DeletAsync(id);
        }

        public async Task<List<OrdenCompraDTO>> GetAllsync()
        {
            return _mapper.Map<List<OrdenCompraDTO>>(await _ordenCompraRepositorio.GetAllasync());
        }

        public async Task<OrdenCompraDTO> GetByIdAsync(int id)
        {
            return _mapper.Map<OrdenCompraDTO>(await _ordenCompraRepositorio.GetAsync(id));
        }

        public async Task UpdateAsync(UpdateOrdenCompraDTO orden)
        {
            var ordenActualizar = _mapper.Map<Orden_Compra>(orden);

            if (!ordenActualizar.fecha_limite_ofertas.HasValue)
            {
                ordenActualizar.fecha_limite_ofertas = ordenActualizar.Fecha_Limite;
            }

            await _ordenCompraRepositorio.UpdateAsync(ordenActualizar);
        }
    }
}