import { Component, OnInit, ChangeDetectorRef, ViewChild, AfterViewInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { MatTableModule, MatTableDataSource } from '@angular/material/table';
import { MatPaginatorModule, MatPaginator } from '@angular/material/paginator';
import { MatSortModule, MatSort } from '@angular/material/sort';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatChipsModule } from '@angular/material/chips';
import { PedidoInternoService } from '../../../services/pedido-interno.service';
import { DepartamentoService } from '../../../services/departamento.service';
import { OrdenCompraService, OrdenCompra } from '../../../services/orden-compra.service';
import { AuthService } from '../../../services/auth.service';
import { PedidoInterno } from '../../../models/pedido-interno.model';
import { Departamento } from '../../../models/departamento.model';
import { PedidoFormComponent } from './pedido-form/pedido-form';
import { ConfirmService } from '../../../services/confirm';
import { NotificacionService } from '../../../services/notificacion';

@Component({
  selector: 'app-pedidos-internos',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    PedidoFormComponent,
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
  templateUrl: './pedidos-internos.html',
  styleUrl: './pedidos-internos.scss'
})
export class PedidosInternosComponent implements OnInit, AfterViewInit {

  displayedColumns: string[] = [
    'urgente',
    'id',
    'codigo',
    'cantidad',
    'departamento',
    'sucursal',
    'orden',
    'estado',
    'fecha_Solicitada',
    'fecha_Ingreso',
    'acciones'
  ];

  dataSource = new MatTableDataSource<PedidoInterno>([]);
  departamentos: Departamento[] = [];
  ordenes: OrdenCompra[] = [];
  cargando = false;
  error = '';
  mostrarFormulario = false;
  pedidoSeleccionado: PedidoInterno | null = null;

  // ⚠️ NUEVO: Orden preseleccionada (cuando vienes desde el modal de órdenes)
  ordenPreseleccionada: number | null = null;

  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;

  constructor(
    private pedidoService: PedidoInternoService,
    private departamentoService: DepartamentoService,
    private ordenCompraService: OrdenCompraService,
    private authService: AuthService,
    private route: ActivatedRoute,
    private cdr: ChangeDetectorRef,
    private notificacion: NotificacionService,
    private confirm: ConfirmService
  ) { }

  ngOnInit(): void { }

  ngAfterViewInit(): void {
    this.dataSource.paginator = this.paginator;
    this.dataSource.sort = this.sort;

    // ===== FILTRO PERSONALIZADO =====
    this.dataSource.filterPredicate = (pedido: PedidoInterno, filtro: string) => {
      const estadoStr = this.estadoPedido(pedido).toLowerCase();
      const dataStr = (
        pedido.id + ' ' +
        (pedido.codigo || '') + ' ' +
        (pedido.cantidad || '') + ' ' +
        (pedido.nombreDepartamento || '') + ' ' +
        (pedido.nombreSucursal || '') + ' ' +
        estadoStr
      ).toLowerCase();
      return dataStr.includes(filtro);
    };

    this.cargarCatalogos();
    this.cargar();

    // ===== LEER QUERY PARAM ?ordenId=X =====
    this.route.queryParams.subscribe(params => {
      const ordenId = params['ordenId'];
      if (ordenId) {
        const id = parseInt(ordenId, 10);
        if (!isNaN(id)) {
          this.ordenPreseleccionada = id;
          // Esperar a que carguen los catálogos y luego abrir el formulario
          setTimeout(() => {
            this.abrirFormularioConOrden();
          }, 600);
        }
      }
    });
  }

  aplicarFiltro(event: Event): void {
    const valor = (event.target as HTMLInputElement).value;
    this.dataSource.filter = valor.trim().toLowerCase();
  }

  cargarCatalogos(): void {
    this.departamentoService.listar().subscribe({
      next: (data) => { this.departamentos = data; this.cdr.detectChanges(); }
    });
    this.ordenCompraService.listar().subscribe({
      next: (data: OrdenCompra[]) => { this.ordenes = data; this.cdr.detectChanges(); }
    });
  }

  cargar(): void {
    this.cargando = true;
    this.error = '';

    // Admin y Auditor ven TODOS los pedidos
    if (this.authService.esAdmin() || this.authService.esAuditor()) {
      this.pedidoService.listar().subscribe({
        next: (data) => {
          this.dataSource.data = data;
          this.cargando = false;
          this.cdr.detectChanges();
        },
        error: (err: any) => {
          this.error = `Error: ${err.status} ${err.statusText}`;
          this.cargando = false;
          this.cdr.detectChanges();
        }
      });
    } else {
      const idDepto = this.authService.getIdDepartamento();

      if (idDepto) {
        this.pedidoService.listarPorDepartamento(idDepto).subscribe({
          next: (data) => {
            this.dataSource.data = data;
            this.cargando = false;
            this.cdr.detectChanges();
          },
          error: (err: any) => {
            this.error = `Error: ${err.status} ${err.statusText}`;
            this.cargando = false;
            this.cdr.detectChanges();
          }
        });
      } else {
        this.dataSource.data = [];
        this.cargando = false;
        this.cdr.detectChanges();
      }
    }
  }

  nombreDepartamento(id: number): string {
    return this.departamentos.find(d => d.id === id)?.nombre || `Depto #${id}`;
  }

  nombreOrden(id: number | null): string {
    if (!id) return 'Sin orden';
    return this.ordenes.find(o => o.id === id)?.descripcion || `Orden #${id}`;
  }

  abrirFormulario(pedido: PedidoInterno | null): void {
    this.pedidoSeleccionado = pedido;
    this.mostrarFormulario = true;
    this.cdr.detectChanges();
  }

  // ⚠️ NUEVO: Abre el formulario con la orden preseleccionada
  abrirFormularioConOrden(): void {
    if (!this.ordenPreseleccionada) return;
    this.pedidoSeleccionado = null;
    this.mostrarFormulario = true;
    this.cdr.detectChanges();
  }

  cerrarFormulario(): void {
    this.mostrarFormulario = false;
    this.pedidoSeleccionado = null;
    this.ordenPreseleccionada = null;
    this.cdr.detectChanges();
  }

  onGuardado(): void {
    this.cerrarFormulario();
    this.cargar();
  }

  confirmarEliminar(pedido: PedidoInterno): void {
    this.confirm.eliminar(`Pedido #${pedido.id}`).subscribe((confirmado: boolean) => {
      if (!confirmado) return;

      this.pedidoService.eliminar(pedido.id).subscribe({
        next: () => {
          this.notificacion.exito(`Pedido #${pedido.id} eliminado correctamente`);
          this.cargar();
        },
        error: (err: any) => {
          this.notificacion.error(`Error al eliminar: ${err.error?.mensaje || err.status + ' ' + err.statusText}`);
        }
      });
    });
  }

  // ===== PERMISOS =====
  puedeCrearPedidos(): boolean {
    return this.authService.puedeCrearPedidos();
  }

  puedeEditarPedidos(): boolean {
    return this.authService.puedeEditarPedidos();
  }

  puedeEliminarPedidos(): boolean {
    return this.authService.esAdmin();
  }

  // ===== ESTADO DEL PEDIDO =====
  estadoPedido(pedido: PedidoInterno): string {
    if (pedido.adjudicado) return 'Adjudicado';
    if (!pedido.id_OrdenCompra) return 'Sin Orden';
    if (pedido.totalOfertas > 0) return 'Con Ofertas';
    return 'Creado';
  }

  claseEstado(pedido: PedidoInterno): string {
    if (pedido.adjudicado) return 'chip-adjudicado';
    if (!pedido.id_OrdenCompra) return 'chip-sin-orden';
    if (pedido.totalOfertas > 0) return 'chip-con-ofertas';
    return 'chip-creado';
  }

  iconoEstado(pedido: PedidoInterno): string {
    if (pedido.adjudicado) return 'check_circle';
    if (!pedido.id_OrdenCompra) return 'remove_circle_outline';
    if (pedido.totalOfertas > 0) return 'local_offer';
    return 'fiber_new';
  }

  tooltipEstado(pedido: PedidoInterno): string {
    if (pedido.adjudicado) {
      return `Adjudicado a ${pedido.nombreProveedorGanador || 'Proveedor #' + pedido.idProveedorGanador}` +
             `\nPrecio: Q ${(pedido.precioAdjudicado || 0).toFixed(2)}`;
    }
    if (!pedido.id_OrdenCompra) {
      return 'Sin orden de compra asignada';
    }
    if (pedido.totalOfertas > 0) {
      return `${pedido.totalOfertas} oferta(s) recibida(s). Pendiente de adjudicar.`;
    }
    return 'Sin ofertas aún';
  }
}