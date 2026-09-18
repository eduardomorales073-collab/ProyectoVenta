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
    public class DetalleTelefonoService : IDetTelefonoService
    {
        private readonly DetTelRepositorio _detTelRepositorio;
        private readonly IMapper _mapper;
        public DetalleTelefonoService(DetTelRepositorio detTelRepository, IMapper mapper)
        {
            _mapper = mapper;
            _detTelRepositorio = detTelRepository;

        }
        public async Task AddAsync(CreateDetalleTelefonoDTO detalle)
        {
            await _detTelRepositorio.AddAsync(_mapper.Map<Detalle_Telefono>(detalle));
        }

        public async Task DeleteAsync(int idSucursal, int idTelefono)
        {
            await _detTelRepositorio.DeletAsync(idSucursal, idTelefono);
        }

        public async Task<List<DetalleTelefonoDTO>> GetAllsync()
        {
            return _mapper.Map<List<DetalleTelefonoDTO>>(await _detTelRepositorio.GetAllasync());
        }

        public async Task<DetalleTelefonoDTO> GetByIdAsync(int idSucursal, int idTelefono)
        {
            return _mapper.Map<DetalleTelefonoDTO>(
                await _detTelRepositorio.GetAsync(idSucursal, idTelefono));
        }

        public async Task UpdateAsync(UpdateDetalleTelefonoDTO detalle)
        {
            await _detTelRepositorio.UpdateAsync(_mapper.Map<Detalle_Telefono>(detalle));
        }
    }
}
