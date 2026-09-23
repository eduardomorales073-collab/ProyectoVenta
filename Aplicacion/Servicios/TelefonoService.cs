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
    public class TelefonoService : ITelefonoService
    {
        private readonly TelefonoRepositorio _telefonoRepositorio;
        private readonly IMapper _mapper;
        public TelefonoService(TelefonoRepositorio telefonoRepository, IMapper mapper)
        {
            _mapper = mapper;
            _telefonoRepositorio = telefonoRepository ;

        }
        public  async Task AddAsync(CreateTelefonoDTO telefono)
        {
            await _telefonoRepositorio.AddAsync(_mapper.Map<Telefono>(telefono));
        }

        public  async Task DeleteAsync(int id)
        {
            await _telefonoRepositorio.DeletAsync(id);
        }

        public  async Task<List<TelefonoDTO>> GetAllsync()
        {
            return _mapper.Map<List<TelefonoDTO>>(await _telefonoRepositorio.GetAllasync());
        }

        public  async Task<TelefonoDTO> GetByIdAsync(int id)
        {
            return _mapper.Map<TelefonoDTO>(await _telefonoRepositorio.GetAsync(id));
        }

        public  async Task UpdateAsync(UpdateTelefonoDTO telefono)
        {
            await _telefonoRepositorio.UpdateAsync(_mapper.Map<Telefono>(telefono));
        }
    }
}
