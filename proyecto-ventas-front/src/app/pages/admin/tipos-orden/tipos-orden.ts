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
import { TipoOrdenService } from '../../../services/tipo-orden.service';
import { TipoOrden } from '../../../models/tipo-orden.model';
import { TipoOrdenFormComponent } from './tipo-orden-form/tipo-orden-form';
import { ConfirmService } from '../../../services/confirm';
import { NotificacionService } from '../../../services/notificacion';

@Component({
  selector: 'app-tipos-orden',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    TipoOrdenFormComponent,
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
  templateUrl: './tipos-orden.html',
  styleUrl: './tipos-orden.scss'
})
export class TiposOrdenComponent implements OnInit, AfterViewInit {
  displayedColumns: string[] = ['id', 'grande', 'urgente', 'acciones'];
  dataSource = new MatTableDataSource<TipoOrden>([]);

  cargando = false;
  error = '';
  mostrarFormulario = false;
  tipoSeleccionado: TipoOrden | null = null;

  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;

  constructor(
    private tipoOrdenService: TipoOrdenService,
    private cdr: ChangeDetectorRef,
    private notificacion: NotificacionService,
    private confirm: ConfirmService
  ) { }

  ngOnInit(): void {
  }

  ngAfterViewInit(): void {
    this.dataSource.paginator = this.paginator;
    this.dataSource.sort = this.sort;

    this.dataSource.filterPredicate = (tipo: TipoOrden, filtro: string) => {
      const dataStr = (
        tipo.id + ' ' +
        (tipo.grande ? 'grande' : 'normal') + ' ' +
        (tipo.urgente ? 'urgente' : 'no urgente')
      ).toLowerCase();
      return dataStr.includes(filtro);
    };

    this.cargar();
  }

  cargar(): void {
    this.cargando = true;
    this.error = '';
    this.tipoOrdenService.listar().subscribe({
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

  abrirFormulario(tipo: TipoOrden | null): void {
    this.tipoSeleccionado = tipo;
    this.mostrarFormulario = true;
    this.cdr.detectChanges();
  }

  cerrarFormulario(): void {
    this.mostrarFormulario = false;
    this.tipoSeleccionado = null;
    this.cdr.detectChanges();
  }

  onGuardado(): void {
    this.cerrarFormulario();
    this.cargar();
  }

  confirmarEliminar(tipo: TipoOrden): void {
    const descripcion = `TipoOrden #${tipo.id} (${tipo.grande ? 'Grande' : 'Normal'}, ${tipo.urgente ? 'Urgente' : 'No urgente'})`;
    this.confirm.eliminar(descripcion).subscribe((confirmado: boolean) => {
      if (!confirmado) return;

      this.tipoOrdenService.eliminar(tipo.id).subscribe({
        next: () => {
          this.notificacion.exito(`TipoOrden eliminado correctamente`);
          this.cargar();
        },
        error: (err: any) => {
          this.notificacion.error(`Error al eliminar: ${err.status} ${err.statusText}`);
        }
      });
    });
  }
}
