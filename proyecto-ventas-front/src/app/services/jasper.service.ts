import { Injectable } from '@angular/core';

export interface ReporteJasper {
  id: string;
  titulo: string;
  descripcion: string;
  icono: string;
  rutaJasper: string;
  color: string;
}

@Injectable({ providedIn: 'root' })
export class JasperService {
  private readonly serverBaseUrl = 'http://localhost:8080/jasperserver-pro';

  readonly reportes: ReporteJasper[] = [
    {
      id: 'ranking-proveedores',
      titulo: 'Ranking de Proveedores',
      descripcion: 'Top proveedores con mayores montos adjudicados.',
      icono: 'leaderboard',
      rutaJasper: '/datasources/ReporteRanking',
      color: 'gradient-green'
    },
    {
      id: 'gasto-departamental',
      titulo: 'Gasto Departamental',
      descripcion: 'Monto total gastado por departamento y sucursal.',
      icono: 'account_balance',
      rutaJasper: '/datasources/ReporteGastoDepartamenta_',
      color: 'gradient-red'
    },
    {
      id: 'ordenes-activas',
      titulo: 'Órdenes Activas',
      descripcion: 'Órdenes de compra abiertas a recibir ofertas.',
      icono: 'play_circle',
      rutaJasper: '/datasources/RepoteOrdenesActivas',
      color: 'gradient-cyan'
    },
    {
      id: 'pedidos-pendientes',
      titulo: 'Pedidos Pendientes',
      descripcion: 'Pedidos internos sin asignar a una orden.',
      icono: 'pending_actions',
      rutaJasper: '/datasources/ReportePedidoPendiente',
      color: 'gradient-purple'
    },
    {
      id: 'historial-articulo',
      titulo: 'Historial por Artículo',
      descripcion: 'Compras de un artículo con proveedor y precio.',
      icono: 'history',
      rutaJasper: '/datasources/ReporteHistorialArticulo',
      color: 'gradient-blue'
    },
    {
      id: 'ofertas-orden',
      titulo: 'Ofertas por Orden',
      descripcion: 'Comparativa de ofertas recibidas por orden.',
      icono: 'compare_arrows',
      rutaJasper: '/datasources/ReporteOfertaOrden',
      color: 'gradient-orange'
    },
    {
      id: 'eficiencia-proceso',
      titulo: 'Eficiencia del Proceso',
      descripcion: 'Tiempo promedio desde orden hasta adjudicación.',
      icono: 'speed',
      rutaJasper: '/datasources/ReporteEficienciaProceso',
      color: 'gradient-teal'
    },
    {
      id: 'variacion-precios',
      titulo: 'Variación de Precios',
      descripcion: 'Evolución de precios por artículo y proveedor.',
      icono: 'trending_up',
      rutaJasper: '/datasources/ReporteVariacionPrecio',
      color: 'gradient-pink'
    }
  ];

  /**
   * Genera la URL de iframe para un reporte Jasper.
   */
  getUrlReporte(rutaJasper: string): string {
    const params = new URLSearchParams({
      _flowId: 'viewReportFlow',
      reportUnit: rutaJasper,
      standAlone: 'true',
      output: 'html',
      decorator: 'no'
    });

    return `${this.serverBaseUrl}/flow.html?${params.toString()}`;
  }
}