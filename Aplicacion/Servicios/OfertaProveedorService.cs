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
        private readonly IMapper _mapper;
        private readonly IPublishEndpoint _publishEndpoint;

        public OfertaProveedorService(
            OferProvRepositorio ofertaProveedorRepository,
            IPublishEndpoint publishEndpoint,
            IMapper mapper)
        {
            _mapper = mapper;
            _ofertaProveedorRepositorio = ofertaProveedorRepository;
            _publishEndpoint = publishEndpoint;
        }

        // ✅ HELPER: Publicar evento SIN bloquear
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

            // ✅ PUBLICAR EVENTO SIN BLOQUEAR
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

            // ✅ PUBLICAR EVENTO SIN BLOQUEAR
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

        public async Task<List<OfertaProveedorDTO>> GetByProveedorAsync(int idProveedor)
        {
            var todas = await _ofertaProveedorRepositorio.GetAllasync();
            var filtradas = todas.Where(o => o.id_Proveedor == idProveedor).ToList();
            return _mapper.Map<List<OfertaProveedorDTO>>(filtradas);
        }
    }
}