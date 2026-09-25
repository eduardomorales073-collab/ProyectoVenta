import { Component, EventEmitter, Input, Output, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatDividerModule } from '@angular/material/divider';
import { TipoOrdenService } from '../../../../services/tipo-orden.service';
import { TipoOrden, CreateTipoOrdenDTO } from '../../../../models/tipo-orden.model';
import { NotificacionService } from '../../../../services/notificacion';
import { MatChipsModule } from '@angular/material/chips'; 

@Component({
  selector: 'app-tipo-orden-form',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatButtonModule,
    MatIconModule,
    MatCheckboxModule,
    MatDividerModule,
    MatChipsModule 
  ],
  templateUrl: './tipo-orden-form.html',
  styleUrl: './tipo-orden-form.scss'
})
export class TipoOrdenFormComponent implements OnInit {
  @Input() tipoOrden: TipoOrden | null = null;
  @Output() guardado = new EventEmitter<void>();
  @Output() cancelado = new EventEmitter<void>();

  form: FormGroup;
  guardando = false;

  constructor(
    private fb: FormBuilder,
    private tipoOrdenService: TipoOrdenService,
    private notificacion: NotificacionService
  ) {
    this.form = this.fb.group({
      grande: [false],
      urgente: [false]
    });
  }

  ngOnInit(): void {
    if (this.tipoOrden) {
      this.form.patchValue({
        grande: this.tipoOrden.grande,
        urgente: this.tipoOrden.urgente
      });
    }
  }

  get esEdicion(): boolean {
    return !!this.tipoOrden;
  }

  onSubmit(): void {
    this.guardando = true;
    const datos = this.form.value;

    if (this.esEdicion) {
      const dto = {
        id: this.tipoOrden!.id,
        grande: datos.grande,
        urgente: datos.urgente
      };
      this.tipoOrdenService.actualizar(dto).subscribe({
        next: () => {
          this.guardando = false;
          this.notificacion.exito('Tipo de orden actualizado correctamente');
          this.guardado.emit();
        },
        error: (err: any) => {
          this.guardando = false;
          this.notificacion.error(`Error: ${err.status} ${err.statusText}`);
        }
      });
    } else {
      const dto: CreateTipoOrdenDTO = {
        grande: datos.grande,
        urgente: datos.urgente
      };
      this.tipoOrdenService.crear(dto).subscribe({
        next: () => {
          this.guardando = false;
          this.notificacion.exito('Tipo de orden creado correctamente');
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
