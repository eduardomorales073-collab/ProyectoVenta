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
        private readonly OferProvRepositorio _ofertaRepositorio;
        private readonly ProveedorRepositorio _proveedorRepositorio;  // ← NUEVO
        private readonly RelacionRepositorio _relacionRepositorio;    // ← NUEVO
        private readonly IMapper _mapper;

        public DetalleAdjudicacionService(
            DetaAdjRepositorio detaAdjRepository,
            OferProvRepositorio ofertaRepositorio,
            ProveedorRepositorio proveedorRepositorio,               // ← NUEVO
            RelacionRepositorio relacionRepositorio,                 // ← NUEVO
            IMapper mapper)
        {
            _mapper = mapper;
            _detaAdjRepositorio = detaAdjRepository;
            _ofertaRepositorio = ofertaRepositorio;
            _proveedorRepositorio = proveedorRepositorio;            // ← NUEVO
            _relacionRepositorio = relacionRepositorio;              // ← NUEVO
        }

        public async Task AddAsync(CreateDetalleAdjudicacionDTO detalle)
        {
            await ValidarUnicaAdjudicacionAsync(detalle.id_Pedido);
            await ValidarExistenOfertasAsync(detalle.id_Pedido);
            await ValidarMenorPrecioAsync(detalle.id_Pedido, detalle.id_Proveedor, detalle.Precio);

            await _detaAdjRepositorio.AddAsync(_mapper.Map<Detalle_Adjudicacion>(detalle));
        }

        public async Task DeleteAsync(int idAdjudicacion, int idPedido, int idProveedor)
        {
            await _detaAdjRepositorio.DeletAsync(idAdjudicacion, idPedido, idProveedor);
        }

        // ✅ MODIFICADO: Enriquece los detalles con nombre y relaciones del proveedor
        public async Task<List<DetalleAdjudicacionDTO>> GetAllsync()
        {
            var detalles = await _detaAdjRepositorio.GetAllasync();
            var proveedores = await _proveedorRepositorio.GetAllasync();
            var relaciones = await _relacionRepositorio.GetAllasync();

            return detalles.Select(d =>
            {
                var proveedor = proveedores.FirstOrDefault(p => p.id == d.id_Proveedor);

                // Relaciones bidireccionales del proveedor
                var relacionesProveedor = relaciones
                    .Where(r => r.Proveedor1 == d.id_Proveedor || r.Proveedor2 == d.id_Proveedor)
                    .Select(r =>
                    {
                        var idOtroProveedor = r.Proveedor1 == d.id_Proveedor
                            ? r.Proveedor2
                            : r.Proveedor1;

                        var otroProveedor = proveedores.FirstOrDefault(p => p.id == idOtroProveedor);

                        return new ProveedorRelacionInfoDTO(
                            idOtroProveedor,
                            otroProveedor?.Nombre ?? $"Proveedor #{idOtroProveedor}",
                            r.TipoRelacion ?? "Relacionado"
                        );
                    })
                    .ToList();

                return new DetalleAdjudicacionDTO(
                    d.id_adjudicacion,
                    d.id_Pedido,
                    d.id_Proveedor,
                    d.Precio,
                    d.Cantidad,
                    proveedor?.Nombre,
                    relacionesProveedor
                );
            }).ToList();
        }

        public async Task<DetalleAdjudicacionDTO> GetByIdAsync(int idAdjudicacion, int idPedido, int idProveedor)
        {
            var detalle = await _detaAdjRepositorio.GetAsync(idAdjudicacion, idPedido, idProveedor);
            if (detalle == null) return null;

            var proveedores = await _proveedorRepositorio.GetAllasync();
            var relaciones = await _relacionRepositorio.GetAllasync();

            var proveedor = proveedores.FirstOrDefault(p => p.id == detalle.id_Proveedor);

            var relacionesProveedor = relaciones
                .Where(r => r.Proveedor1 == detalle.id_Proveedor || r.Proveedor2 == detalle.id_Proveedor)
                .Select(r =>
                {
                    var idOtroProveedor = r.Proveedor1 == detalle.id_Proveedor
                        ? r.Proveedor2
                        : r.Proveedor1;
                    var otroProveedor = proveedores.FirstOrDefault(p => p.id == idOtroProveedor);

                    return new ProveedorRelacionInfoDTO(
                        idOtroProveedor,
                        otroProveedor?.Nombre ?? $"Proveedor #{idOtroProveedor}",
                        r.TipoRelacion ?? "Relacionado"
                    );
                })
                .ToList();

            return new DetalleAdjudicacionDTO(
                detalle.id_adjudicacion,
                detalle.id_Pedido,
                detalle.id_Proveedor,
                detalle.Precio,
                detalle.Cantidad,
                proveedor?.Nombre,
                relacionesProveedor
            );
        }

        public async Task UpdateAsync(UpdateDetalleAdjudicacionDTO detalle)
        {
            await ValidarExistenOfertasAsync(detalle.id_Pedido);
            await ValidarMenorPrecioAsync(detalle.id_Pedido, detalle.id_Proveedor, detalle.Precio);

            await _detaAdjRepositorio.UpdateAsync(_mapper.Map<Detalle_Adjudicacion>(detalle));
        }

        // ===== VALIDACIONES EXISTENTES =====
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

        private async Task ValidarMenorPrecioAsync(int idPedido, int idProveedor, decimal precioAdjudicado)
        {
            var todasLasOfertas = await _ofertaRepositorio.GetAllasync();
            var ofertasDelPedido = todasLasOfertas
                .Where(o => o.id_Pedido_Interno == idPedido)
                .ToList();

            var menorPrecio = ofertasDelPedido.Min(o => o.Precio);
            var proveedorEsGanador = ofertasDelPedido
                .Any(o => o.id_Proveedor == idProveedor && o.Precio == menorPrecio);

            if (!proveedorEsGanador)
            {
                var proveedoresMenor = string.Join(", ",
                    ofertasDelPedido
                        .Where(o => o.Precio == menorPrecio)
                        .Select(o => $"#{o.id_Proveedor}"));

                var ofertaDelProveedor = ofertasDelPedido
                    .FirstOrDefault(o => o.id_Proveedor == idProveedor);

                string mensaje;
                if (ofertaDelProveedor == null)
                {
                    mensaje = $"El proveedor #{idProveedor} no tiene una oferta registrada para el pedido #{idPedido}. " +
                              $"La adjudicación debe ir al proveedor con menor precio (Q {menorPrecio:N2} de los proveedores {proveedoresMenor}).";
                }
                else
                {
                    mensaje = $"La adjudicación del pedido #{idPedido} debe ir al proveedor con menor precio " +
                              $"(Q {menorPrecio:N2} de los proveedores {proveedoresMenor}). " +
                              $"El proveedor seleccionado (#{idProveedor}) ofertó Q {ofertaDelProveedor.Precio:N2}.";
                }

                throw new InvalidOperationException(mensaje);
            }
        }
    }
}