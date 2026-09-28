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
        private readonly OrdComRepositorio _ordenCompraRepositorio;
        private readonly OferProvRepositorio _ofertaRepositorio;   
        private readonly IMapper _mapper;

        public AdjudicacionService(
            AdjuRepositorio adjuRepository,
            OrdComRepositorio ordenCompraRepositorio,
            OferProvRepositorio ofertaRepositorio,                 
            IMapper mapper)
        {
            _mapper = mapper;
            _adjuRepositorio = adjuRepository;
            _ordenCompraRepositorio = ordenCompraRepositorio;
            _ofertaRepositorio = ofertaRepositorio;                 
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

            // Calcular el id manualmente
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

        // ===== REGLA DE NEGOCIO 4 =====
        // El proveedor adjudicado debe ser el que tenga la oferta con MENOR precio
        public async Task ValidarMenorPrecioAsync(int idPedido, int idProveedor, decimal precioAdjudicado)
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

            // 3. Encontrar la oferta con el menor precio
            var mejorOferta = ofertasDelPedido
                .OrderBy(o => o.Precio)
                .First();

            // 4. Verificar si hay empate en el menor precio
            var ofertasConMenorPrecio = ofertasDelPedido
                .Where(o => o.Precio == mejorOferta.Precio)
                .ToList();

            // 5. Si el proveedor seleccionado NO está entre los de menor precio, lanzar error
            var proveedorEsGanador = ofertasConMenorPrecio
                .Any(o => o.id_Proveedor == idProveedor);

            if (!proveedorEsGanador)
            {
                // Construir lista de proveedores con el menor precio
                var proveedoresMenor = string.Join(", ",
                    ofertasConMenorPrecio.Select(o => $"#{o.id_Proveedor}"));

                throw new InvalidOperationException(
                    $"La adjudicación del pedido #{idPedido} debe ir al proveedor con menor precio " +
                    $"(Q {mejorOferta.Precio:N2} del proveedor {proveedoresMenor}). " +
                    $"El proveedor seleccionado (#{idProveedor}) ofertó Q {precioAdjudicado:N2}."
                );
            }
        }
    }
}