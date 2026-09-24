import { Component, EventEmitter, Input, Output, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { UsuarioService } from '../../../../services/usuario.service';
import { PermisoUsuario } from '../../../../models/permiso.model';

@Component({
  selector: 'app-permisos-modal',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './permisos-modal.html',
  styleUrl: './permisos-modal.scss'
})
export class PermisosModalComponent implements OnInit {
  @Input() idUsuario!: number;
  @Input() nombreUsuario!: string;
  @Output() cerrar = new EventEmitter<void>();

  permisos: PermisoUsuario | null = null;
  cargando = false;
  error = '';

  constructor(
    private usuarioService: UsuarioService,
    private cdr: ChangeDetectorRef
  ) { }

  ngOnInit(): void {
    this.cargar();
  }

  cargar(): void {
    this.cargando = true;
    this.error = '';
    this.usuarioService.obtenerPermisos(this.idUsuario).subscribe({
      next: (data) => {
        this.permisos = data;
        this.cargando = false;
        this.cdr.detectChanges();
      },
      error: (err: any) => {
        this.error = `Error: ${err.status} ${err.statusText}`;
        this.cargando = false;
        this.cdr.detectChanges();
      }
    });
  }

  onCerrar(): void {
    this.cerrar.emit();
  }
}
