import { Injectable } from '@angular/core';

export interface ReportePdf {
  id: string;
  titulo: string;
  descripcion: string;
  icono: string;
  endpoint: string;
  color: string;
}

@Injectable({ providedIn: 'root' })
export class ReportePdfService {
  private readonly apiUrl = 'http://localhost:5000/api/ReporteControlador/pdf';

  readonly reportes: ReportePdf[] = [
    {
      id: 'ranking-proveedores',
      titulo: 'Ranking de Proveedores',
      descripcion: 'Top proveedores con mayores montos adjudicados.',
      icono: 'leaderboard',
      endpoint: 'ranking-proveedores',
      color: 'gradient-green'
    },
    {
      id: 'gasto-departamental',
      titulo: 'Gasto Departamental',
      descripcion: 'Monto total gastado por departamento y sucursal.',
      icono: 'account_balance',
      endpoint: 'gasto-departamental',
      color: 'gradient-red'
    },
    {
      id: 'ordenes-activas',
      titulo: 'Órdenes Activas',
      descripcion: 'Órdenes de compra abiertas a recibir ofertas.',
      icono: 'play_circle',
      endpoint: 'ordenes-activas',
      color: 'gradient-cyan'
    },
    {
      id: 'pedidos-pendientes',
      titulo: 'Pedidos Pendientes',
      descripcion: 'Pedidos internos sin asignar a una orden.',
      icono: 'pending_actions',
      endpoint: 'pedidos-pendientes',
      color: 'gradient-purple'
    },
    {
      id: 'historial-articulo',
      titulo: 'Historial por Artículo',
      descripcion: 'Compras de un artículo con proveedor y precio.',
      icono: 'history',
      endpoint: 'historial-articulo',
      color: 'gradient-blue'
    },
    {
      id: 'ofertas-orden',
      titulo: 'Ofertas por Orden',
      descripcion: 'Comparativa de ofertas recibidas por orden.',
      icono: 'compare_arrows',
      endpoint: 'ofertas-orden',
      color: 'gradient-orange'
    },
    {
      id: 'eficiencia-proceso',
      titulo: 'Eficiencia del Proceso',
      descripcion: 'Tiempo promedio desde orden hasta adjudicación.',
      icono: 'speed',
      endpoint: 'eficiencia-proceso',
      color: 'gradient-teal'
    },
    {
      id: 'variacion-precios',
      titulo: 'Variación de Precios',
      descripcion: 'Evolución de precios por artículo y proveedor.',
      icono: 'trending_up',
      endpoint: 'variacion-precios',
      color: 'gradient-pink'
    }
  ];

  /**
   * Obtiene la URL del PDF para un reporte.
   */
  getUrlPdf(endpoint: string): string {
    return `${this.apiUrl}/${endpoint}`;
  }
}