import { Component, EventEmitter, Input, Output, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { CategoriaProveedorService } from '../../../../services/categoria-proveedor.service';
import { CategoriaProveedor, CreateCategoriaProveedorDTO } from '../../../../models/categoria-proveedor.model';
import { NotificacionService } from '../../../../services/notificacion';

@Component({
  selector: 'app-categoria-form',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule
  ],
  templateUrl: './categoria-form.html',
  styleUrl: './categoria-form.scss'
})
export class CategoriaFormComponent implements OnInit {
  @Input() categoria: CategoriaProveedor | null = null;
  @Output() guardado = new EventEmitter<void>();
  @Output() cancelado = new EventEmitter<void>();

  form: FormGroup;
  guardando = false;

  constructor(
    private fb: FormBuilder,
    private categoriaService: CategoriaProveedorService,
    private notificacion: NotificacionService
  ) {
    this.form = this.fb.group({
      nombre: ['', [Validators.required, Validators.maxLength(50)]],
      descripcion: ['', [Validators.maxLength(200)]]
    });
  }

  ngOnInit(): void {
    if (this.categoria) {
      this.form.patchValue({
        nombre: this.categoria.nombre,
        descripcion: this.categoria.descripcion
      });
    }
  }

  get esEdicion(): boolean {
    return !!this.categoria;
  }

  onSubmit(): void {
    if (this.form.invalid) return;

    this.guardando = true;
    const datos = this.form.value;

    if (this.esEdicion) {
      const dto = {
        id: this.categoria!.id,
        nombre: datos.nombre,
        descripcion: datos.descripcion || ''
      };

      this.categoriaService.actualizar(dto).subscribe({
        next: () => {
          this.guardando = false;
          this.notificacion.exito('Categoría actualizada correctamente');
          this.guardado.emit();
        },
        error: (err: any) => {
          this.guardando = false;
          this.notificacion.error(`Error al actualizar: ${err.status} ${err.statusText}`);
        }
      });
    } else {
      const dto: CreateCategoriaProveedorDTO = {
        nombre: datos.nombre,
        descripcion: datos.descripcion || ''
      };

      this.categoriaService.crear(dto).subscribe({
        next: () => {
          this.guardando = false;
          this.notificacion.exito('Categoría creada correctamente');
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