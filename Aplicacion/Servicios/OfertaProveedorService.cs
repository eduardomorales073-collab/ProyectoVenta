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
            
            IMapper mapper
            )
        {
            _mapper = mapper;
            _ofertaProveedorRepositorio = ofertaProveedorRepository;

            _publishEndpoint = publishEndpoint;

        }

        public async Task AddAsync(CreateOfertaProveedorDTO oferta)
        {
            // ===== VALIDACIÓN: Un proveedor solo puede ofertar UNA VEZ por pedido =====
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

            // Calcular id manualmente
            var nuevaOferta = _mapper.Map<Oferta_Proveedor>(oferta);
            nuevaOferta.id = todas.Any() ? todas.Max(o => o.id) + 1 : 1;

            await _ofertaProveedorRepositorio.AddAsync(nuevaOferta);
            // ===== PUBLICAR EVENTO: OfertaRegistrada =====
            await _publishEndpoint.Publish(new OfertaRegistrada(
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
            await _ofertaProveedorRepositorio.UpdateAsync(_mapper.Map<Oferta_Proveedor>(oferta));
        }

        // ← NUEVO: Obtener solo las ofertas de un proveedor específico
        public async Task<List<OfertaProveedorDTO>> GetByProveedorAsync(int idProveedor)
        {
            var todas = await _ofertaProveedorRepositorio.GetAllasync();
            var filtradas = todas.Where(o => o.id_Proveedor == idProveedor).ToList();
            return _mapper.Map<List<OfertaProveedorDTO>>(filtradas);
        }
    }
}