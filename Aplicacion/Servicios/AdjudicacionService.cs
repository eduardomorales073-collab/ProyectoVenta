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
    public class AdjudicacionService : IAdjudicacionService
    {
        private readonly AdjuRepositorio _adjuRepositorio;
        private readonly OrdComRepositorio _ordenCompraRepositorio;  // ← NUEVO
        private readonly IMapper _mapper;

        public AdjudicacionService(
            AdjuRepositorio adjuRepository,
            OrdComRepositorio ordenCompraRepositorio,  // ← NUEVO
            IMapper mapper)
        {
            _mapper = mapper;
            _adjuRepositorio = adjuRepository;
            _ordenCompraRepositorio = ordenCompraRepositorio;  // ← NUEVO
        }

        public async Task AddAsync(CreateAdjudicacionDTO adjudicacion)
        {
            // ===== REGLA DE NEGOCIO 2 =====
            // La fecha de resolución no puede ser anterior a la fecha de creación de la orden
            var orden = await _ordenCompraRepositorio.GetAsync(adjudicacion.Orden_Compra);

            if (orden == null)
            {
                throw new InvalidOperationException(
                    $"La orden de compra #{adjudicacion.Orden_Compra} no existe."
                );
            }

            if (adjudicacion.Fecha_Resolucion < orden.Fecha_Creacion)
            {
                throw new InvalidOperationException(
                    $"La fecha de resolución ({adjudicacion.Fecha_Resolucion:dd/MM/yyyy}) " +
                    $"no puede ser anterior a la fecha de creación de la orden " +
                    $"#{orden.id} ({orden.Fecha_Creacion:dd/MM/yyyy})."
                );
            }

            // Calcular el id manualmente (porque la BD no tiene IDENTITY)
            var todas = await _adjuRepositorio.GetAllasync();
            var nuevaAdjudicacion = _mapper.Map<Adjudicacion>(adjudicacion);
            nuevaAdjudicacion.id = todas.Any() ? todas.Max(a => a.id) + 1 : 1;

            await _adjuRepositorio.AddAsync(nuevaAdjudicacion);
        }

        public async Task DeleteAsync(int id)
        {
            await _adjuRepositorio.DeletAsync(id);
        }

        public async Task<List<AdjudicacionDTO>> GetAllsync()
        {
            return _mapper.Map<List<AdjudicacionDTO>>(await _adjuRepositorio.GetAllasync());
        }

        public async Task<AdjudicacionDTO> GetByIdAsync(int id)
        {
            return _mapper.Map<AdjudicacionDTO>(await _adjuRepositorio.GetAsync(id));
        }

        public async Task UpdateAsync(UpdateAdjudicacionDTO adjudicacion)
        {
            // ===== REGLA DE NEGOCIO 2 (también al actualizar) =====
            var orden = await _ordenCompraRepositorio.GetAsync(adjudicacion.Orden_Compra);

            if (orden == null)
            {
                throw new InvalidOperationException(
                    $"La orden de compra #{adjudicacion.Orden_Compra} no existe."
                );
            }

            if (adjudicacion.Fecha_Resolucion < orden.Fecha_Creacion)
            {
                throw new InvalidOperationException(
                    $"La fecha de resolución ({adjudicacion.Fecha_Resolucion:dd/MM/yyyy}) " +
                    $"no puede ser anterior a la fecha de creación de la orden " +
                    $"#{orden.id} ({orden.Fecha_Creacion:dd/MM/yyyy})."
                );
            }

            await _adjuRepositorio.UpdateAsync(_mapper.Map<Adjudicacion>(adjudicacion));
        }
    }
}