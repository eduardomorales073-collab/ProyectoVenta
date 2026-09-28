export interface PedidoInterno {
  id: number;
  codigo: string | null;
  cantidad: number | null;
  id_Departamento: number;
  nombreDepartamento: string | null;
  id_OrdenCompra: number | null;
  fecha_Solicitada: string;
  fecha_Ingreso: string;
  nombreSucursal: string | null;
  id_Sucursal: number | null;
}

export interface CreatePedidoInternoDTO {
  codigo: string | null;
  cantidad: number | null;
  id_Departamento: number;
  id_OrdenCompra: number | null;
  fecha_Solicitada: string;
  fecha_Ingreso: string;
}

export interface UpdatePedidoInternoDTO {
  id: number;
  codigo: string | null;
  cantidad: number | null;
  id_Departamento: number;
  id_OrdenCompra: number | null;
  fecha_Solicitada: string;
  fecha_Ingreso: string;
}