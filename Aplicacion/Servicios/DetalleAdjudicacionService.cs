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
    public class DetalleAdjudicacionService : IDetAdjuService
    {
        private readonly DetaAdjRepositorio _detaAdjRepositorio;
        private readonly OferProvRepositorio _ofertaRepositorio;   // ← NUEVO
        private readonly IMapper _mapper;

        public DetalleAdjudicacionService(
            DetaAdjRepositorio detaAdjRepository,
            OferProvRepositorio ofertaRepositorio,                  // ← NUEVO
            IMapper mapper)
        {
            _mapper = mapper;
            _detaAdjRepositorio = detaAdjRepository;
            _ofertaRepositorio = ofertaRepositorio;                 // ← NUEVO
        }

        public async Task AddAsync(CreateDetalleAdjudicacionDTO detalle)
        {
            // ===== REGLA DE NEGOCIO 4 =====
            // La adjudicación debe ir al proveedor con MENOR precio
            await ValidarMenorPrecioAsync(detalle.id_Pedido, detalle.id_Proveedor, detalle.Precio);

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

        public async Task UpdateAsync(UpdateDetalleAdjudicacionDTO detalle)
        {
            // ===== REGLA DE NEGOCIO 4 (también al actualizar) =====
            await ValidarMenorPrecioAsync(detalle.id_Pedido, detalle.id_Proveedor, detalle.Precio);

            await _detaAdjRepositorio.UpdateAsync(_mapper.Map<Detalle_Adjudicacion>(detalle));
        }

        // ===== REGLA DE NEGOCIO 4: Validar menor precio =====
        private async Task ValidarMenorPrecioAsync(int idPedido, int idProveedor, decimal precioAdjudicado)
        {
            // 1. Obtener todas las ofertas del pedido
            var todasLasOfertas = await _ofertaRepositorio.GetAllasync();
            var ofertasDelPedido = todasLasOfertas
                .Where(o => o.id_Pedido_Interno == idPedido)
                .ToList();

            // 2. Validar que existan ofertas
            if (!ofertasDelPedido.Any())
            {
                throw new InvalidOperationException(
                    $"No se puede adjudicar el pedido #{idPedido} porque no tiene ofertas registradas."
                );
            }

            // 3. Encontrar el menor precio
            var menorPrecio = ofertasDelPedido.Min(o => o.Precio);

            // 4. Verificar si el proveedor seleccionado tiene el menor precio
            var proveedorEsGanador = ofertasDelPedido
                .Any(o => o.id_Proveedor == idProveedor && o.Precio == menorPrecio);

            if (!proveedorEsGanador)
            {
                // Obtener info del proveedor que SÍ tiene el menor precio
                var mejorOferta = ofertasDelPedido.First(o => o.Precio == menorPrecio);

                throw new InvalidOperationException(
                    $"La adjudicación del pedido #{idPedido} debe ir al proveedor con menor precio " +
                    $"(Q {menorPrecio:N2} del proveedor #{mejorOferta.id_Proveedor}). " +
                    $"El proveedor seleccionado (#{idProveedor}) ofertó Q {precioAdjudicado:N2}."
                );
            }
        }
    }
}