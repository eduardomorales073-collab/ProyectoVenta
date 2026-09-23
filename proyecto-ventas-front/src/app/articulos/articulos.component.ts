import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Articulo, ArticuloService } from '../services/articulo.service';
import { ArticuloFormComponent } from './articulo-form/articulo-form';

@Component({
  selector: 'app-articulos',
  standalone: true,
  imports: [CommonModule, ArticuloFormComponent],
  template: `
    <div class="container">
      <div class="header-actions">
        <h2>Lista de Artículos</h2>
        <button class="btn-nuevo" (click)="abrirFormulario(null)">
          ➕ Nuevo Artículo
        </button>
      </div>

      <p *ngIf="cargando">Cargando...</p>
      <p *ngIf="error" class="error">{{ error }}</p>

      <table *ngIf="!cargando && !error" class="tabla">
        <thead>
          <tr>
            <th>ID</th>
            <th>Nombre</th>
            <th>Descripción</th>
            <th>Acciones</th>
          </tr>
        </thead>
        <tbody>
          <tr *ngFor="let a of articulos">
            <td>{{ a.id }}</td>
            <td>{{ a.nombre }}</td>
            <td>{{ a.descripcion }}</td>
            <td class="acciones">
              <button class="btn-editar" (click)="abrirFormulario(a)" title="Editar">
                ✏️
              </button>
              <button class="btn-eliminar" (click)="confirmarEliminar(a)" title="Eliminar">
                🗑️
              </button>
            </td>
          </tr>
        </tbody>
      </table>
    </div>

    <!-- Modal del formulario -->
    <app-articulo-form
      *ngIf="mostrarFormulario"
      [articulo]="articuloSeleccionado"
      (guardado)="onGuardado()"
      (cancelado)="cerrarFormulario()">
    </app-articulo-form>
  `,
  styles: [`
    .container { padding: 1.5rem; max-width: 1200px; margin: 0 auto; }

    .header-actions {
      display: flex;
      justify-content: space-between;
      align-items: center;
      margin-bottom: 1.5rem;
    }

    h2 { margin: 0; color: #1e293b; }

    .btn-nuevo {
      background: #3b82f6;
      color: #fff;
      border: none;
      padding: 0.625rem 1.25rem;
      border-radius: 0.5rem;
      font-weight: 600;
      cursor: pointer;
      transition: all 0.2s;
    }
    .btn-nuevo:hover { background: #2563eb; }

    .tabla {
      width: 100%;
      border-collapse: collapse;
      background: #fff;
      border-radius: 0.5rem;
      overflow: hidden;
      box-shadow: 0 1px 3px rgba(0,0,0,0.1);
    }

    .tabla th {
      background: #f1f5f9;
      padding: 0.75rem 1rem;
      text-align: left;
      font-weight: 600;
      color: #334155;
      border-bottom: 2px solid #e2e8f0;
    }

    .tabla td {
      padding: 0.75rem 1rem;
      border-bottom: 1px solid #e2e8f0;
      color: #475569;
    }

    .tabla tr:hover { background: #f8fafc; }

    .acciones {
      display: flex;
      gap: 0.5rem;
    }

    .btn-editar, .btn-eliminar {
      background: transparent;
      border: 1px solid #e2e8f0;
      padding: 0.375rem 0.625rem;
      border-radius: 0.375rem;
      cursor: pointer;
      font-size: 1rem;
      transition: all 0.2s;
    }

    .btn-editar:hover {
      background: #dbeafe;
      border-color: #3b82f6;
    }

    .btn-eliminar:hover {
      background: #fee2e2;
      border-color: #ef4444;
    }

    .error { color: #ef4444; }
  `]
})
export class ArticulosComponent implements OnInit {
  articulos: Articulo[] = [];
  cargando = false;
  error = '';
  mostrarFormulario = false;
  articuloSeleccionado: Articulo | null = null;

  constructor(
    private articuloService: ArticuloService,
    private cdr: ChangeDetectorRef
  ) { }

  ngOnInit(): void {
    this.cargar();
  }

  cargar(): void {
    this.cargando = true;
    this.error = '';
    this.articuloService.listar().subscribe({
      next: (data) => {
        this.articulos = data;
        this.cargando = false;
        this.cdr.detectChanges();
      },
      error: (err) => {
        this.error = `Error: ${err.status} ${err.statusText}`;
        this.cargando = false;
        this.cdr.detectChanges();
      }
    });
  }

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
      error: (err) => {
        alert(`Error al eliminar: ${err.status} ${err.statusText}`);
      }
    });
  }
}
