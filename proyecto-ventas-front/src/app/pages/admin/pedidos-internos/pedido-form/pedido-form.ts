import { Component, EventEmitter, Input, Output, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { PedidoInternoService } from '../../../../services/pedido-interno.service';
import { PedidoInterno, CreatePedidoInternoDTO } from '../../../../models/pedido-interno.model';
import { Departamento } from '../../../../models/departamento.model';
import { OrdenCompra } from '../../../../services/orden-compra.service';
import { NotificacionService } from '../../../../services/notificacion';

@Component({
  selector: 'app-pedido-form',
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
  templateUrl: './pedido-form.html',
  styleUrl: './pedido-form.scss'
})
export class PedidoFormComponent implements OnInit {
  @Input() pedido: PedidoInterno | null = null;
  @Input() departamentos: Departamento[] = [];
  @Input() ordenes: OrdenCompra[] = [];
  @Output() guardado = new EventEmitter<void>();
  @Output() cancelado = new EventEmitter<void>();

  form: FormGroup;
  guardando = false;

  constructor(
    private fb: FormBuilder,
    private pedidoService: PedidoInternoService,
    private notificacion: NotificacionService
  ) {
    this.form = this.fb.group({
      id_Departamento: ['', [Validators.required]],
      id_OrdenCompra: [null],
      fecha_Solicitada: ['', [Validators.required]],
      fecha_Ingreso: ['', [Validators.required]]
    });
  }

  ngOnInit(): void {
    if (this.pedido) {
      this.form.patchValue({
        id_Departamento: this.pedido.id_Departamento,
        id_OrdenCompra: this.pedido.id_OrdenCompra,
        fecha_Solicitada: this.pedido.fecha_Solicitada?.substring(0, 10),
        fecha_Ingreso: this.pedido.fecha_Ingreso?.substring(0, 10)
      });
    }
  }

  get esEdicion(): boolean {
    return !!this.pedido;
  }

  onSubmit(): void {
    if (this.form.invalid) return;
    this.guardando = true;
    const datos = this.form.value;

    if (this.esEdicion) {
      const dto = {
        id: this.pedido!.id,
        id_Departamento: datos.id_Departamento,
        id_OrdenCompra: datos.id_OrdenCompra || null,
        fecha_Solicitada: datos.fecha_Solicitada,
        fecha_Ingreso: datos.fecha_Ingreso
      };
      this.pedidoService.actualizar(dto).subscribe({
        next: () => {
          this.guardando = false;
          this.notificacion.exito('Pedido actualizado correctamente');
          this.guardado.emit();
        },
        error: (err: any) => {
          this.guardando = false;
          this.notificacion.error(err.error?.mensaje || `Error: ${err.status} ${err.statusText}`);
        }
      });
    } else {
      const dto: CreatePedidoInternoDTO = {
        id_Departamento: datos.id_Departamento,
        id_OrdenCompra: datos.id_OrdenCompra || null,
        fecha_Solicitada: datos.fecha_Solicitada,
        fecha_Ingreso: datos.fecha_Ingreso
      };
      this.pedidoService.crear(dto).subscribe({
        next: () => {
          this.guardando = false;
          this.notificacion.exito('Pedido creado correctamente');
          this.guardado.emit();
        },
        error: (err: any) => {
          this.guardando = false;
          this.notificacion.error(err.error?.mensaje || `Error: ${err.status} ${err.statusText}`);
        }
      });
    }
  }

  onCancelar(): void {
    this.cancelado.emit();
  }
}
