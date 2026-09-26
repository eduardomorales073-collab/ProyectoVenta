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
import { Adjudicacion } from '../../../models/adjudicacion.model';
import { AdjudicacionFormComponent } from './adjudicacion-form/adjudicacion-form';
import { ConfirmService } from '../../../services/confirm';
import { NotificacionService } from '../../../services/notificacion';

@Component({
  selector: 'app-adjudicaciones',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    AdjudicacionFormComponent,
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
  displayedColumns: string[] = ['id', 'fecha_Resolucion', 'orden_Compra', 'estado', 'acciones'];
  dataSource = new MatTableDataSource<Adjudicacion>([]);
  cargando = false;
  error = '';
  mostrarFormulario = false;
  adjudicacionSeleccionada: Adjudicacion | null = null;

  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;

  constructor(
    private adjudicacionService: AdjudicacionService,
    private cdr: ChangeDetectorRef,
    private notificacion: NotificacionService,
    private confirm: ConfirmService
  ) { }

  ngOnInit(): void { }

  ngAfterViewInit(): void {
    this.dataSource.paginator = this.paginator;
    this.dataSource.sort = this.sort;
    this.dataSource.filterPredicate = (adj: Adjudicacion, filtro: string) => {
      const dataStr = (adj.id + ' ' + adj.estado + ' ' + adj.orden_Compra).toLowerCase();
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
    this.confirm.eliminar(`Adjudicación #${adjudicacion.id}`).subscribe((confirmado: boolean) => {
      if (!confirmado) return;
      this.adjudicacionService.eliminar(adjudicacion.id).subscribe({
        next: () => {
          this.notificacion.exito(`Adjudicación #${adjudicacion.id} eliminada correctamente`);
          this.cargar();
        },
        error: (err: any) => {
          this.notificacion.error(`Error al eliminar: ${err.error?.mensaje || err.status + ' ' + err.statusText}`);
        }
      });
    });
  }

  colorEstado(estado: string): string {
    switch (estado?.toLowerCase()) {
      case 'activa': return 'chip-activa';
      case 'cerrada': return 'chip-cerrada';
      case 'cancelada': return 'chip-cancelada';
      default: return 'chip-default';
    }
  }
}
