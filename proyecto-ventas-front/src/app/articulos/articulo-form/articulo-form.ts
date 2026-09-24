import { Component, EventEmitter, Input, Output, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Articulo, ArticuloService, CreateArticuloDTO } from '../../services/articulo.service';
import { NotificacionService } from '../../services/notificacion'; 

@Component({
  selector: 'app-articulo-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './articulo-form.html',
  styleUrl: './articulo-form.scss'
})
export class ArticuloFormComponent implements OnInit {
  @Input() articulo: Articulo | null = null;
  @Output() guardado = new EventEmitter<void>();
  @Output() cancelado = new EventEmitter<void>();

  form: FormGroup;
  guardando = false;
  error = '';

  constructor(
    private fb: FormBuilder,
    private articuloService: ArticuloService,
    private notificacion: NotificacionService  
  ) {
    this.form = this.fb.group({
      nombre: ['', [Validators.required, Validators.maxLength(100)]],
      descripcion: ['', [Validators.required, Validators.maxLength(200)]]
    });
  }

  ngOnInit(): void {
    if (this.articulo) {
      this.form.patchValue({
        nombre: this.articulo.nombre,
        descripcion: this.articulo.descripcion
      });
    }
  }

  get esEdicion(): boolean {
    return !!this.articulo;
  }

  onSubmit(): void {
    if (this.form.invalid) return;

    this.guardando = true;
    this.error = '';

    const datos = this.form.value;

    if (this.esEdicion) {
      const dto = {
        id: this.articulo!.id,
        nombre: datos.nombre,
        descripcion: datos.descripcion
      };
      this.articuloService.actualizar(dto).subscribe({
        next: () => {
          this.guardando = false;
          this.notificacion.exito(this.esEdicion ? 'Artículo actualizado' : 'Artículo creado');   // ← AÑADIR
          this.guardado.emit();
        },
        error: (err) => {
          this.guardando = false;
          this.error = `Error al actualizar: ${err.status} ${err.statusText}`;
        }
      });
    } else {
      const dto: CreateArticuloDTO = {
        nombre: datos.nombre,
        descripcion: datos.descripcion
      };
      this.articuloService.crear(dto).subscribe({
        next: () => {
          this.guardando = false;
          this.guardado.emit();
        },
        error: (err) => {
          this.guardando = false;
          this.error = `Error al crear: ${err.status} ${err.statusText}`;
        }
      });
    }
  }

  onCancelar(): void {
    this.cancelado.emit();
  }
}
