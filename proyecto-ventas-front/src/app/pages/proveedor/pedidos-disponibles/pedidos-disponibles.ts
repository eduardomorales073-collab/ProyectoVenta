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
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';

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
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    OfertaFormComponent
  ],
  templateUrl: './pedidos-disponibles.html',
  styleUrl: './pedidos-disponibles.scss'
})
export class PedidosDisponiblesComponent implements OnInit {
  pedidos: PedidoDisponibleProveedor[] = [];
  pedidosOriginales: PedidoDisponibleProveedor[] = [];
  departamentos: Departamento[] = [];

  // Filtros
  filtroPrioridad: string = 'todos';
  filtroTexto: string = '';
  pedidosUrgentesCount = 0;

  cargando = false;
  mostrarFormulario = false;
  pedidoSeleccionado: PedidoDisponibleProveedor | null = null;

  // ✅ CORRECCIÓN: Se eliminó 'miOferta' de las columnas
  displayedColumns: string[] = [
    'urgente', 'codigo', 'articulos', 'observaciones',
    'cantidad', 'departamento', 'fecha_Solicitada',
    'competencia', 'acciones'
  ];

  constructor(
    private pedidoService: PedidoInternoService,
    private departamentoService: DepartamentoService,
    private notification: NotificacionService,
    private cdr: ChangeDetectorRef
  ) { }

  ngOnInit(): void {
    this.cargar();
  }

  cargar(): void {
    this.cargando = true;
    this.pedidoService.listarDisponiblesParaProveedor().subscribe({
      next: (data) => {
        this.pedidosOriginales = data;
        this.pedidosUrgentesCount = data.filter(p => p.urgente).length;
        this.aplicarFiltros();
        this.cargando = false;
        this.cdr.detectChanges();
      },
      error: (err: any) => {
        this.notification.error(`Error: ${err.status}`);
        this.cargando = false;
      }
    });
  }

  aplicarFiltros(): void {
    let filtrados = [...this.pedidosOriginales];

    // Filtro de prioridad
    if (this.filtroPrioridad === 'urgentes') {
      filtrados = filtrados.filter(p => p.urgente);
    } else if (this.filtroPrioridad === 'recientes') {
      const hace7Dias = new Date();
      hace7Dias.setDate(hace7Dias.getDate() - 7);
      filtrados = filtrados.filter(p => new Date(p.fecha_Ingreso) >= hace7Dias);
    }

    // Filtro de texto
    if (this.filtroTexto.trim()) {
      const texto = this.filtroTexto.toLowerCase();
      filtrados = filtrados.filter(p =>
        p.codigo.toLowerCase().includes(texto) ||
        (p.nombreDepartamento || '').toLowerCase().includes(texto)
      );
    }

    // ✅ ORDENAMIENTO: Urgentes primero, luego los más recientes (ID mayor)
    this.pedidos = filtrados.sort((a, b) => {
      if (a.urgente === b.urgente) {
        return b.id - a.id;
      }
      return a.urgente ? -1 : 1;
    });

    this.cdr.detectChanges();
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
    this.cargar(); // Recarga para ocultar el pedido ofertado
  }
}
