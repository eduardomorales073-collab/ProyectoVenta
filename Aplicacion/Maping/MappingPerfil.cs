using Aplicacion.DTO;
using AutoMapper;
using Aplicacion.modelos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aplicacion.Mappings
{
    public class AdjudicacionPerfil : Profile
    {
        public AdjudicacionPerfil()
        {
            // CreateMap<Source, Destination>();
            // Example: CreateMap<Permisos, PermisosDTO>();
            CreateMap<Adjudicacion, AdjudicacionDTO>();
            CreateMap<CreateAdjudicacionDTO, Adjudicacion>();
            CreateMap<UpdateAdjudicacionDTO, Adjudicacion>();


        }
    }

    public class ArticuloPerfil : Profile
    {
        public ArticuloPerfil()
        {
            // CreateMap<Source, Destination>();
            // Example: CreateMap<Permisos, PermisosDTO>();
            CreateMap<Articulo, ArticuloDTO>();
            CreateMap<CreateArticuloDTO, Articulo>();
            CreateMap<UpdateActArtDTO, Articulo>();


        }
    }
    public class DepartamentoPerfil : Profile
    {
        public DepartamentoPerfil()
        {
            // CreateMap<Source, Destination>();
            // Example: CreateMap<Permisos, PermisosDTO>();
            CreateMap<Departamento, DepartamentoDTO>();
            CreateMap<CreateDepartamentoDTO, Departamento>();
            CreateMap<UpdateDepartamentoDTO, Departamento>();


        }
    }
    public class DetalleAdjudicacionPerfil : Profile
    {
        public DetalleAdjudicacionPerfil()
        {
            // CreateMap<Source, Destination>();
            // Example: CreateMap<Permisos, PermisosDTO>();
            CreateMap<Detalle_Adjudicacion, DetalleAdjudicacionDTO>();
            CreateMap<CreateDetalleAdjudicacionDTO, Detalle_Adjudicacion>();
            CreateMap<UpdateDetalleAdjudicacionDTO, Detalle_Adjudicacion>();


        }
    }
    public class DetallePedidoPerfil : Profile
    {
        public DetallePedidoPerfil()
        {
            // CreateMap<Source, Destination>();
            // Example: CreateMap<Permisos, PermisosDTO>();
            CreateMap<Detalle_Pedido, DetallePedidoDTO>();
            CreateMap<CreateDetallePedidoDTO, Detalle_Pedido>();
            CreateMap<UpdateDetallePedidoDTO, Detalle_Pedido>();


        }
    }
    public class DetalleTelefonoPerfil : Profile
    {
        public DetalleTelefonoPerfil()
        {
            // CreateMap<Source, Destination>();
            // Example: CreateMap<Permisos, PermisosDTO>();
            CreateMap<Detalle_Telefono, DetalleTelefonoDTO>();
            CreateMap<CreateDetalleTelefonoDTO, Detalle_Telefono>();
            CreateMap<UpdateDetalleTelefonoDTO, Detalle_Telefono>();


        }
    }
    public class DireccionPerfil : Profile
    {
        public DireccionPerfil()
        {
            // CreateMap<Source, Destination>();
            // Example: CreateMap<Permisos, PermisosDTO>();
            CreateMap<Direccion, DireccionDTO>();
            CreateMap<CreateDireccionDTO, Direccion>();
            CreateMap<UpdateDireccionDTO, Direccion>();


        }
    }
    public class OfertaProveedorPerfil : Profile
    {
        public OfertaProveedorPerfil()
        {
            // CreateMap<Source, Destination>();
            // Example: CreateMap<Permisos, PermisosDTO>();
            CreateMap<Oferta_Proveedor, OfertaProveedorDTO>();
            CreateMap<CreateOfertaProveedorDTO, Oferta_Proveedor>();
            CreateMap<UpdateOfertaProveedorDTO, Oferta_Proveedor>();


        }
    }
    public class OrdenCompraPerfil : Profile
    {
        public OrdenCompraPerfil()
        {
            // CreateMap<Source, Destination>();
            // Example: CreateMap<Permisos, PermisosDTO>();
            CreateMap<Orden_Compra, OrdenCompraDTO>();
            CreateMap<CreateOrdenCompraDTO, Orden_Compra>();
            CreateMap<UpdateOrdenCompraDTO, Orden_Compra>();


        }
    }
    public class PedidoInternoPerfil : Profile
    {
        public PedidoInternoPerfil()
        {
            // CreateMap<Source, Destination>();
            // Example: CreateMap<Permisos, PermisosDTO>();
            CreateMap<Pedido_Interno, PedidoInternoDTO>();
            CreateMap<CreatePedidoInternoDTO, Pedido_Interno>();
            CreateMap<UpdatePedidoInternoDTO, Pedido_Interno>();


        }
    }
    public class PermisosPerfil : Profile
    {
        public PermisosPerfil()
        {
            // CreateMap<Source, Destination>();
            // Example: CreateMap<Permisos, PermisosDTO>();
            CreateMap<Permisos, PermisosDTO>();
            CreateMap<CreatePermisosDTO, Permisos>();
            CreateMap<UpdatePermisosDTO, Permisos>();


        }
    }
    public class ProveedorArticuloPerfil : Profile
    {
        public ProveedorArticuloPerfil()
        {
            // CreateMap<Source, Destination>();
            // Example: CreateMap<Permisos, PermisosDTO>();
            CreateMap<Provee_Artic, ProveeArtcDTO>();
            CreateMap<CreateProveeArtcDTO, Provee_Artic>();
            CreateMap<UpdateProveeArtcDTO, Provee_Artic>();


        }
    }
    public class ProveeRubroPerfil : Profile
    {
        public ProveeRubroPerfil()
        {
            // CreateMap<Source, Destination>();
            // Example: CreateMap<Permisos, PermisosDTO>();
            CreateMap<Provee_Rubro, ProveeRubroDTO>();
            CreateMap<CreateProveeRubroDTO, Provee_Rubro>();
            CreateMap<UpdateProveeRubroDTO, Provee_Rubro>();


        }
    }
    public class ProveedorPerfil : Profile
    {
        public ProveedorPerfil()
        {
            // CreateMap<Source, Destination>();
            // Example: CreateMap<Permisos, PermisosDTO>();
            CreateMap<Proveedor, ProveedorDTO>();
            CreateMap<CreateProveedorDTO, Proveedor>();
            CreateMap<UpdateProveedorDTO, Proveedor>();


        }
    }
    public class RelacionPerfil : Profile
    {
        public RelacionPerfil()
        {
            // CreateMap<Source, Destination>();
            // Example: CreateMap<Permisos, PermisosDTO>();
            CreateMap<Relacion, RelacionDTO>();
            CreateMap<CreateRelacionDTO, Relacion>();
            CreateMap<UpdateRelacionDTO, Relacion>();


        }
    }
    public class RolPermisoPerfil : Profile
    {
        public RolPermisoPerfil()
        {
            // CreateMap<Source, Destination>();
            // Example: CreateMap<Permisos, PermisosDTO>();
            CreateMap<Rol_Permiso, RolPermisoDTO>();
            CreateMap<CreateRolPermisoDTO, Rol_Permiso>();
            CreateMap<UpdateRolPermisoDTO, Rol_Permiso>();


        }
    }
    public class RolesPerfil : Profile
    {
        public RolesPerfil()
        {
            // CreateMap<Source, Destination>();
            // Example: CreateMap<Permisos, PermisosDTO>();
            CreateMap<Roles, RolesDTO>();
            CreateMap<CreateRolesDTO, Roles>();
            CreateMap<UpdateRolesDTO, Roles>();


        }
    }
    public class RubroPerfil : Profile
    {
        public RubroPerfil()
        {
            // CreateMap<Source, Destination>();
            // Example: CreateMap<Permisos, PermisosDTO>();
            CreateMap<Rubro, RubroDTO>();
            CreateMap<CreateRubroDTO, Rubro>();
            CreateMap<UpdateRubroDTO, Rubro>();


        }
    }
    public class SucursalPerfil : Profile
    {
        public SucursalPerfil()
        {
            // CreateMap<Source, Destination>();
            // Example: CreateMap<Permisos, PermisosDTO>();
            CreateMap<Sucursal, SucursalDTO>();
            CreateMap<CreateSucursalDTO, Sucursal>();
            CreateMap<UpdateSucursalDTO, Sucursal>();


        }
    }
    public class TelefonoPerfil : Profile
    {
        public TelefonoPerfil()
        {
            // CreateMap<Source, Destination>();
            // Example: CreateMap<Permisos, PermisosDTO>();
            CreateMap<Telefono, TelefonoDTO>();
            CreateMap<CreateTelefonoDTO, Telefono>();
            CreateMap<UpdateTelefonoDTO, Telefono>();


        }
    }
    public class TipoOrdenPerfil : Profile
    {
        public TipoOrdenPerfil()
        {
            // CreateMap<Source, Destination>();
            // Example: CreateMap<Permisos, PermisosDTO>();
            CreateMap<Tipo_Orden, TipoOrdenDTO>();
            CreateMap<CreateTipoOrdenDTO, Tipo_Orden>();
            CreateMap<UpdateTipoOrdenDTO, Tipo_Orden>();


        }
    }
    public class UsuariosPerfil : Profile
    {
        public UsuariosPerfil()
        {
            // CreateMap<Source, Destination>();
            // Example: CreateMap<Permisos, PermisosDTO>();
            CreateMap<Usuarios, UsuariosDTO>();
            CreateMap<CreateUsuariosDTO, Usuarios>();
            CreateMap<UpdateUsuariosDTO, Usuarios>();


        }
    }
    public class UsuariosRolesPerfil : Profile
    {
        public UsuariosRolesPerfil()
        {
            // CreateMap<Source, Destination>();
            // Example: CreateMap<Permisos, PermisosDTO>();
            CreateMap<Usuarios_Roles, UsuariosRolesDTO>();
            CreateMap<CreateUsuariosRolesDTO, Usuarios_Roles>();
            CreateMap<UpdateUsuariosRolesDTO, Usuarios_Roles>();


        }
    }

}
