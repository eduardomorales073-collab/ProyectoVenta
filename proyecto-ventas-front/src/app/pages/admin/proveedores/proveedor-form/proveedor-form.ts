import { Component, EventEmitter, Input, Output, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatChipsModule } from '@angular/material/chips';
import { ProveedorService } from '../../../../services/proveedor.service';
import { Proveedor, CreateProveedorDTO } from '../../../../models/proveedor.model';
import { Rubro } from '../../../../models/rubro.model';
import { NotificacionService } from '../../../../services/notificacion';

@Component({
  selector: 'app-proveedor-form',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatButtonModule,
    MatIconModule,
    MatChipsModule
  ],
  templateUrl: './proveedor-form.html',
  styleUrl: './proveedor-form.scss'
})
export class ProveedorFormComponent implements OnInit {
  @Input() proveedor: Proveedor | null = null;
  @Input() rubros: Rubro[] = [];
  @Output() guardado = new EventEmitter<void>();
  @Output() cancelado = new EventEmitter<void>();

  form: FormGroup;
  guardando = false;

  constructor(
    private fb: FormBuilder,
    private proveedorService: ProveedorService,
    private notificacion: NotificacionService
  ) {
    this.form = this.fb.group({
      nombre: ['', [Validators.required, Validators.maxLength(100)]],
      descripcion: ['', [Validators.required, Validators.maxLength(200)]],
      telefono: ['', [Validators.required, Validators.maxLength(20)]],
      direccion: ['', [Validators.required, Validators.maxLength(200)]],
      id_Rubros: [[], [Validators.required]]
    });
  }

  ngOnInit(): void {
    if (this.proveedor) {
      this.form.patchValue({
        nombre: this.proveedor.nombre,
        descripcion: this.proveedor.descripcion,
        telefono: this.proveedor.telefono,
        direccion: this.proveedor.direccion,
        id_Rubros: this.proveedor.id_Rubros || []
      });
    }
  }

  get esEdicion(): boolean {
    return !!this.proveedor;
  }

  onSubmit(): void {
    if (this.form.invalid) return;

    this.guardando = true;
    const datos = this.form.value;

    if (this.esEdicion) {
      const dto = {
        id: this.proveedor!.id,
        nombre: datos.nombre,
        descripcion: datos.descripcion,
        telefono: datos.telefono,
        direccion: datos.direccion,
        id_Rubros: datos.id_Rubros
      };
      this.proveedorService.actualizar(dto).subscribe({
        next: () => {
          this.guardando = false;
          this.notificacion.exito('Proveedor actualizado correctamente');
          this.guardado.emit();
        },
        error: (err: any) => {
          this.guardando = false;
          this.notificacion.error(`Error: ${err.status} ${err.statusText}`);
        }
      });
    } else {
      const dto: CreateProveedorDTO = {
        nombre: datos.nombre,
        descripcion: datos.descripcion,
        telefono: datos.telefono,
        direccion: datos.direccion,
        id_Rubros: datos.id_Rubros
      };
      this.proveedorService.crear(dto).subscribe({
        next: () => {
          this.guardando = false;
          this.notificacion.exito('Proveedor creado correctamente');
          this.guardado.emit();
        },
        error: (err: any) => {
          this.guardando = false;
          this.notificacion.error(`Error: ${err.status} ${err.statusText}`);
        }
      });
    }
  }

  onCancelar(): void {
    this.cancelado.emit();
  }
}
