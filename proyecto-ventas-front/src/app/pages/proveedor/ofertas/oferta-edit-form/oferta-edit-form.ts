import { Component, EventEmitter, Input, Output, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { OfertaProveedorService } from '../../../../services/oferta-proveedor.service';
import { OfertaProveedor } from '../../../../models/oferta-proveedor.model';
import { NotificacionService } from '../../../../services/notificacion';

@Component({
  selector: 'app-oferta-edit-form',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule
  ],
  templateUrl: './oferta-edit-form.html',
  styleUrl: './oferta-edit-form.scss'
})
export class OfertaEditFormComponent implements OnInit {
  @Input() oferta: OfertaProveedor | null = null;
  @Output() guardado = new EventEmitter<void>();
  @Output() cancelado = new EventEmitter<void>();

  form: FormGroup;
  guardando = false;

  constructor(
    private fb: FormBuilder,
    private ofertaService: OfertaProveedorService,
    private notificacion: NotificacionService
  ) {
    this.form = this.fb.group({
      precio: ['', [Validators.required, Validators.min(0.01)]]
    });
  }

  ngOnInit(): void {
    if (this.oferta) {
      this.form.patchValue({
        precio: this.oferta.precio
      });
    }
  }

  onSubmit(): void {
    if (this.form.invalid || !this.oferta) return;

    this.guardando = true;

    const dto = {
      id: this.oferta.id,
      id_Proveedor: this.oferta.id_Proveedor,
      id_Pedido_Interno: this.oferta.id_Pedido_Interno,
      precio: parseFloat(this.form.value.precio),
      fecha_Oferta: this.oferta.fecha_Oferta
    };

    this.ofertaService.actualizar(dto).subscribe({
      next: () => {
        this.guardando = false;
        this.notificacion.exito('Oferta actualizada correctamente');
        this.guardado.emit();
      },
      error: (err: any) => {
        this.guardando = false;
        this.notificacion.error(err.error?.mensaje || `Error: ${err.status} ${err.statusText}`);
      }
    });
  }

  onCancelar(): void {
    this.cancelado.emit();
  }
}