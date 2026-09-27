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
import { UnidadMedidaService } from '../../../services/unidad-medida.service';
import { UnidadMedida } from '../../../models/unidad-medida.model';
import { UnidadFormComponent } from './unidad-form/unidad-form';
import { ConfirmService } from '../../../services/confirm';
import { NotificacionService } from '../../../services/notificacion';

@Component({
  selector: 'app-unidades-medida',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    UnidadFormComponent,
    MatTableModule,
    MatPaginatorModule,
    MatSortModule,
    MatFormFieldModule,
    MatInputModule,
    MatIconModule,
    MatButtonModule,
    MatTooltipModule
  ],
  templateUrl: './unidades-medida.html',
  styleUrl: './unidades-medida.scss'
})
export class UnidadesMedidaComponent implements OnInit, AfterViewInit {
  displayedColumns: string[] = ['id', 'nombre', 'abreviatura', 'descripcion', 'acciones'];
  dataSource = new MatTableDataSource<UnidadMedida>([]);

  cargando = false;
  error = '';
  mostrarFormulario = false;
  unidadSeleccionada: UnidadMedida | null = null;

  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;

  constructor(
    private unidadService: UnidadMedidaService,
    private cdr: ChangeDetectorRef,
    private notificacion: NotificacionService,
    private confirm: ConfirmService
  ) { }

  ngOnInit(): void { }

  ngAfterViewInit(): void {
    this.dataSource.paginator = this.paginator;
    this.dataSource.sort = this.sort;

    this.dataSource.filterPredicate = (u: UnidadMedida, filtro: string) => {
      const dataStr = (u.id + ' ' + u.nombre + ' ' + u.abreviatura + ' ' + u.descripcion).toLowerCase();
      return dataStr.includes(filtro);
    };

    this.cargar();
  }

  cargar(): void {
    this.cargando = true;
    this.error = '';
    this.unidadService.listar().subscribe({
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

  abrirFormulario(unidad: UnidadMedida | null): void {
    this.unidadSeleccionada = unidad;
    this.mostrarFormulario = true;
    this.cdr.detectChanges();
  }

  cerrarFormulario(): void {
    this.mostrarFormulario = false;
    this.unidadSeleccionada = null;
    this.cdr.detectChanges();
  }

  onGuardado(): void {
    this.cerrarFormulario();
    this.cargar();
  }

  confirmarEliminar(unidad: UnidadMedida): void {
    this.confirm.eliminar(unidad.nombre).subscribe((confirmado: boolean) => {
      if (!confirmado) return;

      this.unidadService.eliminar(unidad.id).subscribe({
        next: () => {
          this.notificacion.exito(`Unidad "${unidad.nombre}" eliminada correctamente`);
          this.cargar();
        },
        error: (err: any) => {
          this.notificacion.error(`Error al eliminar: ${err.status} ${err.statusText}`);
        }
      });
    });
  }
}