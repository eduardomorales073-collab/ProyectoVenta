import { Component, OnInit, ChangeDetectorRef, ViewChild, AfterViewInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { MatTableModule, MatTableDataSource } from '@angular/material/table';
import { MatPaginatorModule, MatPaginator } from '@angular/material/paginator';
import { MatSortModule, MatSort } from '@angular/material/sort';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatChipsModule } from '@angular/material/chips';
import { MatSelectModule } from '@angular/material/select';
import { OrdenCompraService, OrdenCompraConContadores } from '../../../services/orden-compra.service';
import { TipoOrdenService } from '../../../services/tipo-orden.service';
import { TipoOrden } from '../../../models/tipo-orden.model';
import { AuthService } from '../../../services/auth.service';
import { OrdenCompraFormComponent } from './orden-compra-form/orden-compra-form';
import { PedidosDeOrdenModalComponent } from './pedidos-de-orden-modal/pedidos-de-orden-modal';
import { ConfirmService } from '../../../services/confirm';
import { NotificacionService } from '../../../services/notificacion';

@Component({
  selector: 'app-ordenes-compra',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    MatSelectModule,
    OrdenCompraFormComponent,
    PedidosDeOrdenModalComponent,
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
  templateUrl: './ordenes-compra.html',
  styleUrl: './ordenes-compra.scss'
})
export class OrdenesCompraComponent implements OnInit, AfterViewInit {
  // ✅ Filtro por estado
  estadoFiltro: string = 'Todas';
  estadosDisponibles: string[] = ['Todas', 'Borrador', 'Aprobada', 'Publicada', 'Adjudicada', 'Cancelada'];

  displayedColumns: string[] = [
    'id',
    'descripcion',
    'fecha_Creacion',
    'fecha_Limite',
    'tipo_Orden',
    'pedidos',
    'estado',
    'departamento',
    'sucursal',
    'acciones'
  ];

  dataSource = new MatTableDataSource<OrdenCompraConContadores>([]);

  // ✅ Guardar todas las órdenes sin filtrar (para el filtro de estado)
  todasLasOrdenes: OrdenCompraConContadores[] = [];

  cargando = false;
  error = '';
  mostrarFormulario = false;
  mostrarPedidos = false;
  ordenSeleccionada: OrdenCompraConContadores | null = null;

  tiposOrden: TipoOrden[] = [];

  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;

  constructor(
    private ordenService: OrdenCompraService,
    private tipoOrdenService: TipoOrdenService,
    private authService: AuthService,
    private router: Router,
    private cdr: ChangeDetectorRef,
    private notificacion: NotificacionService,
    private confirm: ConfirmService
  ) { }

  ngOnInit(): void { }

  ngAfterViewInit(): void {
    this.dataSource.paginator = this.paginator;
    this.dataSource.sort = this.sort;

    this.dataSource.filterPredicate = (orden: OrdenCompraConContadores, filtro: string) => {
      const tipoStr = this.nombreTipo(orden.tipo_Orden);
      const dataStr = (
        orden.id + ' ' +
        (orden.descripcion || '') + ' ' +
        tipoStr + ' ' +
        orden.estado
      ).toLowerCase();
      return dataStr.includes(filtro);
    };

    this.tipoOrdenService.listar().subscribe({
      next: (data) => {
        this.tiposOrden = data;
        this.cdr.detectChanges();
        this.cargar();
      },
      error: () => {
        this.cargar();
      }
    });
  }

  cargar(): void {
    this.cargando = true;
    this.error = '';
    this.ordenService.listarConContadores().subscribe({
      next: (data) => {
        // ✅ Guardar todas las órdenes sin filtrar
        this.todasLasOrdenes = data;
        // ✅ Aplicar el filtro de estado actual
        this.aplicarFiltroEstado();
        this.cargando = false;
        this.cdr.detectChanges();
      },
      error: (err: any) => {
        this.error = `Error: ${err.status} ${err.statusText}`;
        this.cargando = false;
        this.cdr.detectChanges();
      }
    });
  }

  // ✅ Aplicar filtro por estado
  aplicarFiltroEstado(): void {
    if (this.estadoFiltro === 'Todas') {
      this.dataSource.data = this.todasLasOrdenes;
    } else {
      this.dataSource.data = this.todasLasOrdenes.filter(
        (o: OrdenCompraConContadores) => o.estado === this.estadoFiltro
      );
    }
    this.cdr.detectChanges();
  }

  // ✅ Contar órdenes por estado
  contarPorEstado(estado: string): number {
    if (estado === 'Todas') return this.todasLasOrdenes.length;
    return this.todasLasOrdenes.filter(
      (o: OrdenCompraConContadores) => o.estado === estado
    ).length;
  }

  aplicarFiltro(event: Event): void {
    const valor = (event.target as HTMLInputElement).value;
    this.dataSource.filter = valor.trim().toLowerCase();
  }

  abrirFormulario(orden: OrdenCompraConContadores | null): void {
    this.ordenSeleccionada = orden;
    this.mostrarFormulario = true;
    this.cdr.detectChanges();
  }

  cerrarFormulario(): void {
    this.mostrarFormulario = false;
    this.ordenSeleccionada = null;
    this.cdr.detectChanges();
  }

  onGuardado(): void {
    this.cerrarFormulario();
    this.cargar();
  }

  verPedidos(orden: OrdenCompraConContadores): void {
    this.ordenSeleccionada = orden;
    this.mostrarPedidos = true;
    this.cdr.detectChanges();
  }

  cerrarPedidos(): void {
    this.mostrarPedidos = false;
    this.ordenSeleccionada = null;
    this.cdr.detectChanges();
  }

  onPedidosActualizados(): void {
    this.cerrarPedidos();
    this.cargar();
  }

  irACrearPedido(idOrden: number): void {
    this.cerrarPedidos();
    if (this.authService.esAdmin()) {
      this.router.navigate(['/admin/pedidos-internos'], { queryParams: { ordenId: idOrden } });
    } else {
      this.router.navigate(['/empleado/pedidos'], { queryParams: { ordenId: idOrden } });
    }
  }

  confirmarEliminar(orden: OrdenCompraConContadores): void {
    this.confirm.eliminar(`la orden #${orden.id}`).subscribe(confirmado => {
      if (!confirmado) return;

      this.ordenService.eliminar(orden.id).subscribe({
        next: () => {
          this.notificacion.exito(`Orden #${orden.id} eliminada correctamente`);
          this.cargar();
        },
        error: (err: any) => {
          this.notificacion.error(`Error al eliminar: ${err.error?.mensaje || err.status + ' ' + err.statusText}`);
        }
      });
    });
  }

  // ==================== ACCIONES DE ESTADO ====================
  aprobarOrden(orden: OrdenCompraConContadores): void {
    this.confirm.aceptar(
      'Aprobar orden',
      `¿Aprobar la orden #${orden.id}? Después podrás agregar pedidos y publicarla.`
    ).subscribe((confirmado: boolean) => {
      if (!confirmado) return;

      this.ordenService.aprobar(orden.id).subscribe({
        next: () => {
          this.notificacion.exito(`Orden #${orden.id} aprobada correctamente`);
          this.cargar();
        },
        error: (err: any) => {
          this.notificacion.error(`Error: ${err.error?.mensaje || err.status} ${err.statusText}`);
        }
      });
    });
  }

  

  cerrarOrden(orden: OrdenCompraConContadores): void {
    this.confirm.aceptar(
      'Cerrar orden',
      `¿Cerrar la orden #${orden.id}? Ya no se podrán agregar pedidos.`
    ).subscribe((confirmado: boolean) => {
      if (!confirmado) return;

      this.ordenService.cerrar(orden.id).subscribe({
        next: () => {
          this.notificacion.exito(`Orden #${orden.id} cerrada correctamente`);
          this.cargar();
        },
        error: (err: any) => {
          this.notificacion.error(`Error: ${err.error?.mensaje || err.status} ${err.statusText}`);
        }
      });
    });
  }

  cancelarOrden(orden: OrdenCompraConContadores): void {
    this.confirm.aceptar(
      'Cancelar orden',
      `¿Cancelar la orden #${orden.id}? Esta acción no se puede deshacer.`
    ).subscribe((confirmado: boolean) => {
      if (!confirmado) return;

      this.ordenService.cancelar(orden.id).subscribe({
        next: () => {
          this.notificacion.exito(`Orden #${orden.id} cancelada correctamente`);
          this.cargar();
        },
        error: (err: any) => {
          this.notificacion.error(`Error: ${err.error?.mensaje || err.status} ${err.statusText}`);
        }
      });
    });
  }

  // ==================== PERMISOS ====================
  puedeCrearOrden(): boolean {
    return this.authService.esAdmin()
        || this.authService.esGestorCompras()
        || this.authService.esCreadorPedidos();
  }

  puedeEditarOrden(orden: OrdenCompraConContadores): boolean {
    if (this.authService.esAdmin()) return true;
    if (this.authService.esGestorCompras() && orden.estado !== 'Adjudicada' && orden.estado !== 'Cancelada') return true;
    if (this.authService.esCreadorPedidos() && orden.estado === 'Borrador') return true;
    return false;
  }

  puedeEliminarOrden(): boolean {
    return this.authService.esAdmin();
  }

  puedeVerPedidos(): boolean {
    return this.authService.esAdmin() || this.authService.esGestorCompras();
  }

  puedeAprobar(orden: OrdenCompraConContadores): boolean {
    return (this.authService.esAdmin() || this.authService.esGestorCompras())
           && orden.estado === 'Borrador';
  }

  

  puedeCerrar(orden: OrdenCompraConContadores): boolean {
    return (this.authService.esAdmin() || this.authService.esGestorCompras())
           && orden.estado === 'Publicada'
           && orden.pedidosPendientes === 0
           && orden.totalPedidos > 0;
  }

  puedeCancelar(orden: OrdenCompraConContadores): boolean {
    return this.authService.esAdmin()
           && orden.estado !== 'Cancelada'
           && orden.estado !== 'Adjudicada';
  }

  // ==================== HELPERS PARA EL TIPO DE ORDEN ====================
  nombreTipo(idTipo: number): string {
    const tipo = this.tiposOrden.find(t => t.id === idTipo);
    if (!tipo) return `Tipo #${idTipo}`;
    const partes: string[] = [];
    if (tipo.grande) partes.push('Grande');
    if (tipo.urgente) partes.push('Urgente');
    return partes.length > 0 ? partes.join(' · ') : 'Normal';
  }

  claseTipo(idTipo: number): string {
    const tipo = this.tiposOrden.find(t => t.id === idTipo);
    if (!tipo) return 'chip-tipo';
    if (tipo.urgente) return 'chip-tipo-urgente';
    if (tipo.grande) return 'chip-tipo-grande';
    return 'chip-tipo-normal';
  }

  iconoTipo(idTipo: number): string {
    const tipo = this.tiposOrden.find(t => t.id === idTipo);
    if (!tipo) return 'label';
    if (tipo.urgente) return 'priority_high';
    if (tipo.grande) return 'unfold_more';
    return 'label';
  }

  // ==================== HELPERS PARA EL ESTADO ====================
  claseEstado(estado: string): string {
    switch (estado) {
      case 'Borrador': return 'chip-borrador';
      case 'Aprobada': return 'chip-aprobada';
      case 'Publicada': return 'chip-publicada';
      case 'Adjudicada': return 'chip-adjudicada';
      case 'Cancelada': return 'chip-cancelada';
      default: return 'chip-default';
    }
  }

  iconoEstado(estado: string): string {
    switch (estado) {
      case 'Borrador': return 'edit_note';
      case 'Aprobada': return 'verified';
      case 'Publicada': return 'public';
      case 'Adjudicada': return 'check_circle';
      case 'Cancelada': return 'cancel';
      default: return 'label';
    }
  }
}