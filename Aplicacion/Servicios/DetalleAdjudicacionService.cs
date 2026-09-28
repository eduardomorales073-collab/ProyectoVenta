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
        private readonly OferProvRepositorio _ofertaRepositorio;   // ← NUEVO (Regla 4 y 7)
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
            // ===== REGLA 5: Un pedido solo puede tener 1 adjudicación =====
            await ValidarUnicaAdjudicacionAsync(detalle.id_Pedido);

            // ===== REGLA 7: No adjudicar sin ofertas =====
            await ValidarExistenOfertasAsync(detalle.id_Pedido);

            // ===== REGLA 4: Adjudicación al proveedor con menor precio =====
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
            // ===== REGLA 7: No adjudicar sin ofertas =====
            await ValidarExistenOfertasAsync(detalle.id_Pedido);

            // ===== REGLA 4: Adjudicación al proveedor con menor precio =====
            await ValidarMenorPrecioAsync(detalle.id_Pedido, detalle.id_Proveedor, detalle.Precio);

            await _detaAdjRepositorio.UpdateAsync(_mapper.Map<Detalle_Adjudicacion>(detalle));
        }

        // ===== REGLA 5: Un pedido solo puede tener 1 adjudicación =====
        private async Task ValidarUnicaAdjudicacionAsync(int idPedido)
        {
            var todosLosDetalles = await _detaAdjRepositorio.GetAllasync();
            var detalleExistente = todosLosDetalles.FirstOrDefault(d => d.id_Pedido == idPedido);

            if (detalleExistente != null)
            {
                throw new InvalidOperationException(
                    $"El pedido #{idPedido} ya está adjudicado en la adjudicación #{detalleExistente.id_adjudicacion}. " +
                    $"Un pedido solo puede tener una única adjudicación."
                );
            }
        }

        // ===== REGLA 7: No adjudicar sin ofertas =====
        private async Task ValidarExistenOfertasAsync(int idPedido)
        {
            var todasLasOfertas = await _ofertaRepositorio.GetAllasync();
            var ofertasDelPedido = todasLasOfertas
                .Where(o => o.id_Pedido_Interno == idPedido)
                .ToList();

            if (!ofertasDelPedido.Any())
            {
                throw new InvalidOperationException(
                    $"No se puede adjudicar el pedido #{idPedido} porque no tiene ofertas registradas."
                );
            }
        }

        // ===== REGLA 4: Adjudicación al proveedor con menor precio =====
        private async Task ValidarMenorPrecioAsync(int idPedido, int idProveedor, decimal precioAdjudicado)
        {
            var todasLasOfertas = await _ofertaRepositorio.GetAllasync();
            var ofertasDelPedido = todasLasOfertas
                .Where(o => o.id_Pedido_Interno == idPedido)
                .ToList();

            // Encontrar el menor precio
            var menorPrecio = ofertasDelPedido.Min(o => o.Precio);

            // Verificar si el proveedor seleccionado tiene el menor precio (permite empates)
            var proveedorEsGanador = ofertasDelPedido
                .Any(o => o.id_Proveedor == idProveedor && o.Precio == menorPrecio);

            if (!proveedorEsGanador)
            {
                // Obtener TODOS los proveedores que tienen el menor precio
                var proveedoresMenor = string.Join(", ",
                    ofertasDelPedido
                        .Where(o => o.Precio == menorPrecio)
                        .Select(o => $"#{o.id_Proveedor}"));

                // Buscar si el proveedor seleccionado participó (con otro precio)
                var ofertaDelProveedor = ofertasDelPedido
                    .FirstOrDefault(o => o.id_Proveedor == idProveedor);

                string mensaje;
                if (ofertaDelProveedor == null)
                {
                    // Escenario A: Proveedor NO participó
                    mensaje = $"El proveedor #{idProveedor} no tiene una oferta registrada para el pedido #{idPedido}. " +
                              $"La adjudicación debe ir al proveedor con menor precio (Q {menorPrecio:N2} de los proveedores {proveedoresMenor}).";
                }
                else
                {
                    // Escenario B: Proveedor participó pero con mayor precio
                    mensaje = $"La adjudicación del pedido #{idPedido} debe ir al proveedor con menor precio " +
                              $"(Q {menorPrecio:N2} de los proveedores {proveedoresMenor}). " +
                              $"El proveedor seleccionado (#{idProveedor}) ofertó Q {ofertaDelProveedor.Precio:N2}.";
                }

                throw new InvalidOperationException(mensaje);
            }

            // Escenario C: El proveedor tiene el menor precio (o está en empate) → ✅ Permitir
        }
    }
}