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
import { DepartamentoService } from '../../../services/departamento.service';
import { SucursalService } from '../../../services/sucursal.service';
import { Departamento } from '../../../models/departamento.model';
import { Sucursal } from '../../../models/sucursal.model';
import { DepartamentoFormComponent } from './departamento-form/departamento-form';
import { ConfirmService } from '../../../services/confirm';
import { NotificacionService } from '../../../services/notificacion';

@Component({
  selector: 'app-departamentos',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    DepartamentoFormComponent,
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
  templateUrl: './departamentos.html',
  styleUrl: './departamentos.scss'
})
export class DepartamentosComponent implements OnInit, AfterViewInit {
  displayedColumns: string[] = ['id', 'nombre', 'descripcion', 'sucursal', 'acciones'];
  dataSource = new MatTableDataSource<Departamento>([]);

  sucursales: Sucursal[] = [];
  cargando = false;
  error = '';
  mostrarFormulario = false;
  departamentoSeleccionado: Departamento | null = null;

  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;

  constructor(
    private departamentoService: DepartamentoService,
    private sucursalService: SucursalService,
    private cdr: ChangeDetectorRef,
    private notificacion: NotificacionService,
    private confirm: ConfirmService
  ) { }

  ngOnInit(): void {
  }

  ngAfterViewInit(): void {
    this.dataSource.paginator = this.paginator;
    this.dataSource.sort = this.sort;

    this.dataSource.filterPredicate = (dep: Departamento, filtro: string) => {
      const sucursal = this.nombreSucursal(dep.id_Sucursal).toLowerCase();
      const dataStr = (
        dep.id + ' ' + dep.nombre + ' ' + dep.descripcion + ' ' + sucursal
      ).toLowerCase();
      return dataStr.includes(filtro);
    };

    this.cargarSucursales();
    this.cargar();
  }

  cargarSucursales(): void {
    this.sucursalService.listar().subscribe({
      next: (data) => {
        this.sucursales = data;
      }
    });
  }

  cargar(): void {
    this.cargando = true;
    this.error = '';
    this.departamentoService.listar().subscribe({
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

  nombreSucursal(idSucursal: number): string {
    const sucursal = this.sucursales.find(s => s.id === idSucursal);
    return sucursal ? sucursal.nombre : 'Desconocida';
  }

  abrirFormulario(departamento: Departamento | null): void {
    this.departamentoSeleccionado = departamento;
    this.mostrarFormulario = true;
    this.cdr.detectChanges();
  }

  cerrarFormulario(): void {
    this.mostrarFormulario = false;
    this.departamentoSeleccionado = null;
    this.cdr.detectChanges();
  }

  onGuardado(): void {
    this.cerrarFormulario();
    this.cargar();
  }

  confirmarEliminar(departamento: Departamento): void {
    this.confirm.eliminar(departamento.nombre).subscribe((confirmado: boolean) => {
      if (!confirmado) return;

      this.departamentoService.eliminar(departamento.id).subscribe({
        next: () => {
          this.notificacion.exito(`Departamento "${departamento.nombre}" eliminado correctamente`);
          this.cargar();
        },
        error: (err: any) => {
          this.notificacion.error(`Error al eliminar: ${err.status} ${err.statusText}`);
        }
      });
    });
  }
}
