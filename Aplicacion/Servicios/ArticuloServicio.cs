using Aplicacion.DTO;
using Aplicacion.Interfaz;
using AutoMapper;
using Aplicacion.modelos;
using Aplicacion.Repositorio;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aplicacion.Servicios
{
    public class ArticuloServicio : Aplicacion.Interfaz.IArticuloService
    {
        private readonly ArticuRepositorio _articuloRepository;
        private readonly IMapper _mapper;
        public ArticuloServicio(ArticuRepositorio articuloRepository, IMapper mapper ) 
        { 
            _mapper = mapper;
            _articuloRepository = articuloRepository;

        } 
        public async Task AddAsync(CreateArticuloDTO articulo)
        {
            await _articuloRepository.AddAsync(_mapper.Map<Articulo>(articulo));
        }

        public async Task DeleteAsync(int id)
        {
            await _articuloRepository.DeletAsync(id);
        }

        public  async Task<List<ArticuloDTO>> GetAllsync()
        {
            return _mapper.Map<List<ArticuloDTO>>(await _articuloRepository.GetAllasync());
        }

        public async Task<ArticuloDTO> GetByIdAsync(int id)
        {
            return _mapper.Map<ArticuloDTO>(await _articuloRepository.GetAsync(id));
        }

        public async Task UpdateAsync(UpdateActArtDTO articulo)
        {
            await _articuloRepository.UpdateAsync(_mapper.Map<Articulo>(articulo));
        }
    }
}
