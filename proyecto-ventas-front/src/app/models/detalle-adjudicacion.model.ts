export interface DetalleAdjudicacion {
  id_Adjudicacion: number;
  id_Pedido: number;
  id_Proveedor: number;
  precio: number;
  cantidad: number;
  // Opcionales para mostrar en la tabla
  codigoPedido?: string;
  nombreProveedor?: string;
}

export interface CreateDetalleAdjudicacionDTO {
  id_Adjudicacion: number;
  id_Pedido: number;
  id_Proveedor: number;
  precio: number;
  cantidad: number;
}

export interface UpdateDetalleAdjudicacionDTO {
  id_Adjudicacion: number;
  id_Pedido: number;
  id_Proveedor: number;
  precio: number;
  cantidad: number;
}