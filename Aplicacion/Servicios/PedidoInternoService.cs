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
    public class PedidoInternoService : IPedIntService
    {
        private readonly PedIntRepositorio _pedidoInternoRepositorio;
        private readonly IMapper _mapper;
        public PedidoInternoService(PedIntRepositorio pedidoInternoRepository, IMapper mapper)
        {
            _mapper = mapper;
            _pedidoInternoRepositorio = pedidoInternoRepository;

        }
        public async Task AddAsync(CreatePedidoInternoDTO pedido)
        {
            await _pedidoInternoRepositorio.AddAsync(_mapper.Map<Pedido_Interno>(pedido));
        }

        public async  Task DeleteAsync(int id)
        {
            await _pedidoInternoRepositorio.DeletAsync(id);
        }

        public async Task<List<PedidoInternoDTO>> GetAllsync()
        {
            return _mapper.Map<List<PedidoInternoDTO>>(await _pedidoInternoRepositorio.GetAllasync());
        
        }

        public async Task<PedidoInternoDTO> GetByIdAsync(int id)
        {
            return _mapper.Map<PedidoInternoDTO>(await _pedidoInternoRepositorio.GetAsync(id));
        }

        public async Task UpdateAsync(UpdatePedidoInternoDTO pedido)
        {
            await _pedidoInternoRepositorio.UpdateAsync(_mapper.Map<Pedido_Interno>(pedido));
        }
    }
}
