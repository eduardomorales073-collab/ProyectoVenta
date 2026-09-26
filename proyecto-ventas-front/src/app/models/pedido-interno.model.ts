export interface PedidoInterno {
  id: number;
  id_Departamento: number;
  id_OrdenCompra: number | null;
  fecha_Solicitada: string;
  fecha_Ingreso: string;
}

export interface CreatePedidoInternoDTO {
  id_Departamento: number;
  id_OrdenCompra: number | null;
  fecha_Solicitada: string;
  fecha_Ingreso: string;
}

export interface UpdatePedidoInternoDTO {
  id: number;
  id_Departamento: number;
  id_OrdenCompra: number | null;
  fecha_Solicitada: string;
  fecha_Ingreso: string;
}
