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

  // ===== ESTADO DEL PEDIDO =====
  totalOfertas: number;
  adjudicado: boolean;
  idProveedorGanador?: number;
  nombreProveedorGanador?: string;
  precioAdjudicado?: number;
}

export interface CreatePedidoInternoDTO {
  codigo: string;
  cantidad: number;
  id_Departamento: number;
  id_OrdenCompra: number | null;
  fecha_Solicitada: string;
  urgente: boolean;
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
}