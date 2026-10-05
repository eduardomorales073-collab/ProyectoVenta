import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { AuthService } from '../../../services/auth.service';

interface ReporteCard {
  id: string;
  titulo: string;
  descripcion: string;
  icono: string;
  ruta: string;
  color: string;
}

@Component({
  selector: 'app-reportes',
  standalone: true,
  imports: [
    CommonModule,
    MatCardModule,
    MatIconModule,
    MatButtonModule
  ],
  templateUrl: './reportes.html',
  styleUrl: './reportes.scss'
})
export class ReportesComponent implements OnInit {
  reportes: ReporteCard[] = [];
  rutaBase: string = '/admin/reportes/jasper/';
  titulo: string = 'Reportes y Análisis';
  subtitulo: string = 'Selecciona el reporte que deseas visualizar';

  constructor(
    private router: Router,
    private authService: AuthService
  ) { }

  ngOnInit(): void {
    // ✅ Adaptar la ruta según el rol del usuario
    if (this.authService.esAdmin()) {
      this.rutaBase = '/admin/reportes/jasper/';
    } else if (this.authService.esAuditor()) {
      this.rutaBase = '/auditor/reportes/jasper/';
    } else if (this.authService.esGestorCompras()) {
      this.rutaBase = '/empleado/reportes/jasper/';
    }

    this.reportes = [
      { id: 'ranking-proveedores', titulo: 'Ranking de Proveedores', descripcion: 'Top proveedores con mayores montos adjudicados.', icono: 'leaderboard', ruta: '', color: 'gradient-green' },
      { id: 'gasto-departamental', titulo: 'Gasto Departamental', descripcion: 'Monto total gastado por departamento y sucursal.', icono: 'account_balance', ruta: '', color: 'gradient-red' },
      { id: 'ordenes-activas', titulo: 'Órdenes Activas', descripcion: 'Órdenes de compra abiertas a recibir ofertas.', icono: 'play_circle', ruta: '', color: 'gradient-cyan' },
      { id: 'pedidos-pendientes', titulo: 'Pedidos Pendientes', descripcion: 'Pedidos internos sin asignar a una orden.', icono: 'pending_actions', ruta: '', color: 'gradient-purple' },
      { id: 'historial-articulo', titulo: 'Historial por Artículo', descripcion: 'Compras de un artículo con proveedor y precio.', icono: 'history', ruta: '', color: 'gradient-blue' },
      { id: 'ofertas-orden', titulo: 'Ofertas por Orden', descripcion: 'Comparativa de ofertas recibidas por orden.', icono: 'compare_arrows', ruta: '', color: 'gradient-orange' },
      { id: 'eficiencia-proceso', titulo: 'Eficiencia del Proceso', descripcion: 'Tiempo promedio desde orden hasta adjudicación.', icono: 'speed', ruta: '', color: 'gradient-teal' },
      { id: 'variacion-precios', titulo: 'Variación de Precios', descripcion: 'Evolución de precios por artículo y proveedor.', icono: 'trending_up', ruta: '', color: 'gradient-pink' }
    ];
  }

  navegar(reporte: ReporteCard): void {
    this.router.navigate([this.rutaBase + reporte.id]);
  }
}