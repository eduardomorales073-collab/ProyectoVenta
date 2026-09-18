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
    public class DetallePedidoService : IDetPedidoService
    {
        private readonly DetaPedRepositorio _detaPedRepositorio;
        private readonly IMapper _mapper;
        public DetallePedidoService(DetaPedRepositorio detaPedRepository, IMapper mapper)
        {
            _mapper = mapper;
            _detaPedRepositorio = detaPedRepository;

        }
        public async Task AddAsync(CreateDetallePedidoDTO detalle)
        {
            await _detaPedRepositorio.AddAsync(_mapper.Map<Detalle_Pedido>(detalle));
        }

        public async Task DeleteAsync(int idPedido, int idArticulo)
        {
            await _detaPedRepositorio.DeletAsync(idPedido, idArticulo);
        }

        public async Task<List<DetallePedidoDTO>> GetAllsync()
        {
            return _mapper.Map<List<DetallePedidoDTO>>(await _detaPedRepositorio.GetAllasync());
        }

        public async Task<DetallePedidoDTO> GetByIdAsync(int idPedido, int idArticulo)
        {
            return _mapper.Map<DetallePedidoDTO>(
                await _detaPedRepositorio.GetAsync(idPedido, idArticulo));
        }

        public async Task UpdateAsync(UpdateDetallePedidoDTO detalle)
        {
            await _detaPedRepositorio.UpdateAsync(_mapper.Map<Detalle_Pedido>(detalle));
        }
    }
}
