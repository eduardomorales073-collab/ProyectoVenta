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
            await _ordenCompraRepositorio.AddAsync(_mapper.Map<Orden_Compra>(orden));
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
            await _ordenCompraRepositorio.UpdateAsync(_mapper.Map<Orden_Compra>(orden));
        }
    }
}
