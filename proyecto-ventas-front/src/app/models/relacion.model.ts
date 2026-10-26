export interface Relacion {
  id: number;
  proveedor1: number;
  nombreProveedor1: string;
  proveedor2: number;
  nombreProveedor2: string;
  tipoRelacion: string;
}

export interface CreateRelacionDTO {
  proveedor1: number;
  proveedor2: number;
  tipoRelacion: string;
}
