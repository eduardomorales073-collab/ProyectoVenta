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
}

export interface CreatePedidoInternoDTO {
  codigo: string;
  cantidad: number;
  id_Departamento: number;
  id_OrdenCompra: number | null;
  fecha_Solicitada: string;
  urgente: boolean;
  // ⚠️ NO tiene fecha_Ingreso
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