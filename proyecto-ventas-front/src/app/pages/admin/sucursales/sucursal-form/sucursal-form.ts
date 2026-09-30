import { Component, EventEmitter, Input, Output, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, FormArray, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { SucursalService } from '../../../../services/sucursal.service';
import { Sucursal, CreateSucursalDTO } from '../../../../models/sucursal.model';
import { NotificacionService } from '../../../../services/notificacion';

@Component({
  selector: 'app-sucursal-form',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule
  ],
  templateUrl: './sucursal-form.html',
  styleUrl: './sucursal-form.scss'
})
export class SucursalFormComponent implements OnInit {
  @Input() sucursal: Sucursal | null = null;
  @Output() guardado = new EventEmitter<void>();
  @Output() cancelado = new EventEmitter<void>();

  form: FormGroup;
  guardando = false;

  constructor(
    private fb: FormBuilder,
    private sucursalService: SucursalService,
    private notificacion: NotificacionService
  ) {
    this.form = this.fb.group({
      nombre: ['', [Validators.required, Validators.maxLength(100)]],
      telefonos: this.fb.array([])
    });
  }

  ngOnInit(): void {
  if (this.sucursal) {
    this.form.patchValue({ nombre: this.sucursal.nombre });
    
    // ⚠️ IMPORTANTE: Limpiar el FormArray antes de agregar
    this.telefonos.clear();
    
    // Cargar los teléfonos existentes
    if (this.sucursal.telefonos && this.sucursal.telefonos.length > 0) {
      this.sucursal.telefonos.forEach(tel => {
        this.telefonos.push(this.fb.control(tel, [Validators.maxLength(20)]));
      });
    } else {
      // Si no tiene teléfonos, agregar uno vacío
      this.agregarTelefono();
    }
  } else {
    // Al crear una sucursal nueva, agregar un campo de teléfono vacío
    this.agregarTelefono();
  }
}

  // Getter para el FormArray de teléfonos
  get telefonos(): FormArray {
    return this.form.get('telefonos') as FormArray;
  }

  get esEdicion(): boolean {
    return !!this.sucursal;
  }

  agregarTelefono(): void {
    this.telefonos.push(this.fb.control('', [Validators.maxLength(20)]));
  }

  eliminarTelefono(index: number): void {
    if (this.telefonos.length > 1) {
      this.telefonos.removeAt(index);
    } else {
      // Si es el último, solo limpiar el valor
      this.telefonos.at(0).setValue('');
    }
  }

  onSubmit(): void {
    if (this.form.invalid) return;

    this.guardando = true;
    const datos = this.form.value;

    // Filtrar teléfonos vacíos
    const telefonosFiltrados = datos.telefonos
      ? datos.telefonos.filter((t: string) => t && t.trim() !== '')
      : [];

    if (this.esEdicion) {
      const dto = {
        id: this.sucursal!.id,
        nombre: datos.nombre,
        telefonos: telefonosFiltrados
      };
      this.sucursalService.actualizar(dto).subscribe({
        next: () => {
          this.guardando = false;
          this.notificacion.exito('Sucursal actualizada correctamente');
          this.guardado.emit();
        },
        error: (err: any) => {
          this.guardando = false;
          this.notificacion.error(`Error: ${err.status} ${err.statusText}`);
        }
      });
    } else {
      const dto: CreateSucursalDTO = {
        nombre: datos.nombre,
        telefonos: telefonosFiltrados
      };
      this.sucursalService.crear(dto).subscribe({
        next: () => {
          this.guardando = false;
          this.notificacion.exito('Sucursal creada correctamente');
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