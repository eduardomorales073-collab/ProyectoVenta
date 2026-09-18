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
    public class OfertaProveedorService : IOfeProveService
    {
        private readonly OferProvRepositorio _ofertaProveedorRepositorio;
        private readonly IMapper _mapper;
        public OfertaProveedorService(OferProvRepositorio ofertaProveedorRepository, IMapper mapper)
        {
            _mapper = mapper;
            _ofertaProveedorRepositorio = ofertaProveedorRepository;

        }
        public async Task AddAsync(CreateOfertaProveedorDTO oferta)
        {
            await _ofertaProveedorRepositorio.AddAsync(_mapper.Map<Oferta_Proveedor>(oferta));
        }

        public async Task DeleteAsync(int id)
        {
            await _ofertaProveedorRepositorio.DeletAsync(id);
        }

        public async Task<List<OfertaProveedorDTO>> GetAllsync()
        {
            return _mapper.Map<List<OfertaProveedorDTO>>(await _ofertaProveedorRepositorio.GetAllasync());
        }

        public async Task<OfertaProveedorDTO> GetByIdAsync(int id)
        {
            return _mapper.Map<OfertaProveedorDTO>(await _ofertaProveedorRepositorio.GetAsync(id));
        }

        public async Task UpdateAsync(UpdateOfertaProveedorDTO oferta)
        {
            await _ofertaProveedorRepositorio.UpdateAsync(_mapper.Map<Oferta_Proveedor>(oferta));
        }
    }
}
