export interface ArticuloDePedido {
  id_Articulo: number;
  codigo: string;
  nombre: string;
  cantidad: number;
  unidadMedida?: string;
}

export interface PedidoInterno {
  id: number;
  codigo: string;
  cantidad: number;
  id_Departamento: number;
  nombreDepartamento?: string;
  id_OrdenCompra: number | null;
  fecha_Solicitada: string;
  fecha_Ingreso: string;
  nombreSucursal?: string;
  id_Sucursal?: number;
  urgente: boolean;
  totalOfertas: number;
  adjudicado: boolean;
  idProveedorGanador?: number;
  nombreProveedorGanador?: string;
  precioAdjudicado?: number;
  observaciones?: string;                   
  articulos: ArticuloDePedido[];            
}

export interface CreateArticuloPedidoDTO {
  id_Articulo: number;
  cantidad: number;
}

export interface CreatePedidoInternoDTO {
  codigo: string;
  cantidad: number;
  id_Departamento: number;
  id_OrdenCompra: number | null;
  fecha_Solicitada: string;
  urgente: boolean;
  observaciones?: string;                    
  articulos?: CreateArticuloPedidoDTO[];     
}

export interface UpdatePedidoInternoDTO {
  id: number;
  codigo: string;
  cantidad: number;
  id_Departamento: number;
  id_OrdenCompra: number | null;
  fecha_Solicitada: string;
  fecha_Ingreso: string;
  urgente: boolean;
  observaciones?: string;                    
  articulos?: CreateArticuloPedidoDTO[];     
}

export interface PedidoDisponibleProveedor {
  id: number;
  codigo: string;
  cantidad: number;
  id_Departamento: number;
  nombreDepartamento?: string;
  id_OrdenCompra: number | null;
  fecha_Solicitada: string;
  fecha_Ingreso: string;
  nombreSucursal?: string;
  id_Sucursal?: number;
  urgente: boolean;
  observaciones?: string;
  totalOfertas: number;
  adjudicado: boolean;
  articulos: ArticuloDePedido[];
  // ✅ Info de competencia
  ofertasDeMiRubro: number;
  precioMinimoDelRubro: number | null;
  yaOferte: boolean;
}

export interface ProveedorRelacionado {
  id: number;
  nombre: string;
  tipoRelacion: string;
  precioOfertado: number;
}

export interface PedidoDisponibleProveedor {
  id: number;
  codigo: string;
  cantidad: number;
  id_Departamento: number;
  nombreDepartamento?: string;
  id_OrdenCompra: number | null;
  fecha_Solicitada: string;
  fecha_Ingreso: string;
  nombreSucursal?: string;
  id_Sucursal?: number;
  urgente: boolean;
  observaciones?: string;
  totalOfertas: number;
  adjudicado: boolean;
  articulos: ArticuloDePedido[];
  ofertasDeMiRubro: number;
  precioMinimoDelRubro: number | null;
  yaOferte: boolean;
  miPrecioOfertado: number | null;
  proveedoresRelacionadosQueOfertaron: ProveedorRelacionado[];
}