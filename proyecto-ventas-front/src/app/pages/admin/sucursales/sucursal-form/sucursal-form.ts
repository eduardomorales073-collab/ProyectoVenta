import { Component, EventEmitter, Input, Output, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, FormArray, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatTooltipModule } from '@angular/material/tooltip';
import { SucursalService } from '../../../../services/sucursal.service';
import { Sucursal, CreateSucursalDTO, TelefonoInfo } from '../../../../models/sucursal.model';
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
    MatIconModule,
    MatTooltipModule
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
  
  // Mapa de fechas por índice (para mostrar en el formulario de edición)
  fechasTelefonos: { [index: number]: string } = {};

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
      this.telefonos.clear();

      if (this.sucursal.telefonos && this.sucursal.telefonos.length > 0) {
        this.sucursal.telefonos.forEach((tel, index) => {
          this.telefonos.push(this.fb.control(tel.telefono, [Validators.maxLength(20)]));
          // Guardar la fecha
          if (tel.fecha) {
            this.fechasTelefonos[index] = tel.fecha;
          }
        });
      } else {
        this.agregarTelefono();
      }
    } else {
      this.agregarTelefono();
    }
  }

  get telefonos(): FormArray {
    return this.form.get('telefonos') as FormArray;
  }

  get esEdicion(): boolean {
    return !!this.sucursal;
  }

  // ===== HELPERS PARA LAS FECHAS =====
  tieneFecha(index: number): boolean {
    return !!this.fechasTelefonos[index];
  }

  getFecha(index: number): string {
    return this.fechasTelefonos[index] || '';
  }

  agregarTelefono(): void {
    this.telefonos.push(this.fb.control('', [Validators.maxLength(20)]));
  }

  eliminarTelefono(index: number): void {
    if (this.telefonos.length > 1) {
      this.telefonos.removeAt(index);
      // Reorganizar las fechas
      delete this.fechasTelefonos[index];
    } else {
      this.telefonos.at(0).setValue('');
      delete this.fechasTelefonos[0];
    }
  }

  onSubmit(): void {
    if (this.form.invalid) return;

    this.guardando = true;
    const datos = this.form.value;

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