import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';

interface ReporteCard {
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
export class ReportesComponent {
  reportes: ReporteCard[] = [
    {
      titulo: 'Historial por Artículo',
      descripcion: 'Compras de un artículo con proveedor y precio de adjudicación.',
      icono: 'history',
      ruta: '/admin/reportes/historial-articulo',
      color: 'gradient-blue'
    },
    {
      titulo: 'Ranking de Proveedores',
      descripcion: 'Top 5 proveedores con mayores montos adjudicados.',
      icono: 'leaderboard',
      ruta: '/admin/reportes/ranking-proveedores',
      color: 'gradient-green'
    },
    {
      titulo: 'Ofertas por Orden',
      descripcion: 'Comparativa de ofertas recibidas, resaltando al ganador.',
      icono: 'compare_arrows',
      ruta: '/admin/reportes/ofertas-orden',
      color: 'gradient-orange'
    },
    {
      titulo: 'Pedidos Pendientes',
      descripcion: 'Pedidos internos sin asignar a una orden de compra.',
      icono: 'pending_actions',
      ruta: '/admin/reportes/pedidos-pendientes',
      color: 'gradient-purple'
    },
    {
      titulo: 'Órdenes Activas',
      descripcion: 'Órdenes de compra abiertas a recibir ofertas.',
      icono: 'play_circle',
      ruta: '/admin/reportes/ordenes-activas',
      color: 'gradient-cyan'
    },
    {
      titulo: 'Gasto Departamental',
      descripcion: 'Monto total gastado por departamento en una sucursal y año.',
      icono: 'account_balance',
      ruta: '/admin/reportes/gasto-departamental',
      color: 'gradient-red'
    },
    {
      titulo: 'Eficiencia del Proceso',
      descripcion: 'Tiempo promedio (días) desde la orden hasta la adjudicación.',
      icono: 'speed',
      ruta: '/admin/reportes/eficiencia-proceso',
      color: 'gradient-teal'
    },
    {
      titulo: 'Variación de Precios',
      descripcion: 'Evolución de precios de oferta por artículo y proveedor.',
      icono: 'trending_up',
      ruta: '/admin/reportes/variacion-precios',
      color: 'gradient-pink'
    }
  ];

  constructor(private router: Router) { }

  navegar(ruta: string): void {
    this.router.navigate([ruta]);
  }
}
