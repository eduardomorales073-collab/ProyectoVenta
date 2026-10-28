import { Component, EventEmitter, Input, Output, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatCardModule } from '@angular/material/card';
import { Subscription } from 'rxjs';

import { Articulo, ArticuloService, CreateArticuloDTO, UpdateArticuloDTO } from '../../services/articulo.service';
import { RubroService } from '../../services/rubro.service';
import { Rubro } from '../../models/rubro.model';
import { UnidadMedidaService } from '../../services/unidad-medida.service';
import { UnidadMedida } from '../../models/unidad-medida.model';
import { NotificacionService } from '../../services/notificacion';
import { AuthService } from '../../services/auth.service';
import { ProveedorService } from '../../services/proveedor.service';

@Component({
  selector: 'app-articulo-form',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatButtonModule,
    MatIconModule,
    MatCardModule
  ],
  templateUrl: './articulo-form.html',
  styleUrl: './articulo-form.scss'
})
export class ArticuloFormComponent implements OnInit, OnDestroy {
  @Input() articulo: Articulo | null = null;
  @Output() guardado = new EventEmitter<void>();
  @Output() cancelado = new EventEmitter<void>();

  form: FormGroup;
  guardando = false;
  esEdicion = false;
  esProveedor = false;

  rubros: Rubro[] = [];
  unidadesMedida: UnidadMedida[] = [];
  private rubroSub?: Subscription;

  constructor(
    private fb: FormBuilder,
    private articuloService: ArticuloService,
    private rubroService: RubroService,
    private unidadMedidaService: UnidadMedidaService,
    private notification: NotificacionService,
    private authService: AuthService,
    private proveedorService: ProveedorService
  ) {
    this.form = this.fb.group({
      codigo: ['', [Validators.required]],
      nombre: ['', [Validators.required]],
      descripcion: ['', [Validators.required]],
      unidad_medida: ['', [Validators.required]],
      id_Rubro: [null]
    });
  }

  ngOnInit(): void {
    this.esEdicion = !!this.articulo;
    this.esProveedor = this.authService.esProveedor();

    // Cargar Rubros
    this.rubroService.listar().subscribe({
      next: (data: any) => {
        const todosLosRubros = Array.isArray(data) ? data : (data.$values || []);

        // ✅ Si es proveedor y está creando, filtrar SOLO su rubro
        if (!this.esEdicion && this.esProveedor) {
          const idProveedor = this.authService.getIdProveedor();
          if (idProveedor) {
            this.proveedorService.obtener(idProveedor).subscribe({
              next: (proveedor: any) => {
                // Tomamos el primer rubro asignado al proveedor
                const idRubro = proveedor.id_Rubros?.[0] || null;
                if (idRubro) {
                  // ✅ FILTRAMOS: Solo mostramos el rubro que le corresponde
                  this.rubros = todosLosRubros.filter((r: any) => r.id === idRubro);
                  this.form.patchValue({ id_Rubro: idRubro });
                  this.seleccionarRubro(idRubro);
                } else {
                  this.rubros = []; // Si no tiene rubro, no mostramos nada
                }
              },
              error: (err) => {
                console.error('Error al obtener rubro del proveedor', err);
                this.rubros = []; // Fallback
              }
            });
          } else {
            this.rubros = todosLosRubros; // Fallback si no hay ID de proveedor
          }
        } else {
          // Si es Admin o Gestor, mostramos TODOS los rubros
          this.rubros = todosLosRubros;
        }
      }
    });

    // Cargar Unidades de Medida
    this.unidadMedidaService.listar().subscribe({
      next: (data: any) => {
        this.unidadesMedida = Array.isArray(data) ? data : (data.$values || []);
      },
      error: () => {
        this.unidadesMedida = [{ id: 1, nombre: 'Unidad' } as any];
      }
    });

    if (this.articulo) {
      this.form.patchValue({
        codigo: this.articulo.codigo || '',
        nombre: this.articulo.nombre,
        descripcion: this.articulo.descripcion,
        unidad_medida: this.articulo.unidad_medida || '',
        id_Rubro: (this.articulo as any).id_Rubro
      });
    } else {
      this.generarCodigo();
    }

    // Al cambiar el rubro, actualizar el código automáticamente
    this.rubroSub = this.form.get('id_Rubro')?.valueChanges.subscribe(idRubro => {
      const codigoActual = this.form.get('codigo')?.value || '';
      if (!codigoActual || codigoActual.startsWith('ART-') || this.esPrefijoDeRubro(codigoActual)) {
        this.actualizarPrefijoCodigo(idRubro, codigoActual);
      }
    });
  }

  ngOnDestroy(): void {
    if (this.rubroSub) {
      this.rubroSub.unsubscribe();
    }
  }

  private esPrefijoDeRubro(codigo: string): boolean {
    return ['TEC-', 'ALI-', 'MOB-', 'PAP-', 'OTR-'].some(p => codigo.startsWith(p));
  }

  private actualizarPrefijoCodigo(idRubro: number | null, codigoActual: string): void {
    let prefijo = 'ART-';
    switch (idRubro) {
      case 1: prefijo = 'TEC-'; break;
      case 2: prefijo = 'ALI-'; break;
      case 3: prefijo = 'MOB-'; break;
      case 4: prefijo = 'PAP-'; break;
      case 5: prefijo = 'OTR-'; break;
    }

    let numero = '001';
    if (codigoActual && codigoActual.includes('-')) {
      numero = codigoActual.split('-')[1];
      if (!numero || isNaN(Number(numero))) {
        numero = '001';
      }
    } else if (!this.esEdicion) {
      numero = String(Date.now()).slice(-3);
    }

    this.form.patchValue({ codigo: `${prefijo}${numero}` }, { emitEvent: false });
  }

  generarCodigo(): void {
    this.articuloService.listar().subscribe({
      next: (articulos) => {
        const maxId = articulos.length > 0 ? Math.max(...articulos.map(a => a.id)) : 0;
        const nuevoId = maxId + 1;
        const idRubro = this.form.get('id_Rubro')?.value;

        let prefijo = 'ART-';
        switch (idRubro) {
          case 1: prefijo = 'TEC-'; break;
          case 2: prefijo = 'ALI-'; break;
          case 3: prefijo = 'MOB-'; break;
          case 4: prefijo = 'PAP-'; break;
          case 5: prefijo = 'OTR-'; break;
        }

        this.form.patchValue({ codigo: `${prefijo}${String(nuevoId).padStart(3, '0')}` });
      },
      error: () => this.form.patchValue({ codigo: 'ART-' + Date.now().toString().slice(-4) })
    });
  }

  onSubmit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.guardando = true;
    const datos = this.form.getRawValue();

    if (this.articulo) {
      const dto: UpdateArticuloDTO = {
        id: this.articulo.id,
        nombre: datos.nombre,
        descripcion: datos.descripcion,
        codigo: datos.codigo,
        unidad_medida: datos.unidad_medida,
        id_Rubro: datos.id_Rubro
      };

      this.articuloService.actualizar(dto).subscribe({
        next: () => this.finalizar(),
        error: (err) => this.error(err)
      });
    } else {
      const dto: CreateArticuloDTO = {
        nombre: datos.nombre,
        descripcion: datos.descripcion,
        codigo: datos.codigo,
        unidad_medida: datos.unidad_medida,
        id_Rubro: datos.id_Rubro
      };

      this.articuloService.crear(dto).subscribe({
        next: () => this.finalizar(),
        error: (err) => this.error(err)
      });
    }
  }

  private finalizar() {
    this.guardando = false;
    this.notification.exito(this.esEdicion ? 'Artículo actualizado' : 'Artículo creado');
    this.guardado.emit();
  }

  private error(err: any) {
    this.guardando = false;
    this.notification.error(err.error?.mensaje || 'Error al guardar');
  }

  onCancel(): void {
    this.cancelado.emit();
  }

  seleccionarRubro(idRubro: number | null): void {
    this.form.patchValue({ id_Rubro: idRubro });
    const codigoActual = this.form.get('codigo')?.value || '';
    let prefijo = 'ART-';

    switch (idRubro) {
      case 1: prefijo = 'TEC-'; break;
      case 2: prefijo = 'ALI-'; break;
      case 3: prefijo = 'MOB-'; break;
      case 4: prefijo = 'PAP-'; break;
      case 5: prefijo = 'OTR-'; break;
    }

    let numero = '001';
    if (codigoActual && codigoActual.includes('-')) {
      const partes = codigoActual.split('-');
      if (partes[1] && !isNaN(Number(partes[1]))) {
        numero = partes[1];
      }
    }

    this.form.patchValue({ codigo: `${prefijo}${numero}` });
  }

  getIconoRubro(nombre: string): string {
    const n = nombre.toLowerCase();
    if (n.includes('tecnolog')) return 'computer';
    if (n.includes('alimento')) return 'restaurant';
    if (n.includes('mobiliario')) return 'chair';
    if (n.includes('papeler')) return 'description';
    if (n.includes('otro')) return 'category';
    return 'label';
  }

  getPrefijoRubro(idRubro: number): string {
    switch (idRubro) {
      case 1: return 'TEC-';
      case 2: return 'ALI-';
      case 3: return 'MOB-';
      case 4: return 'PAP-';
      case 5: return 'OTR-';
      default: return '';
    }
  }
}
