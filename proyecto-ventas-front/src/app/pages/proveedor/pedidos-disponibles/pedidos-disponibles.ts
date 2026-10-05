import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatTableModule } from '@angular/material/table';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatChipsModule } from '@angular/material/chips';
import { MatTooltipModule } from '@angular/material/tooltip';
import { PedidoInternoService } from '../../../services/pedido-interno.service';
import { DepartamentoService } from '../../../services/departamento.service';
import { NotificacionService } from '../../../services/notificacion';
import { OfertaFormComponent } from './oferta-form/oferta-form';
import { PedidoDisponibleProveedor } from '../../../models/pedido-interno.model';
import { Departamento } from '../../../models/departamento.model';

@Component({
  selector: 'app-pedidos-disponibles',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    MatCardModule,
    MatIconModule,
    MatButtonModule,
    MatTableModule,
    MatProgressSpinnerModule,
    MatChipsModule,
    MatTooltipModule,
    OfertaFormComponent
  ],
  templateUrl: './pedidos-disponibles.html',
  styleUrl: './pedidos-disponibles.scss'
})
export class PedidosDisponiblesComponent implements OnInit {
  pedidos: PedidoDisponibleProveedor[] = [];
  departamentos: Departamento[] = [];
  cargando = false;
  error = '';

  mostrarFormulario = false;
  pedidoSeleccionado: PedidoDisponibleProveedor | null = null;

  displayedColumns: string[] = [
    'codigo',
    'articulos',
    'observaciones',
    'cantidad',
    'departamento',
    'fecha_Solicitada',
    'competencia',
    'miOferta',
    'acciones'
    
  ];

  constructor(
    private pedidoService: PedidoInternoService,
    private departamentoService: DepartamentoService,
    private notification: NotificacionService,
    private cdr: ChangeDetectorRef
  ) { }

  ngOnInit(): void {
    this.cargarDepartamentos();
    this.cargar();
  }

  cargarDepartamentos(): void {
    this.departamentoService.listar().subscribe({
      next: (data) => { this.departamentos = data; this.cdr.detectChanges(); }
    });
  }

  cargar(): void {
    this.cargando = true;
    // ✅ Usar el nuevo endpoint que filtra por rubro del proveedor
    this.pedidoService.listarDisponiblesParaProveedor().subscribe({
      next: (data) => {
        this.pedidos = data;
        this.cargando = false;
        this.cdr.detectChanges();
      },
      error: (err: any) => {
        this.error = `Error: ${err.status} ${err.statusText}`;
        this.notification.error(this.error);
        this.cargando = false;
        this.cdr.detectChanges();
      }
    });
  }

  nombreDepartamento(id: number): string {
    return this.departamentos.find(d => d.id === id)?.nombre || `Depto #${id}`;
  }

  abrirOferta(pedido: PedidoDisponibleProveedor): void {
    this.pedidoSeleccionado = pedido;
    this.mostrarFormulario = true;
    this.cdr.detectChanges();
  }

  cerrarFormulario(): void {
    this.mostrarFormulario = false;
    this.pedidoSeleccionado = null;
    this.cdr.detectChanges();
  }

  onOfertado(): void {
    this.cerrarFormulario();
    this.cargar();
  }
}