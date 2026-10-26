import { Component, EventEmitter, Input, Output, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { RelacionService } from '../../../../services/relacion.service';
import { Proveedor } from '../../../../models/proveedor.model';
import { Relacion } from '../../../../models/relacion.model';
import { NotificacionService } from '../../../../services/notificacion';
import { MatChipsModule } from '@angular/material/chips';

@Component({
  selector: 'app-relacion-modal',
  standalone: true,
  imports: [
    CommonModule, ReactiveFormsModule, MatFormFieldModule,
    MatInputModule, MatSelectModule, MatButtonModule, MatIconModule, MatChipsModule 
  ],
  templateUrl: './relacion-modal.html',
  styleUrl: './relacion-modal.scss'
})
export class RelacionModalComponent implements OnInit {
  @Input() proveedores: Proveedor[] = [];
  @Input() proveedorId: number | null = null;
  @Output() cerrado = new EventEmitter<void>();

  form: FormGroup;
  relaciones: Relacion[] = [];
  guardando = false;

  constructor(
    private fb: FormBuilder,
    private relacionService: RelacionService,
    private notification: NotificacionService
  ) {
    this.form = this.fb.group({
      proveedor2: ['', Validators.required],
      tipoRelacion: ['Socio Comercial', Validators.required]
    });
  }

  ngOnInit(): void {
    this.cargarRelaciones();
  }

  cargarRelaciones(): void {
    this.relacionService.listar().subscribe({
      next: (data) => {
        // Filtrar solo las relaciones del proveedor actual
        this.relaciones = data.filter(r => r.proveedor1 === this.proveedorId || r.proveedor2 === this.proveedorId);
      }
    });
  }

  agregarRelacion(): void {
    if (this.form.invalid || !this.proveedorId) return;
    this.guardando = true;
    const datos = this.form.value;

    this.relacionService.crear({
      proveedor1: this.proveedorId,
      proveedor2: datos.proveedor2,
      tipoRelacion: datos.tipoRelacion
    }).subscribe({
      next: () => {
        this.guardando = false;
        this.notification.exito('Relación agregada correctamente');
        this.form.reset({ tipoRelacion: 'Socio Comercial' });
        this.cargarRelaciones();
      },
      error: (err) => {
        this.guardando = false;
        this.notification.error('Error al agregar relación');
      }
    });
  }

  eliminarRelacion(id: number): void {
    this.relacionService.eliminar(id).subscribe({
      next: () => {
        this.notification.exito('Relación eliminada');
        this.cargarRelaciones();
      }
    });
  }

  cerrar(): void {
    this.cerrado.emit();
  }
}
