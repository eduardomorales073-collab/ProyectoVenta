using Aplicacion.DTO;
using Aplicacion.Interfaz;
using Aplicacion.modelos;
using Aplicacion.Repositorio;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Aplicacion.Mensajeria.Eventos;
using MassTransit;

namespace Aplicacion.Servicios
{
    public class OfertaProveedorService : IOfeProveService
    {
        private readonly OferProvRepositorio _ofertaProveedorRepositorio;
        private readonly DetaAdjRepositorio _detalleAdjRepositorio;
        private readonly RelacionRepositorio _relacionRepositorio;
        private readonly ProveedorRepositorio _proveedorRepositorio;
        private readonly IMapper _mapper;
        private readonly IPublishEndpoint _publishEndpoint;

        public OfertaProveedorService(
            OferProvRepositorio ofertaProveedorRepository,
            DetaAdjRepositorio detalleAdjRepositorio,
            RelacionRepositorio relacionRepositorio,
            ProveedorRepositorio proveedorRepositorio, 
            IPublishEndpoint publishEndpoint,
            IMapper mapper)
        {
            _mapper = mapper;
            _ofertaProveedorRepositorio = ofertaProveedorRepository;
            _detalleAdjRepositorio = detalleAdjRepositorio;
            _relacionRepositorio = relacionRepositorio;
            _proveedorRepositorio = proveedorRepositorio;
            _publishEndpoint = publishEndpoint;
        }

        //  HELPER: Publicar evento SIN bloquear
        private void PublicarEvento<T>(T evento) where T : class
        {
            _ = Task.Run(async () =>
            {
                try
                {
                    using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(3));
                    await _publishEndpoint.Publish(evento, cts.Token);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[WARN] No se pudo publicar {typeof(T).Name}: {ex.Message}");
                }
            });
        }

        public async Task AddAsync(CreateOfertaProveedorDTO oferta)
        {
            var todas = await _ofertaProveedorRepositorio.GetAllasync();

            var ofertaExistente = todas.FirstOrDefault(o =>
                o.id_Proveedor == oferta.id_Proveedor &&
                o.id_Pedido_Interno == oferta.id_Pedido_Interno);

            if (ofertaExistente != null)
            {
                throw new InvalidOperationException(
                    $"Ya has registrado una oferta para este pedido. " +
                    $"Si deseas cambiar el precio, edita tu oferta existente (ID #{ofertaExistente.id})."
                );
            }

            var nuevaOferta = _mapper.Map<Oferta_Proveedor>(oferta);
            nuevaOferta.id = todas.Any() ? todas.Max(o => o.id) + 1 : 1;

            await _ofertaProveedorRepositorio.AddAsync(nuevaOferta);

            //  PUBLICAR EVENTO SIN BLOQUEAR
            PublicarEvento(new OfertaRegistrada(
                OfertaId: nuevaOferta.id,
                IdProveedor: nuevaOferta.id_Proveedor,
                IdPedidoInterno: nuevaOferta.id_Pedido_Interno,
                Precio: nuevaOferta.Precio,
                Fecha: DateTime.UtcNow,
                Proveedor: $"Proveedor #{nuevaOferta.id_Proveedor}"
            ));
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
            var existente = await _ofertaProveedorRepositorio.GetAsync(oferta.id);
            if (existente == null)
            {
                throw new InvalidOperationException($"La oferta #{oferta.id} no existe.");
            }

            var precioAnterior = existente.Precio;
            var precioNuevo = oferta.Precio;

            existente.Precio = oferta.Precio;
            existente.Fecha_Oferta = oferta.Fecha_Oferta;
            existente.id_Proveedor = oferta.id_Proveedor;
            existente.id_Pedido_Interno = oferta.id_Pedido_Interno;

            await _ofertaProveedorRepositorio.UpdateAsync(existente);

            //  PUBLICAR EVENTO SIN BLOQUEAR
            PublicarEvento(new OfertaActualizada(
                OfertaId: existente.id,
                IdProveedor: existente.id_Proveedor,
                IdPedidoInterno: existente.id_Pedido_Interno,
                PrecioAnterior: precioAnterior,
                PrecioNuevo: precioNuevo,
                Fecha: DateTime.UtcNow,
                Proveedor: $"Proveedor #{existente.id_Proveedor}"
            ));
        }

        //  MÉTODO MODIFICADO PARA INCLUIR SOCIOS Y ADJUDICACIÓN
        public async Task<List<OfertaProveedorDTO>> GetByProveedorAsync(int idProveedor)
        {
            var todas = await _ofertaProveedorRepositorio.GetAllasync();
            var misOfertas = todas.Where(o => o.id_Proveedor == idProveedor).ToList();

            var detallesAdj = await _detalleAdjRepositorio.GetAllasync();
            var relaciones = await _relacionRepositorio.GetAllasync();
            var proveedores = await _proveedorRepositorio.GetAllasync();

            var idsRelacionados = relaciones
                .Where(r => r.Proveedor1 == idProveedor || r.Proveedor2 == idProveedor)
                .Select(r => r.Proveedor1 == idProveedor ? r.Proveedor2 : r.Proveedor1)
                .ToHashSet();

            var resultado = new List<OfertaProveedorDTO>();

            foreach (var oferta in misOfertas)
            {
                // Mapeo base
                var dto = _mapper.Map<OfertaProveedorDTO>(oferta);

                // Validar si está adjudicado
                dto.PedidoAdjudicado = detallesAdj.Any(d => d.id_Pedido == oferta.id_Pedido_Interno);

                // Buscar socios que ofertaron en el mismo pedido
                var ofertasDelPedido = todas.Where(o => o.id_Pedido_Interno == oferta.id_Pedido_Interno).ToList();
                var ofertasRelacionados = ofertasDelPedido
                    .Where(o => idsRelacionados.Contains(o.id_Proveedor))
                    .ToList();

                dto.ProveedoresRelacionadosQueOfertaron = ofertasRelacionados.Select(o =>
                {
                    var prov = proveedores.FirstOrDefault(pr => pr.id == o.id_Proveedor);
                    var relacion = relaciones.FirstOrDefault(r =>
                        (r.Proveedor1 == idProveedor && r.Proveedor2 == o.id_Proveedor) ||
                        (r.Proveedor2 == idProveedor && r.Proveedor1 == o.id_Proveedor)
                    );

                    return new ProveedorRelacionadoOfertaDTO
                    {
                        NombreProveedor = prov?.Nombre ?? $"Proveedor #{o.id_Proveedor}",
                        TipoRelacion = relacion?.TipoRelacion ?? "Relacionado",
                        PrecioOfertado = o.Precio
                    };
                }).ToList();

                resultado.Add(dto);
            }

            return resultado.OrderByDescending(o => o.id).ToList();
        }
    }
}