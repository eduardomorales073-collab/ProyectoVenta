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
import { PedidoInternoService } from '../../../services/pedido-interno.service';
import { DepartamentoService } from '../../../services/departamento.service';
import { OrdenCompraService } from '../../../services/orden-compra.service';
import { PedidoInterno } from '../../../models/pedido-interno.model';
import { Departamento } from '../../../models/departamento.model';
import { OrdenCompra } from '../../../services/orden-compra.service';
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
  displayedColumns: string[] = ['id', 'departamento', 'orden', 'fecha_Solicitada', 'fecha_Ingreso', 'acciones'];
  dataSource = new MatTableDataSource<PedidoInterno>([]);
  departamentos: Departamento[] = [];
  ordenes: OrdenCompra[] = [];
  cargando = false;
  error = '';
  mostrarFormulario = false;
  pedidoSeleccionado: PedidoInterno | null = null;

  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;

  constructor(
    private pedidoService: PedidoInternoService,
    private departamentoService: DepartamentoService,
    private ordenCompraService: OrdenCompraService,
    private cdr: ChangeDetectorRef,
    private notificacion: NotificacionService,
    private confirm: ConfirmService
  ) { }

  ngOnInit(): void { }

  ngAfterViewInit(): void {
    this.dataSource.paginator = this.paginator;
    this.dataSource.sort = this.sort;
    this.dataSource.filterPredicate = (pedido: PedidoInterno, filtro: string) => {
      const dep = this.nombreDepartamento(pedido.id_Departamento).toLowerCase();
      const orden = this.nombreOrden(pedido.id_OrdenCompra).toLowerCase();
      const dataStr = (pedido.id + ' ' + dep + ' ' + orden).toLowerCase();
      return dataStr.includes(filtro);
    };
    this.cargarCatalogos();
    this.cargar();
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
  }

  aplicarFiltro(event: Event): void {
    const valor = (event.target as HTMLInputElement).value;
    this.dataSource.filter = valor.trim().toLowerCase();
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

  cerrarFormulario(): void {
    this.mostrarFormulario = false;
    this.pedidoSeleccionado = null;
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
}
