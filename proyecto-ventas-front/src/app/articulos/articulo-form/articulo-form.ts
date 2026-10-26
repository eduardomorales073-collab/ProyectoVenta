import { Component, EventEmitter, Input, Output, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';

import { Articulo, ArticuloService, CreateArticuloDTO, UpdateArticuloDTO } from '../../services/articulo.service';
import { RubroService } from '../../services/rubro.service';
import { Rubro } from '../../models/rubro.model';
import { UnidadMedidaService } from '../../services/unidad-medida.service';
import { UnidadMedida } from '../../models/unidad-medida.model';
import { NotificacionService } from '../../services/notificacion';

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
  esEdicion = false;

  rubros: Rubro[] = [];
  unidadesMedida: UnidadMedida[] = [];

  constructor(
    private fb: FormBuilder,
    private articuloService: ArticuloService,
    private rubroService: RubroService,
    private unidadMedidaService: UnidadMedidaService,
    private notification: NotificacionService
  ) {
    this.form = this.fb.group({
      codigo: [{ value: '', disabled: true }],
      nombre: ['', [Validators.required]],
      descripcion: ['', [Validators.required]],
      unidad_medida: ['', [Validators.required]],
      id_Rubro: [null]
    });
  }

  ngOnInit(): void {
    this.esEdicion = !!this.articulo;

    // Cargar Rubros
    this.rubroService.listar().subscribe({
      next: (data: any) => {
        this.rubros = Array.isArray(data) ? data : (data.$values || []);
      }
    });

    // Cargar Unidades de Medida
    this.unidadMedidaService.listar().subscribe({
      next: (data: any) => {
        this.unidadesMedida = Array.isArray(data) ? data : (data.$values || []);
      },
      error: () => {
        this.unidadesMedida = [{ id: 1, nombre: 'Unidad' } as any];
      }
    });

    if (this.articulo) {
      this.form.patchValue({
        codigo: this.articulo.codigo || '',
        nombre: this.articulo.nombre,
        descripcion: this.articulo.descripcion,
        unidad_medida: this.articulo.unidad_medida || ''
      });
    } else {
      this.generarCodigo();
    }
  }

  generarCodigo(): void {
    this.articuloService.listar().subscribe({
      next: (articulos) => {
        const maxId = articulos.length > 0 ? Math.max(...articulos.map(a => a.id)) : 0;
        this.form.patchValue({ codigo: `ART-${String(maxId + 1).padStart(3, '0')}` });
      },
      error: () => this.form.patchValue({ codigo: 'ART-' + Date.now().toString().slice(-4) })
    });
  }

  onSubmit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.guardando = true;
    const datos = this.form.getRawValue();

    if (this.articulo) {
      const dto: UpdateArticuloDTO = {
        id: this.articulo.id,
        nombre: datos.nombre,
        descripcion: datos.descripcion,
        codigo: this.articulo.codigo,
        unidad_medida: datos.unidad_medida,
        id_Rubro: datos.id_Rubro
      };

      this.articuloService.actualizar(dto).subscribe({
        next: () => this.finalizar(),
        error: (err) => this.error(err)
      });
    } else {
      const dto: CreateArticuloDTO = {
        nombre: datos.nombre,
        descripcion: datos.descripcion,
        codigo: datos.codigo,
        unidad_medida: datos.unidad_medida,
        id_Rubro: datos.id_Rubro
      };

      this.articuloService.crear(dto).subscribe({
        next: () => this.finalizar(),
        error: (err) => this.error(err)
      });
    }
  }

  private finalizar() {
    this.guardando = false;
    this.notification.exito(this.esEdicion ? 'Artículo actualizado' : 'Artículo creado');
    this.guardado.emit();
  }

  private error(err: any) {
    this.guardando = false;
    this.notification.error(err.error?.mensaje || 'Error al guardar');
  }

  // ✅ CORREGIDO: Ahora se llama onCancel() igual que en el HTML
  onCancel(): void {
    this.cancelado.emit();
  }
}
