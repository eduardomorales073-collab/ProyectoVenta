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
    public class OrdenCompraService : IOrdComService
    {
        private readonly OrdComRepositorio _ordenCompraRepositorio;
        private readonly PedIntRepositorio _pedidoRepositorio;
        private readonly DetaAdjRepositorio _detalleAdjRepositorio;
        private readonly DepartaRepositorio _departamentoRepositorio;
        private readonly SucursalRepositorio _sucursalRepositorio;       // ← NUEVO
        private readonly UsuarioRepositorio _usuarioRepositorio;          // ← NUEVO
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly IMapper _mapper;

        public OrdenCompraService(
            OrdComRepositorio ordenCompraRepository,
            PedIntRepositorio pedidoRepositorio,
            DetaAdjRepositorio detalleAdjRepositorio,
            DepartaRepositorio departamentoRepositorio,
            SucursalRepositorio sucursalRepositorio,                      // ← NUEVO
            UsuarioRepositorio usuarioRepositorio,                        // ← NUEVO
            IPublishEndpoint publishEndpoint,
            IMapper mapper)
        {
            _mapper = mapper;
            _publishEndpoint = publishEndpoint;
            _ordenCompraRepositorio = ordenCompraRepository;
            _pedidoRepositorio = pedidoRepositorio;
            _detalleAdjRepositorio = detalleAdjRepositorio;
            _departamentoRepositorio = departamentoRepositorio;
            _sucursalRepositorio = sucursalRepositorio;                   // ← NUEVO
            _usuarioRepositorio = usuarioRepositorio;                     // ← NUEVO
        }

        public async Task AddAsync(CreateOrdenCompraDTO orden, int idUsuario, int idDepartamento)
        {
            var nuevaOrden = _mapper.Map<Orden_Compra>(orden);

            var todos = await _ordenCompraRepositorio.GetAllasync();
            nuevaOrden.id = todos.Any() ? todos.Max(o => o.id) + 1 : 1;

            if (!nuevaOrden.fecha_limite_ofertas.HasValue)
            {
                nuevaOrden.fecha_limite_ofertas = nuevaOrden.Fecha_Limite;
            }

            nuevaOrden.id_UsuarioCreador = idUsuario;
            nuevaOrden.Estado = "Borrador";

            await _ordenCompraRepositorio.AddAsync(nuevaOrden);

            await _publishEndpoint.Publish(new OrdenCreada(
                OrdenId: nuevaOrden.id,
                Descripcion: nuevaOrden.Descripcion,
                Fecha_Creacion: nuevaOrden.Fecha_Creacion,
                Fecha_Limite: nuevaOrden.Fecha_Limite,
                Tipo_Orden: nuevaOrden.Tipo_Orden
            ));
        }

        public async Task DeleteAsync(int id)
        {
            await _ordenCompraRepositorio.DeletAsync(id);
        }

        public async Task<List<OrdenCompraDTO>> GetAllsync()
        {
            return _mapper.Map<List<OrdenCompraDTO>>(await _ordenCompraRepositorio.GetAllasync());
        }

        public async Task<OrdenCompraDTO> GetByIdAsync(int id)
        {
            return _mapper.Map<OrdenCompraDTO>(await _ordenCompraRepositorio.GetAsync(id));
        }

        public async Task UpdateAsync(UpdateOrdenCompraDTO orden, int idUsuario, string rol)
        {
            // ✅ Obtener la entidad YA RASTREADA (no crear nueva)
            var existente = await _ordenCompraRepositorio.GetAsync(orden.id);
            if (existente == null)
                throw new InvalidOperationException($"La orden #{orden.id} no existe.");

            // ✅ Actualizar los campos de la entidad existente (NO mapear)
            existente.Descripcion = orden.Descripcion;
            existente.Fecha_Limite = orden.Fecha_Limite;
            existente.Fecha_Creacion = orden.Fecha_Creacion;
            existente.Tipo_Orden = orden.Tipo_Orden;

            // Preservar campos que no vienen en el DTO
            // (id_UsuarioCreador y Estado ya están en la entidad existente)

            // Si el DTO no trae fecha_limite_ofertas, usar Fecha_Limite
            if (!existente.fecha_limite_ofertas.HasValue)
            {
                existente.fecha_limite_ofertas = existente.Fecha_Limite;
            }

            await _ordenCompraRepositorio.UpdateAsync(existente);
        }

        public async Task<List<OrdenCompraConContadoresDTO>> GetConContadoresAsync(
            int idUsuario, string rol, int? idDepartamento)
        {
            var ordenes = await _ordenCompraRepositorio.GetAllasync();
            var pedidos = await _pedidoRepositorio.GetAllasync();
            var detallesAdj = await _detalleAdjRepositorio.GetAllasync();
            var departamentos = await _departamentoRepositorio.GetAllasync();
            var sucursales = await _sucursalRepositorio.GetAllasync();       // ← NUEVO
            var usuarios = await _usuarioRepositorio.GetAllasync();           // ← NUEVO

            // ===== FILTRAR POR ROL =====
            IEnumerable<Orden_Compra> ordenesFiltradas = ordenes;

            if (rol == "CreadorPedidos")
            {
                ordenesFiltradas = ordenes.Where(o => o.id_UsuarioCreador == idUsuario);
            }
            // Gestor: las de su sucursal (con o sin pedidos) + las que él creó
            else if (rol == "GestorCompras")
            {
                if (idDepartamento.HasValue)
                {
                    var deptoUsuario = departamentos.FirstOrDefault(d => d.id == idDepartamento.Value);
                    if (deptoUsuario != null)
                    {
                        var idSucursalUsuario = deptoUsuario.id_Sucursal;

                        // Departamentos de la misma sucursal
                        var idsDepartamentosDeLaSucursal = departamentos
                            .Where(d => d.id_Sucursal == idSucursalUsuario)
                            .Select(d => d.id)
                            .ToHashSet();

                        // IDs de usuarios cuyo departamento es de la sucursal
                        var idsUsuariosDeLaSucursal = usuarios
                            .Where(u => u.id_Departamento.HasValue
                                     && idsDepartamentosDeLaSucursal.Contains(u.id_Departamento.Value))
                            .Select(u => u.id)
                            .ToHashSet();

                        // ✅ Filtrar por CREADOR de la orden (no por pedidos)
                        // El creador debe pertenecer a un departamento de la sucursal
                        ordenesFiltradas = ordenes.Where(o =>
                            o.id_UsuarioCreador.HasValue
                            && idsUsuariosDeLaSucursal.Contains(o.id_UsuarioCreador.Value));
                    }
                }
            }
            else if (rol == "AdministradorProveedor")
            {
                ordenesFiltradas = ordenes.Where(o => o.Estado == "Publicada" || o.Estado == "Adjudicada");
            }
            // Admin y Auditor: todas

            var idsPedidosAdjudicados = detallesAdj
                .Select(d => d.id_Pedido)
                .ToHashSet();

            return ordenesFiltradas
                .OrderByDescending(o => o.id)
                .Select(o =>
                {
                    var pedidosDeLaOrden = pedidos
                        .Where(p => p.id_OrdenCompra == o.id)
                        .ToList();

                    var totalPedidos = pedidosDeLaOrden.Count;
                    var pedidosAdjudicados = pedidosDeLaOrden
                        .Count(p => idsPedidosAdjudicados.Contains(p.id));
                    var pedidosPendientes = totalPedidos - pedidosAdjudicados;

                    // ✅ CALCULAR SUCURSAL Y DEPARTAMENTO DEL CREADOR
                    string? nombreSucursal = null;
                    string? nombreDepartamento = null;

                    if (o.id_UsuarioCreador.HasValue)
                    {
                        var usuarioCreador = usuarios.FirstOrDefault(u => u.id == o.id_UsuarioCreador.Value);
                        if (usuarioCreador != null && usuarioCreador.id_Departamento.HasValue)
                        {
                            var depto = departamentos.FirstOrDefault(d => d.id == usuarioCreador.id_Departamento.Value);
                            if (depto != null)
                            {
                                nombreDepartamento = depto.Nombre;
                                var sucursal = sucursales.FirstOrDefault(s => s.id == depto.id_Sucursal);
                                nombreSucursal = sucursal?.Nombre;
                            }
                        }
                    }

                    return new OrdenCompraConContadoresDTO(
                        o.id,
                        o.Descripcion,
                        o.Fecha_Creacion,
                        o.Fecha_Limite,
                        o.fecha_limite_ofertas,
                        o.Tipo_Orden,
                        totalPedidos,
                        pedidosAdjudicados,
                        pedidosPendientes,
                        o.Estado,
                        o.id_UsuarioCreador,
                        nombreSucursal,           // ← NUEVO
                        nombreDepartamento        // ← NUEVO
                    );
                }).ToList();
        }

        public async Task<List<PedidoInternoDTO>> GetPedidosDeOrdenAsync(int idOrden)
        {
            var pedidos = await _pedidoRepositorio.GetAllasync();
            var detallesAdj = await _detalleAdjRepositorio.GetAllasync();

            var idsPedidosAdjudicados = detallesAdj
                .Select(d => d.id_Pedido)
                .ToHashSet();

            return pedidos
                .Where(p => p.id_OrdenCompra == idOrden)
                .OrderByDescending(p => p.urgente)
                .ThenByDescending(p => p.id)
                .Select(p => new PedidoInternoDTO(
                    p.id,
                    p.codigo,
                    p.cantidad,
                    p.id_Departamento,
                    null,
                    p.id_OrdenCompra,
                    p.Fecha_Solicitada,
                    p.Fecha_Ingreso,
                    null,
                    null,
                    p.urgente,
                    0,
                    idsPedidosAdjudicados.Contains(p.id),
                    null,
                    null,
                    null,
                    null,                               // ← NUEVO: Observaciones
                    new List<ArticuloDePedidoDTO>()
                )).ToList();
        }

        // ==================== ESTADOS ====================

        public async Task AprobarAsync(int id)
        {
            var orden = await _ordenCompraRepositorio.GetAsync(id);
            if (orden == null)
                throw new InvalidOperationException($"La orden #{id} no existe.");

            if (orden.Estado != "Borrador")
                throw new InvalidOperationException(
                    $"La orden #{id} no está en estado 'Borrador'. Estado actual: '{orden.Estado}'.");

            orden.Estado = "Aprobada";
            await _ordenCompraRepositorio.UpdateAsync(orden);
        }

        public async Task PublicarAsync(int id)
        {
            var orden = await _ordenCompraRepositorio.GetAsync(id);
            if (orden == null)
                throw new InvalidOperationException($"La orden #{id} no existe.");

            if (orden.Estado != "Aprobada")
                throw new InvalidOperationException(
                    $"Solo se pueden publicar órdenes 'Aprobadas'. Estado actual: '{orden.Estado}'.");

            var pedidos = await _pedidoRepositorio.GetAllasync();
            var pedidosDeLaOrden = pedidos.Where(p => p.id_OrdenCompra == id).ToList();

            if (!pedidosDeLaOrden.Any())
                throw new InvalidOperationException(
                    $"No se puede publicar la orden #{id} porque no tiene pedidos. " +
                    $"Agrega al menos 1 pedido antes de publicarla.");

            orden.Estado = "Publicada";
            await _ordenCompraRepositorio.UpdateAsync(orden);
        }

        public async Task CerrarAsync(int id)
        {
            var orden = await _ordenCompraRepositorio.GetAsync(id);
            if (orden == null)
                throw new InvalidOperationException($"La orden #{id} no existe.");

            if (orden.Estado != "Publicada")
                throw new InvalidOperationException(
                    $"Solo se pueden cerrar órdenes 'Publicadas'. Estado actual: '{orden.Estado}'.");

            orden.Estado = "Adjudicada";
            await _ordenCompraRepositorio.UpdateAsync(orden);
        }

        public async Task CancelarAsync(int id)
        {
            var orden = await _ordenCompraRepositorio.GetAsync(id);
            if (orden == null)
                throw new InvalidOperationException($"La orden #{id} no existe.");

            if (orden.Estado == "Adjudicada")
                throw new InvalidOperationException(
                    $"No se puede cancelar una orden ya adjudicada.");

            orden.Estado = "Cancelada";
            await _ordenCompraRepositorio.UpdateAsync(orden);
        }
    }
}