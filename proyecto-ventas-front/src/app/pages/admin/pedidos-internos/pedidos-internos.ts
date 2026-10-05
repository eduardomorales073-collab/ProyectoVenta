import { Component, OnInit, ChangeDetectorRef, ViewChild, AfterViewInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';  // ← Router añadido
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
import { OrdenCompraService, OrdenCompraConContadores } from '../../../services/orden-compra.service';
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
    'urgente', 'id', 'codigo', 'cantidad', 'departamento',
    'sucursal', 'orden', 'estado', 'fecha_Solicitada',
    'fecha_Ingreso', 'acciones'
  ];

  dataSource = new MatTableDataSource<PedidoInterno>([]);
  departamentos: Departamento[] = [];
  ordenes: OrdenCompraConContadores[] = [];
  cargando = false;
  error = '';
  mostrarFormulario = false;
  pedidoSeleccionado: PedidoInterno | null = null;
  ordenPreseleccionada: number | null = null;

  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;

  constructor(
    private pedidoService: PedidoInternoService,
    private departamentoService: DepartamentoService,
    private ordenCompraService: OrdenCompraService,
    private authService: AuthService,
    private route: ActivatedRoute,
    private router: Router,  // ← NUEVO
    private cdr: ChangeDetectorRef,
    private notificacion: NotificacionService,
    private confirm: ConfirmService
  ) { }

  ngOnInit(): void { }

  ngAfterViewInit(): void {
    this.dataSource.paginator = this.paginator;
    this.dataSource.sort = this.sort;
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

    this.route.queryParams.subscribe(params => {
      const ordenId = params['ordenId'];
      if (ordenId) {
        const id = parseInt(ordenId, 10);
        if (!isNaN(id)) {
          this.ordenPreseleccionada = id;
          this.esperarYAbirFormulario(id);
        }
      }
    });
  }

  // ✅ Espera a que las órdenes estén cargadas antes de abrir el formulario
  esperarYAbirFormulario(ordenId: number): void {
  // Si ya están cargadas, verificar que la orden exista
  if (this.ordenes.length > 0) {
    const ordenExiste = this.ordenes.find(o => o.id === ordenId);
    if (ordenExiste) {
      this.abrirFormularioConOrden();
    } else {
      this.notificacion.error(`La orden #${ordenId} no está disponible para agregar pedidos (debe estar en estado Borrador o Aprobada sin pedidos).`);
    }
    return;
  }

  // Esperar a que carguen
  let intentos = 0;
  const intervalo = setInterval(() => {
    intentos++;
    if (this.ordenes.length > 0 || intentos > 25) {
      clearInterval(intervalo);
      if (this.ordenes.length > 0) {
        const ordenExiste = this.ordenes.find(o => o.id === ordenId);
        if (ordenExiste) {
          this.abrirFormularioConOrden();
        } else {
          this.notificacion.error(`La orden #${ordenId} no está disponible para agregar pedidos.`);
        }
      } else {
        this.notificacion.error('No se pudieron cargar las órdenes disponibles.');
      }
    }
  }, 200);
}

  aplicarFiltro(event: Event): void {
    const valor = (event.target as HTMLInputElement).value;
    this.dataSource.filter = valor.trim().toLowerCase();
  }

  cargarCatalogos(): void {
    this.departamentoService.listar().subscribe({
      next: (data) => { this.departamentos = data; this.cdr.detectChanges(); }
    });

    this.ordenCompraService.listarConContadores().subscribe({
      next: (data) => {
        this.ordenes = data.filter(o =>
          (o.estado === 'Borrador' || o.estado === 'Aprobada')
          && o.totalPedidos === 0
        );
        this.cdr.detectChanges();
      },
      error: (err) => {
        console.error('Error al cargar órdenes:', err);
      }
    });
  }

  cargar(): void {
    this.cargando = true;
    this.error = '';
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

  // ===== IR A ADJUDICAR =====
  irAAdjudicar(pedido: PedidoInterno): void {
    if (!pedido.id_OrdenCompra) {
      this.notificacion.error('Este pedido no tiene una orden asignada.');
      return;
    }

    if (this.authService.esAdmin()) {
      this.router.navigate(['/admin/adjudicaciones'], {
        queryParams: { ordenId: pedido.id_OrdenCompra }
      });
    } else {
      this.router.navigate(['/empleado/adjudicaciones'], {
        queryParams: { ordenId: pedido.id_OrdenCompra }
      });
    }
  }

  // ===== PERMISOS =====
  puedeCrearPedidos(): boolean {
    return this.authService.puedeCrearPedidos();
  }

  puedeEditarPedidos(): boolean {
    return this.authService.puedeEditarPedidos();
  }

  puedeEliminarPedidos(): boolean {
    // ✅ Admin y Gestor pueden eliminar
    return this.authService.esAdmin() || this.authService.esGestorCompras();
  }

  puedeEliminarEstePedido(pedido: PedidoInterno): boolean {
    // ❌ No se puede eliminar un pedido ya adjudicado
    if (pedido.adjudicado) return false;
    return this.puedeEliminarPedidos();
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
      return `Adjudicado a ${pedido.nombreProveedorGanador || 'Proveedor #' + pedido.idProveedorGanador}\nPrecio: Q ${(pedido.precioAdjudicado || 0).toFixed(2)}`;
    }
    if (!pedido.id_OrdenCompra) return 'Sin orden de compra asignada';
    if (pedido.totalOfertas > 0) return `${pedido.totalOfertas} oferta(s) recibida(s). Pendiente de adjudicar.`;
    return 'Sin ofertas aún';
  }
}