export interface PedidoAdjudicar {
  id_Pedido: number;
  id_Proveedor: number;
}

export interface AdjudicarPedidosDTO {
  orden_Compra: number;
  pedidos: PedidoAdjudicar[];
}

/** Representa un pedido con sus ofertas disponibles para adjudicar */
export interface PedidoConOfertas {
  pedido: any; // PedidoInterno
  ofertas: any[]; // OfertaProveedor[]
  seleccionado: boolean;
  id_ProveedorSeleccionado: number | null;
}