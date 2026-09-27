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
import { CategoriaProveedorService } from '../../../services/categoria-proveedor.service';
import { CategoriaProveedor } from '../../../models/categoria-proveedor.model';
import { CategoriaFormComponent } from './categoria-form/categoria-form';
import { ConfirmService } from '../../../services/confirm';
import { NotificacionService } from '../../../services/notificacion';

@Component({
  selector: 'app-categorias-proveedor',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    CategoriaFormComponent,
    MatTableModule,
    MatPaginatorModule,
    MatSortModule,
    MatFormFieldModule,
    MatInputModule,
    MatIconModule,
    MatButtonModule,
    MatTooltipModule
  ],
  templateUrl: './categorias-proveedor.html',
  styleUrl: './categorias-proveedor.scss'
})
export class CategoriasProveedorComponent implements OnInit, AfterViewInit {
  displayedColumns: string[] = ['id', 'nombre', 'descripcion', 'acciones'];
  dataSource = new MatTableDataSource<CategoriaProveedor>([]);

  cargando = false;
  error = '';
  mostrarFormulario = false;
  categoriaSeleccionada: CategoriaProveedor | null = null;

  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;

  constructor(
    private categoriaService: CategoriaProveedorService,
    private cdr: ChangeDetectorRef,
    private notificacion: NotificacionService,
    private confirm: ConfirmService
  ) { }

  ngOnInit(): void { }

  ngAfterViewInit(): void {
    this.dataSource.paginator = this.paginator;
    this.dataSource.sort = this.sort;

    this.dataSource.filterPredicate = (cat: CategoriaProveedor, filtro: string) => {
      const dataStr = (cat.id + ' ' + cat.nombre + ' ' + cat.descripcion).toLowerCase();
      return dataStr.includes(filtro);
    };

    this.cargar();
  }

  cargar(): void {
    this.cargando = true;
    this.error = '';
    this.categoriaService.listar().subscribe({
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

  abrirFormulario(categoria: CategoriaProveedor | null): void {
    this.categoriaSeleccionada = categoria;
    this.mostrarFormulario = true;
    this.cdr.detectChanges();
  }

  cerrarFormulario(): void {
    this.mostrarFormulario = false;
    this.categoriaSeleccionada = null;
    this.cdr.detectChanges();
  }

  onGuardado(): void {
    this.cerrarFormulario();
    this.cargar();
  }

  confirmarEliminar(categoria: CategoriaProveedor): void {
    this.confirm.eliminar(categoria.nombre).subscribe((confirmado: boolean) => {
      if (!confirmado) return;

      this.categoriaService.eliminar(categoria.id).subscribe({
        next: () => {
          this.notificacion.exito(`Categoría "${categoria.nombre}" eliminada correctamente`);
          this.cargar();
        },
        error: (err: any) => {
          this.notificacion.error(`Error al eliminar: ${err.status} ${err.statusText}`);
        }
      });
    });
  }
}