export interface ProveedorRelacionInfo {
  id: number;
  nombre: string;
  tipoRelacion: string;
}

export interface DetalleAdjudicacion {
  id_Adjudicacion: number;
  id_Pedido: number;
  id_Proveedor: number;
  precio: number;
  cantidad: number;
  nombreProveedor?: string;
  relacionesProveedor?: ProveedorRelacionInfo[];
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