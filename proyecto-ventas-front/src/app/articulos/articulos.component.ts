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
import { Articulo, ArticuloService } from '../services/articulo.service';
import { ArticuloFormComponent } from './articulo-form/articulo-form';
import { AuthService } from '../services/auth.service';

@Component({
  selector: 'app-articulos',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    ArticuloFormComponent,
    MatTableModule,
    MatPaginatorModule,
    MatSortModule,
    MatFormFieldModule,
    MatInputModule,
    MatIconModule,
    MatButtonModule,
    MatTooltipModule
  ],
  templateUrl: './articulos.component.html',
  styleUrl: './articulos.component.scss'
})
export class ArticulosComponent implements OnInit, AfterViewInit {
  displayedColumns: string[] = ['id', 'nombre', 'descripcion', 'acciones'];
  dataSource = new MatTableDataSource<Articulo>([]);

  cargando = false;
  error = '';
  mostrarFormulario = false;
  articuloSeleccionado: Articulo | null = null;

  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;

  constructor(
    private articuloService: ArticuloService,
    private cdr: ChangeDetectorRef,
    private authService: AuthService
  ) { }

  ngOnInit(): void {
    // vacío
  }

  ngAfterViewInit(): void {
    this.dataSource.paginator = this.paginator;
    this.dataSource.sort = this.sort;

    // ✅ Filtro personalizado
    this.dataSource.filterPredicate = (articulo: Articulo, filtro: string) => {
      const dataStr = (
        articulo.id + ' ' +
        articulo.nombre + ' ' +
        articulo.descripcion
      ).toLowerCase();

      return dataStr.includes(filtro);
    };

    this.cargar();
  }

  cargar(): void {
    this.cargando = true;
    this.error = '';
    this.articuloService.listar().subscribe({
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

  // ====== PERMISOS ======
  puedeCrear(): boolean {
    return this.authService.puedeCrear();
  }

  puedeEditar(): boolean {
    return this.authService.puedeEditar();
  }

  puedeEliminar(): boolean {
    return this.authService.puedeEliminar();
  }

  // ====== CRUD ======
  abrirFormulario(articulo: Articulo | null): void {
    this.articuloSeleccionado = articulo;
    this.mostrarFormulario = true;
    this.cdr.detectChanges();
  }

  cerrarFormulario(): void {
    this.mostrarFormulario = false;
    this.articuloSeleccionado = null;
    this.cdr.detectChanges();
  }

  onGuardado(): void {
    this.cerrarFormulario();
    this.cargar();
  }

  confirmarEliminar(articulo: Articulo): void {
    if (!confirm(`¿Eliminar "${articulo.nombre}"?`)) return;

    this.articuloService.eliminar(articulo.id).subscribe({
      next: () => this.cargar(),
      error: (err: any) => {
        alert(`Error al eliminar: ${err.status} ${err.statusText}`);
      }
    });
  }
}
