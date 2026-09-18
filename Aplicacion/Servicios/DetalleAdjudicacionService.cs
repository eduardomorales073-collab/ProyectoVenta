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
    public class DetalleAdjudicacionService : IDetAdjuService
    {
        private readonly DetaAdjRepositorio _detaAdjRepositorio;
        private readonly IMapper _mapper;
        public DetalleAdjudicacionService(DetaAdjRepositorio detaAdjRepository, IMapper mapper)
        {
            _mapper = mapper;
            _detaAdjRepositorio = detaAdjRepository;

        }
        public async Task AddAsync(CreateDetalleAdjudicacionDTO detalle)
        {
            await _detaAdjRepositorio.AddAsync(_mapper.Map<Detalle_Adjudicacion>(detalle));
        }

        public async Task DeleteAsync(int idAdjudicacion, int idPedido, int idProveedor)
        {
            await _detaAdjRepositorio.DeletAsync(idAdjudicacion, idPedido, idProveedor);
        }

        public async Task<List<DetalleAdjudicacionDTO>> GetAllsync()
        {
            return _mapper.Map<List<DetalleAdjudicacionDTO>>(await _detaAdjRepositorio.GetAllasync());
        }

        public async Task<DetalleAdjudicacionDTO> GetByIdAsync(int idAdjudicacion, int idPedido, int idProveedor)
        {
            return _mapper.Map<DetalleAdjudicacionDTO>(
                await _detaAdjRepositorio.GetAsync(idAdjudicacion, idPedido, idProveedor));
        }

        public async  Task UpdateAsync(UpdateDetalleAdjudicacionDTO detalle)
        {
            await _detaAdjRepositorio.UpdateAsync(_mapper.Map<Detalle_Adjudicacion>(detalle));
        }
    }
}
