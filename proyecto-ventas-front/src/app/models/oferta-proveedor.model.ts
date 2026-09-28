export interface OfertaProveedor {
  id: number;
  id_Proveedor: number;
  id_Pedido_Interno: number;
  precio: number;
  fecha_Oferta: string;
}

export interface CreateOfertaProveedorDTO {
  id_Proveedor: number;
  id_Pedido_Interno: number;
  precio: number;
  fecha_Oferta: string;
}