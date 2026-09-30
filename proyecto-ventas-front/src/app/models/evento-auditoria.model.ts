export interface EventoAuditoria {
  id: string;
  tipo: 'Compra' | 'Pedido' | 'Oferta' | 'Orden' | 'Cancelacion';
  fecha: string;
  titulo: string;
  detalle: string;
  usuario: string | null;
  referencia: string | null;
}

export interface FiltroAuditoria {
  tipo: string | null;
  fechaDesde: string | null;
  fechaHasta: string | null;
}