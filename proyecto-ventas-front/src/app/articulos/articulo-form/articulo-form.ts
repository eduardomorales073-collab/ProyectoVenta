import { Component, EventEmitter, Input, Output, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { Articulo, ArticuloService, CreateArticuloDTO } from '../../services/articulo.service';
import { NotificacionService } from '../../services/notificacion';
import { UnidadMedidaService } from '../../services/unidad-medida.service';
import { UnidadMedida } from '../../models/unidad-medida.model';

@Component({
  selector: 'app-articulo-form',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatButtonModule,
    MatIconModule
  ],
  templateUrl: './articulo-form.html',
  styleUrl: './articulo-form.scss'
})
export class ArticuloFormComponent implements OnInit {
  @Input() articulo: Articulo | null = null;
  @Output() guardado = new EventEmitter<void>();
  @Output() cancelado = new EventEmitter<void>();

  form: FormGroup;
  guardando = false;
  error = '';

  // Unidades de medida cargadas dinámicamente desde la BD
  unidadesMedida: UnidadMedida[] = [];

  constructor(
    private fb: FormBuilder,
    private articuloService: ArticuloService,
    private unidadMedidaService: UnidadMedidaService,
    private notificacion: NotificacionService
  ) {
    this.form = this.fb.group({
      codigo: [{ value: '', disabled: true }], // ← auto-generado
      nombre: ['', [Validators.required, Validators.maxLength(100)]],
      descripcion: ['', [Validators.required, Validators.maxLength(200)]],
      unidad_medida: ['Unidad', [Validators.required]]
    });
  }

  ngOnInit(): void {
    this.cargarUnidades();

    if (this.articulo) {
      // Edición: mostrar el código actual
      this.form.patchValue({
        codigo: this.articulo.codigo || '',
        nombre: this.articulo.nombre,
        descripcion: this.articulo.descripcion,
        unidad_medida: this.articulo.unidad_medida || 'Unidad'
      });
    } else {
      // Creación: auto-generar el código
      this.generarCodigo();
    }
  }

  cargarUnidades(): void {
    this.unidadMedidaService.listar().subscribe({
      next: (data) => {
        this.unidadesMedida = data;
      },
      error: (err: any) => {
        this.notificacion.error(`Error al cargar unidades: ${err.status}`);
      }
    });
  }

  get esEdicion(): boolean {
    return !!this.articulo;
  }

  /**
   * Genera un código único basado en el último ID + 1
   * Formato: ART-XXX (ej: ART-008)
   */
  generarCodigo(): void {
    this.articuloService.listar().subscribe({
      next: (articulos) => {
        const maxId = articulos.length > 0
          ? Math.max(...articulos.map(a => a.id))
          : 0;
        const nuevoId = maxId + 1;
        const codigo = 'ART-' + String(nuevoId).padStart(3, '0');
        this.form.patchValue({ codigo });
      },
      error: () => {
        // Si falla, generar con timestamp
        const codigo = 'ART-' + Date.now().toString().slice(-4);
        this.form.patchValue({ codigo });
      }
    });
  }

  onSubmit(): void {
    if (this.form.invalid) return;

    this.guardando = true;
    this.error = '';

    const datos = this.form.getRawValue(); // ← getRawValue para incluir el campo disabled

    if (this.esEdicion) {
      const dto = {
        id: this.articulo!.id,
        nombre: datos.nombre,
        descripcion: datos.descripcion,
        codigo: datos.codigo,
        unidad_medida: datos.unidad_medida
      };
      this.articuloService.actualizar(dto).subscribe({
        next: () => {
          this.guardando = false;
          this.notificacion.exito('Artículo actualizado correctamente');
          this.guardado.emit();
        },
        error: (err: any) => {
          this.guardando = false;
          this.notificacion.error(`Error al actualizar: ${err.status} ${err.statusText}`);
        }
      });
    } else {
      const dto: CreateArticuloDTO = {
        nombre: datos.nombre,
        descripcion: datos.descripcion,
        codigo: datos.codigo,
        unidad_medida: datos.unidad_medida
      };
      this.articuloService.crear(dto).subscribe({
        next: () => {
          this.guardando = false;
          this.notificacion.exito('Artículo creado correctamente');
          this.guardado.emit();
        },
        error: (err: any) => {
          this.guardando = false;
          this.notificacion.error(`Error al crear: ${err.status} ${err.statusText}`);
        }
      });
    }
  }

  onCancelar(): void {
    this.cancelado.emit();
  }
}