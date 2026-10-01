import { Component, OnInit, ChangeDetectorRef, ViewChild, AfterViewInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatTableModule, MatTableDataSource } from '@angular/material/table';
import { MatPaginatorModule, MatPaginator } from '@angular/material/paginator';
import { MatSortModule, MatSort } from '@angular/material/sort';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatChipsModule } from '@angular/material/chips';
import { AdjudicacionService } from '../../../services/adjudicacion.service';
import { PedidoInternoService } from '../../../services/pedido-interno.service';
import { OfertaProveedorService } from '../../../services/oferta-proveedor.service';
import { Adjudicacion } from '../../../models/adjudicacion.model';
import { AdjudicacionFormComponent } from './adjudicacion-form/adjudicacion-form';
import { DetalleAdjudicacionModalComponent } from './detalle-adjudicacion-modal/detalle-adjudicacion-modal';
import { AdjudicarPedidosModalComponent } from './adjudicar-pedidos-modal/adjudicar-pedidos-modal';
import { ConfirmService } from '../../../services/confirm';
import { NotificacionService } from '../../../services/notificacion';

@Component({
  selector: 'app-adjudicaciones',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    AdjudicacionFormComponent,
    DetalleAdjudicacionModalComponent,
    AdjudicarPedidosModalComponent,
    MatTableModule,
    MatPaginatorModule,
    MatSortModule,
    MatFormFieldModule,
    MatInputModule,
    MatIconModule,
    MatButtonModule,
    MatTooltipModule,
    MatChipsModule
  ],
  templateUrl: './adjudicaciones.html',
  styleUrl: './adjudicaciones.scss'
})
export class AdjudicacionesComponent implements OnInit, AfterViewInit {
  displayedColumns: string[] = ['id', 'fecha_Resolucion', 'orden_Compra', 'estado', 'detalles', 'acciones'];
  dataSource = new MatTableDataSource<Adjudicacion>([]);
  cargando = false;
  error = '';
  mostrarFormulario = false;
  adjudicacionSeleccionada: Adjudicacion | null = null;

  // Modal de Detalle
  mostrarDetalle = false;
  adjudicacionDetalle: Adjudicacion | null = null;

  // Modal de Adjudicar Pedidos (NUEVO)
  mostrarAdjudicar = false;
  sinPedidosPendientes = true;  // ← AQUÍ ESTÁ LA PROPIEDAD QUE FALTABA

  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;

  constructor(
    private adjudicacionService: AdjudicacionService,
    private pedidoService: PedidoInternoService,
    private ofertaService: OfertaProveedorService,
    private cdr: ChangeDetectorRef,
    private notificacion: NotificacionService,
    private confirm: ConfirmService
  ) { }

  ngOnInit(): void {
    this.verificarPedidosPendientes();
  }

  ngAfterViewInit(): void {
    this.dataSource.paginator = this.paginator;
    this.dataSource.sort = this.sort;
    this.dataSource.filterPredicate = (adjudicacion: Adjudicacion, filtro: string) => {
      const dataStr = (adjudicacion.id + ' ' + adjudicacion.fecha_Resolucion + ' ' + adjudicacion.orden_Compra + ' ' + adjudicacion.estado).toLowerCase();
      return dataStr.includes(filtro);
    };
    this.cargar();
  }

  cargar(): void {
    this.cargando = true;
    this.error = '';
    this.adjudicacionService.listar().subscribe({
      next: (data) => {
        this.dataSource.data = data;
        this.cargando = false;
        this.verificarPedidosPendientes();
        this.cdr.detectChanges();
      },
      error: (err: any) => {
        this.error = `Error: ${err.status} ${err.statusText}`;
        this.cargando = false;
        this.cdr.detectChanges();
      }
    });
  }

  // ===== VERIFICAR SI HAY PEDIDOS PENDIENTES DE ADJUDICAR =====
  verificarPedidosPendientes(): void {
    this.pedidoService.listar().subscribe({
      next: (pedidos) => {
        // Un pedido está pendiente si tiene ofertas registradas (para adjudicar)
        // Como validación simple: cualquier pedido sirve
        // El modal filtrará los que realmente tienen ofertas
        this.sinPedidosPendientes = pedidos.length === 0;
        this.cdr.detectChanges();
      },
      error: (err: any) => {
        console.error('Error al verificar pedidos pendientes:', err);
        this.sinPedidosPendientes = true;
        this.cdr.detectChanges();
      }
    });
  }

  aplicarFiltro(event: Event): void {
    const valor = (event.target as HTMLInputElement).value;
    this.dataSource.filter = valor.trim().toLowerCase();
  }

  abrirFormulario(adjudicacion: Adjudicacion | null): void {
    this.adjudicacionSeleccionada = adjudicacion;
    this.mostrarFormulario = true;
    this.cdr.detectChanges();
  }

  cerrarFormulario(): void {
    this.mostrarFormulario = false;
    this.adjudicacionSeleccionada = null;
    this.cdr.detectChanges();
  }

  onGuardado(): void {
    this.cerrarFormulario();
    this.cargar();
  }

  confirmarEliminar(adjudicacion: Adjudicacion): void {
    this.confirm.eliminar(`la adjudicación #${adjudicacion.id}`).subscribe(confirmado => {
      if (!confirmado) return;
      this.adjudicacionService.eliminar(adjudicacion.id).subscribe({
        next: () => {
          this.notificacion.exito(`Adjudicación #${adjudicacion.id} eliminada correctamente`);
          this.cargar();
        },
        error: (err: any) => {
          this.notificacion.error(`Error al eliminar: ${err.status} ${err.statusText}`);
        }
      });
    });
  }

  // ===== HELPER: Color del chip según el estado =====
  colorEstado(estado: string): string {
    switch (estado?.toLowerCase()) {
      case 'activa':
        return 'chip-activa';
      case 'cancelada':
        return 'chip-cancelada';
        case 'completada':  
        return 'chip-cerrada';
      case 'cerrada':
        return 'chip-cerrada';
      case 'pendiente':
        return 'chip-pendiente';
      case 'aprobada':                      
        return 'chip-aprobada';
      default:
        return 'chip-default';
    }
  }

  // ===== MODAL DE DETALLES =====
  verDetalle(adjudicacion: Adjudicacion): void {
    this.adjudicacionDetalle = adjudicacion;
    this.mostrarDetalle = true;
    this.cdr.detectChanges();
  }

  cerrarDetalle(): void {
    this.mostrarDetalle = false;
    this.adjudicacionDetalle = null;
    this.cdr.detectChanges();
  }

  // ===== MODAL DE ADJUDICAR PEDIDOS =====
  abrirAdjudicar(): void {
    this.mostrarAdjudicar = true;
    this.cdr.detectChanges();
  }

  cerrarAdjudicar(): void {
    this.mostrarAdjudicar = false;
    this.cdr.detectChanges();
  }

  onAdjudicado(): void {
    this.cerrarAdjudicar();
    this.cargar();
  }
}