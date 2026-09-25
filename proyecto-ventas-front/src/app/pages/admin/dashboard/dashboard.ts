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
  totalRoles = 3;
  totalSucursales = 0;

  // Usuarios por rol
  usuariosAdmin = 0;
  usuariosEmpleado = 0;
  usuariosProveedor = 0;

  cargando = false;

  @ViewChild('graficoRoles') graficoRoles!: ElementRef<HTMLCanvasElement>;
  @ViewChild('graficoActivos') graficoActivos!: ElementRef<HTMLCanvasElement>;

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
        this.usuariosAdmin = usuarios.filter(u => u.id_Rol === 1).length;
        this.usuariosEmpleado = usuarios.filter(u => u.id_Rol === 2).length;
        this.usuariosProveedor = usuarios.filter(u => u.id_Rol === 3).length;

        this.cargando = false;
        this.cdr.detectChanges();

        // Crear gráficos después de tener los datos
        setTimeout(() => this.crearGraficos(), 100);
      },
      error: () => {
        this.totalUsuarios = 0;
        this.cargando = false;
      }
    });
  }

  crearGraficos(): void {
    // Gráfico 1: Usuarios por rol (Pie)
    if (this.graficoRoles) {
      new Chart(this.graficoRoles.nativeElement, {
        type: 'doughnut',
        data: {
          labels: ['Administradores', 'Empleados', 'Proveedores'],
          datasets: [{
            data: [this.usuariosAdmin, this.usuariosEmpleado, this.usuariosProveedor],
            backgroundColor: ['#667eea', '#10b981', '#f59e0b'],
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
  }
}
