import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Articulo, ArticuloService } from '../services/articulo.service';

@Component({
  selector: 'app-articulos',
  standalone: true,
  imports: [CommonModule],
  template: `
    <h2>Lista de Artículos</h2>

    <p *ngIf="cargando">Cargando...</p>
    <p *ngIf="error" style="color: red">{{ error }}</p>

    <table *ngIf="!cargando && !error" border="1" cellpadding="8">
      <thead>
        <tr>
          <th>ID</th>
          <th>Nombre</th>
          <th>Descripción</th>
        </tr>
      </thead>
      <tbody>
        <tr *ngFor="let a of articulos">
          <td>{{ a.id }}</td>
          <td>{{ a.nombre }}</td>
          <td>{{ a.descripcion }}</td>
        </tr>
      </tbody>
    </table>
  `
})
export class ArticulosComponent implements OnInit {
  articulos: Articulo[] = [];
  cargando = false;
  error = '';

  constructor(
    private articuloService: ArticuloService,
    private cdr: ChangeDetectorRef    // ← AÑADIR
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
        this.cdr.detectChanges();     // ← AÑADIR: fuerza la actualización de la vista
      },
      error: (err) => {
        this.error = `Error: ${err.status} ${err.statusText}`;
        this.cargando = false;
        this.cdr.detectChanges();     // ← AÑADIR
      }
    });
  }
}
