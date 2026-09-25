// 1. Historial de compras por artículo
export interface HistorialCompra {
  fechaResolucion: string;
  proveedor: string;
  precio: number;
}

// 2. Ranking de proveedores
export interface RankingProveedor {
  idProveedor: number;
  nombre: string;
  totalAdjudicado: number;
}

// 3. Ofertas por orden
export interface OfertaComparativa {
  idProveedor: number;
  proveedor: string;
  precio: number;
  fechaOferta: string;
  esGanador: boolean;
}

// 4. Pedidos pendientes
export interface PedidoPendiente {
  id: number;
  idDepartamento: number;
  fechaSolicitada: string;
}

// 5. Órdenes activas
export interface OrdenActiva {
  id: number;
  descripcion: string;
  fecha_Creacion: string;
  fecha_Limite: string;
}

// 6. Gasto departamental
export interface GastoDepartamental {
  idDepartamento: number;
  nombreDepartamento: string;
  totalGastado: number;
}

// 7. Eficiencia del proceso
export interface EficienciaCompra {
  promedioDias: number;
}

// 8. Variación de precios
export interface VariacionPrecio {
  fecha: string;
  precio: number;
}
