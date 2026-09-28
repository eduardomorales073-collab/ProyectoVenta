import { Component, EventEmitter, Input, Output, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { OfertaProveedorService } from '../../../../services/oferta-proveedor.service';
import { AuthService } from '../../../../services/auth.service';
import { PedidoInterno } from '../../../../models/pedido-interno.model';
import { NotificacionService } from '../../../../services/notificacion';

@Component({
  selector: 'app-oferta-form',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule
  ],
  templateUrl: './oferta-form.html',
  styleUrl: './oferta-form.scss'
})
export class OfertaFormComponent implements OnInit {
  @Input() pedido: PedidoInterno | null = null;
  @Output() guardado = new EventEmitter<void>();
  @Output() cancelado = new EventEmitter<void>();

  form: FormGroup;
  guardando = false;

  constructor(
    private fb: FormBuilder,
    private ofertaService: OfertaProveedorService,
    private authService: AuthService,
    private notificacion: NotificacionService
  ) {
    this.form = this.fb.group({
      precio: ['', [Validators.required, Validators.min(0.01)]]
    });
  }

  ngOnInit(): void {
    if (!this.pedido) {
      this.cancelado.emit();
    }
  }

  onSubmit(): void {
    if (this.form.invalid || !this.pedido) return;

    const idProveedor = this.authService.getIdProveedor();
    if (!idProveedor) {
      this.notificacion.error('No se pudo identificar tu proveedor.');
      return;
    }

    this.guardando = true;

    const dto = {
      id_Proveedor: idProveedor,
      id_Pedido_Interno: this.pedido.id,
      precio: parseFloat(this.form.value.precio),
      fecha_Oferta: new Date().toISOString()
    };

    this.ofertaService.ofertar(dto).subscribe({
      next: () => {
        this.guardando = false;
        this.notificacion.exito('Oferta registrada correctamente');
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