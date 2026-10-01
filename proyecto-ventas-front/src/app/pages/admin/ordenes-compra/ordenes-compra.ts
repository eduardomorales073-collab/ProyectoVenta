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
import { MatBadgeModule } from '@angular/material/badge';
import { OrdenCompraService, OrdenCompraConContadores } from '../../../services/orden-compra.service';
import { TipoOrdenService } from '../../../services/tipo-orden.service';
import { TipoOrden } from '../../../models/tipo-orden.model';
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
    MatChipsModule,
    MatBadgeModule
  ],
  templateUrl: './ordenes-compra.html',
  styleUrl: './ordenes-compra.scss'
})
export class OrdenesCompraComponent implements OnInit, AfterViewInit {
  displayedColumns: string[] = ['id', 'descripcion', 'fecha_Creacion', 'fecha_Limite', 'tipo_Orden', 'pedidos', 'estado', 'acciones'];
  dataSource = new MatTableDataSource<OrdenCompraConContadores>([]);

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
    this.router.navigate(['/admin/pedidos-internos'], { queryParams: { ordenId: idOrden } });
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

  // ===== HELPERS PARA EL TIPO DE ORDEN =====
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

  // ===== HELPERS PARA EL ESTADO =====
  claseEstado(estado: string): string {
    return estado === 'Cerrada' ? 'chip-cerrada' : 'chip-abierta';
  }

  iconoEstado(estado: string): string {
    return estado === 'Cerrada' ? 'check_circle' : 'schedule';
  }
}