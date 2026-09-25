import { Component, EventEmitter, Input, Output, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { RubroService } from '../../../../services/rubro.service';
import { Rubro, CreateRubroDTO } from '../../../../models/rubro.model';
import { NotificacionService } from '../../../../services/notificacion';

@Component({
  selector: 'app-rubro-form',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule
  ],
  templateUrl: './rubro-form.html',
  styleUrl: './rubro-form.scss'
})
export class RubroFormComponent implements OnInit {
  @Input() rubro: Rubro | null = null;
  @Output() guardado = new EventEmitter<void>();
  @Output() cancelado = new EventEmitter<void>();

  form: FormGroup;
  guardando = false;

  constructor(
    private fb: FormBuilder,
    private rubroService: RubroService,
    private notificacion: NotificacionService
  ) {
    this.form = this.fb.group({
      nombre: ['', [Validators.required, Validators.maxLength(100)]],
      descripcion: ['', [Validators.required, Validators.maxLength(200)]]
    });
  }

  ngOnInit(): void {
    if (this.rubro) {
      this.form.patchValue({
        nombre: this.rubro.nombre,
        descripcion: this.rubro.descripcion
      });
    }
  }

  get esEdicion(): boolean {
    return !!this.rubro;
  }

  onSubmit(): void {
    if (this.form.invalid) return;

    this.guardando = true;
    const datos = this.form.value;

    if (this.esEdicion) {
      const dto = { id: this.rubro!.id, nombre: datos.nombre, descripcion: datos.descripcion };
      this.rubroService.actualizar(dto).subscribe({
        next: () => {
          this.guardando = false;
          this.notificacion.exito('Rubro actualizado correctamente');
          this.guardado.emit();
        },
        error: (err: any) => {
          this.guardando = false;
          this.notificacion.error(`Error: ${err.status} ${err.statusText}`);
        }
      });
    } else {
      const dto: CreateRubroDTO = { nombre: datos.nombre, descripcion: datos.descripcion };
      this.rubroService.crear(dto).subscribe({
        next: () => {
          this.guardando = false;
          this.notificacion.exito('Rubro creado correctamente');
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
