import { Component, OnInit, AfterViewInit, ChangeDetectorRef, ElementRef, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatGridListModule } from '@angular/material/grid-list';
import { Chart, registerables } from 'chart.js';
import { AuthService } from '../../../services/auth.service';
import { ArticuloService } from '../../../services/articulo.service';
import { UsuarioService } from '../../../services/usuario.service';
import { Articulo } from '../../../services/articulo.service';
import { Usuario } from '../../../models/usuario.model';

Chart.register(...registerables);

@Component({
  selector: 'app-admin-dashboard',
  standalone: true,
  imports: [
    CommonModule,
    RouterLink,
    MatCardModule,
    MatIconModule,
    MatButtonModule,
    MatGridListModule
  ],
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.scss'
})
export class AdminDashboardComponent implements OnInit, AfterViewInit {
  usuario: any = null;

  // Estadísticas
  totalArticulos = 0;
  totalUsuarios = 0;
  totalRoles = 4;  // ← Ahora son 4 roles
  totalSucursales = 0;

  // Usuarios por rol (4 roles nuevos)
  usuariosAdmin = 0;             // Rol 1: Administrador del Sistema
  usuariosGestorCompras = 0;     // Rol 2: Gestor de Compras
  usuariosAdminProveedor = 0;    // Rol 3: Administrador de Proveedor
  usuariosAuditor = 0;           // Rol 4: Auditor / Reportes

  // Estadísticas de actividad
  usuariosActivos = 0;
  usuariosInactivos = 0;
  articulosRecientes: Articulo[] = [];

  cargando = false;

  @ViewChild('graficoRoles') graficoRoles!: ElementRef<HTMLCanvasElement>;
  @ViewChild('graficoActivos') graficoActivos!: ElementRef<HTMLCanvasElement>;
  @ViewChild('graficoArticulos') graficoArticulos!: ElementRef<HTMLCanvasElement>;

  constructor(
    private authService: AuthService,
    private articuloService: ArticuloService,
    private usuarioService: UsuarioService,
    private cdr: ChangeDetectorRef
  ) { }

  ngOnInit(): void {
    this.usuario = this.authService.getUsuario();
    this.cargarEstadisticas();
  }

  ngAfterViewInit(): void {
    // Los gráficos se crean después de cargar datos
  }

  cargarEstadisticas(): void {
    this.cargando = true;

    // Cargar artículos
    this.articuloService.listar().subscribe({
      next: (articulos) => {
        this.totalArticulos = articulos.length;

        this.articulosRecientes = [...articulos]
          .sort((a, b) => b.id - a.id)
          .slice(0, 5);

        this.cdr.detectChanges();
      },
      error: () => {
        this.totalArticulos = 0;
      }
    });

    // Cargar usuarios
    this.usuarioService.listar().subscribe({
      next: (usuarios) => {
        this.totalUsuarios = usuarios.length;

        // Contadores por rol (4 roles)
        this.usuariosAdmin = usuarios.filter(u => u.id_Rol === 1).length;
        this.usuariosGestorCompras = usuarios.filter(u => u.id_Rol === 2).length;
        this.usuariosAdminProveedor = usuarios.filter(u => u.id_Rol === 3).length;
        this.usuariosAuditor = usuarios.filter(u => u.id_Rol === 4).length;

        this.usuariosActivos = usuarios.filter(u => u.activo).length;
        this.usuariosInactivos = usuarios.filter(u => !u.activo).length;

        this.cargando = false;
        this.cdr.detectChanges();

        setTimeout(() => this.crearGraficos(), 100);
      },
      error: () => {
        this.totalUsuarios = 0;
        this.cargando = false;
      }
    });
  }

  crearGraficos(): void {
    // Gráfico 1: Usuarios por rol (Doughnut con 4 roles)
    if (this.graficoRoles) {
      new Chart(this.graficoRoles.nativeElement, {
        type: 'doughnut',
        data: {
          labels: [
            'Administrador',
            'Gestor de Compras',
            'Admin. Proveedor',
            'Auditor'
          ],
          datasets: [{
            data: [
              this.usuariosAdmin,
              this.usuariosGestorCompras,
              this.usuariosAdminProveedor,
              this.usuariosAuditor
            ],
            backgroundColor: [
              '#667eea',   // Admin - morado
              '#10b981',   // Gestor - verde
              '#f59e0b',   // Proveedor - naranja
              '#8b5cf6'    // Auditor - violeta
            ],
            borderWidth: 3,
            borderColor: '#fff'
          }]
        },
        options: {
          responsive: true,
          maintainAspectRatio: false,
          plugins: {
            legend: {
              position: 'bottom',
              labels: {
                padding: 15,
                font: { size: 12, weight: 'bold' }
              }
            }
          }
        }
      });
    }

    // Gráfico 2: Usuarios activos vs inactivos (Doughnut)
    if (this.graficoActivos) {
      new Chart(this.graficoActivos.nativeElement, {
        type: 'doughnut',
        data: {
          labels: ['Activos', 'Inactivos'],
          datasets: [{
            data: [this.usuariosActivos, this.usuariosInactivos],
            backgroundColor: ['#10b981', '#ef4444'],
            borderWidth: 3,
            borderColor: '#fff'
          }]
        },
        options: {
          responsive: true,
          maintainAspectRatio: false,
          plugins: {
            legend: {
              position: 'bottom',
              labels: {
                padding: 15,
                font: { size: 13, weight: 'bold' }
              }
            }
          }
        }
      });
    }

    // Gráfico 3: Artículos (Bar)
    if (this.graficoArticulos) {
      new Chart(this.graficoArticulos.nativeElement, {
        type: 'bar',
        data: {
          labels: this.articulosRecientes.map(a => `#${a.id}`),
          datasets: [{
            label: 'Artículos recientes',
            data: this.articulosRecientes.map((_, i) => i + 1),
            backgroundColor: [
              '#667eea',
              '#10b981',
              '#f59e0b',
              '#3b82f6',
              '#8b5cf6'
            ],
            borderRadius: 8,
            borderWidth: 0
          }]
        },
        options: {
          responsive: true,
          maintainAspectRatio: false,
          plugins: {
            legend: { display: false }
          },
          scales: {
            y: {
              display: false,
              beginAtZero: true
            },
            x: {
              grid: { display: false }
            }
          }
        }
      });
    }
  }
}
