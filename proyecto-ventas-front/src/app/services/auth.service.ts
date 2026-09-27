import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap } from 'rxjs';
import { environment } from '../../environments/environment';
import { LoginRequest, AuthResponse, Rol } from '../models/auth.model';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly url = `${environment.apiUrl}/AuthControlador`;

  constructor(private http: HttpClient) { }

  login(data: LoginRequest): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.url}/login`, data).pipe(
      tap(res => {
        localStorage.setItem('token', res.token);
        localStorage.setItem('usuario', JSON.stringify({
          nombre: res.nombre,
          email: res.email,
          rol: res.rol,
          idRol: res.idRol
        }));
      })
    );
  }

  logout(): void {
    localStorage.removeItem('token');
    localStorage.removeItem('usuario');
  }

  getToken(): string | null {
    return localStorage.getItem('token');
  }

  isLoggedIn(): boolean {
    return !!this.getToken();
  }

  // ====== USUARIO Y ROL ======

  getUsuario(): { nombre: string; email: string; rol: string; idRol: number } | null {
    const raw = localStorage.getItem('usuario');
    return raw ? JSON.parse(raw) : null;
  }

  getRol(): Rol | null {
    const usuario = this.getUsuario();
    if (!usuario) return null;

    switch (usuario.idRol) {
      case 1: return 'Administrador';
      case 2: return 'GestorCompras';
      case 3: return 'AdministradorProveedor';
      case 4: return 'Auditor';
      default: return null;
    }
  }

  /** Nombre "bonito" del rol para mostrar en la UI */
  getNombreRolBonito(): string {
    switch (this.getRol()) {
      case 'Administrador': return 'Administrador del Sistema';
      case 'GestorCompras': return 'Gestor de Compras';
      case 'AdministradorProveedor': return 'Administrador de Proveedor';
      case 'Auditor': return 'Auditor / Reportes';
      default: return '';
    }
  }

  esAdmin(): boolean {
    return this.getRol() === 'Administrador';
  }

  esGestorCompras(): boolean {
    return this.getRol() === 'GestorCompras';
  }

  esAdminProveedor(): boolean {
    return this.getRol() === 'AdministradorProveedor';
  }

  esAuditor(): boolean {
    return this.getRol() === 'Auditor';
  }

  /** Alias para no romper código legacy */
  esEmpleado(): boolean {
    return this.esGestorCompras();
  }

  /** Alias para no romper código legacy */
  esProveedor(): boolean {
    return this.esAdminProveedor();
  }

  // ====== RUTA DE INICIO SEGÚN ROL ======

  getRutaInicio(): string {
    switch (this.getRol()) {
      case 'Administrador': return '/admin/dashboard';
      case 'GestorCompras': return '/empleado/pedidos';
      case 'AdministradorProveedor': return '/proveedor/ofertas';
      case 'Auditor': return '/auditor/reportes';
      default: return '/login';
    }
  }

  /** Ruta del HUB de reportes según el rol (Admin y Auditor) */
  getRutaHubReportes(): string {
    switch (this.getRol()) {
      case 'Auditor': return '/auditor/reportes';
      case 'Administrador':
      default: return '/admin/reportes';
    }
  }

  // ====== PERMISOS POR ROL ======

  puedeCrear(): boolean {
    return this.esAdmin() || this.esGestorCompras();
  }

  puedeEditar(): boolean {
    return this.esAdmin() || this.esGestorCompras();
  }

  puedeEliminar(): boolean {
    return this.esAdmin();
  }

  puedeGestionarUsuarios(): boolean {
    return this.esAdmin();
  }

  /** Solo Admin y Auditor pueden ver reportes */
  puedeVerReportes(): boolean {
    return this.esAdmin() || this.esAuditor();
  }

  /** Solo Admin puede gestionar catálogos */
  puedeGestionarCatalogos(): boolean {
    return this.esAdmin();
  }

  /** Solo Admin y Gestor de Compras pueden gestionar pedidos/adjudicaciones */
  puedeGestionarCompras(): boolean {
    return this.esAdmin() || this.esGestorCompras();
  }
}