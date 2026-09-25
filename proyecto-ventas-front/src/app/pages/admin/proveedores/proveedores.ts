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
import { ProveedorService } from '../../../services/proveedor.service';
import { RubroService } from '../../../services/rubro.service';
import { Proveedor } from '../../../models/proveedor.model';
import { Rubro } from '../../../models/rubro.model';
import { ProveedorFormComponent } from './proveedor-form/proveedor-form';
import { ConfirmService } from '../../../services/confirm';
import { NotificacionService } from '../../../services/notificacion';

@Component({
  selector: 'app-proveedores',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    ProveedorFormComponent,
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
  templateUrl: './proveedores.html',
  styleUrl: './proveedores.scss'
})
export class ProveedoresComponent implements OnInit, AfterViewInit {
  displayedColumns: string[] = ['id', 'nombre', 'telefono', 'direccion', 'rubros', 'acciones'];
  dataSource = new MatTableDataSource<Proveedor>([]);

  rubros: Rubro[] = [];
  cargando = false;
  error = '';
  mostrarFormulario = false;
  proveedorSeleccionado: Proveedor | null = null;

  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;

  constructor(
    private proveedorService: ProveedorService,
    private rubroService: RubroService,
    private cdr: ChangeDetectorRef,
    private notificacion: NotificacionService,
    private confirm: ConfirmService
  ) { }

  ngOnInit(): void {
  }

  ngAfterViewInit(): void {
    this.dataSource.paginator = this.paginator;
    this.dataSource.sort = this.sort;

    this.dataSource.filterPredicate = (proveedor: Proveedor, filtro: string) => {
      const rubrosNombres = this.nombresRubros(proveedor.id_Rubros).toLowerCase();
      const dataStr = (
        proveedor.id + ' ' +
        proveedor.nombre + ' ' +
        proveedor.telefono + ' ' +
        proveedor.direccion + ' ' +
        rubrosNombres
      ).toLowerCase();
      return dataStr.includes(filtro);
    };

    this.cargarRubros();
    this.cargar();
  }

  cargarRubros(): void {
    this.rubroService.listar().subscribe({
      next: (data) => {
        this.rubros = data;
        this.cdr.detectChanges();
      }
    });
  }

  cargar(): void {
    this.cargando = true;
    this.error = '';
    this.proveedorService.listar().subscribe({
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

  nombresRubros(ids: number[]): string {
    if (!ids || ids.length === 0) return 'Sin rubros';
    return ids
      .map(id => this.rubros.find(r => r.id === id)?.nombre || 'Desconocido')
      .join(', ');
  }

  abrirFormulario(proveedor: Proveedor | null): void {
    this.proveedorSeleccionado = proveedor;
    this.mostrarFormulario = true;
    this.cdr.detectChanges();
  }

  cerrarFormulario(): void {
    this.mostrarFormulario = false;
    this.proveedorSeleccionado = null;
    this.cdr.detectChanges();
  }

  onGuardado(): void {
    this.cerrarFormulario();
    this.cargar();
  }

  confirmarEliminar(proveedor: Proveedor): void {
    this.confirm.eliminar(proveedor.nombre).subscribe((confirmado: boolean) => {
      if (!confirmado) return;

      this.proveedorService.eliminar(proveedor.id).subscribe({
        next: () => {
          this.notificacion.exito(`Proveedor "${proveedor.nombre}" eliminado correctamente`);
          this.cargar();
        },
        error: (err: any) => {
          this.notificacion.error(`Error al eliminar: ${err.status} ${err.statusText}`);
        }
      });
    });
  }
}
