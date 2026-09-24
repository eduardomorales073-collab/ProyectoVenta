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
      case 2: return 'Empleado';
      case 3: return 'Proveedor';
      default: return null;
    }
  }

  esAdmin(): boolean {
    return this.getRol() === 'Administrador';
  }

  esEmpleado(): boolean {
    return this.getRol() === 'Empleado';
  }

  esProveedor(): boolean {
    return this.getRol() === 'Proveedor';
  }

  // ====== RUTA DE INICIO SEGÚN ROL ======

  getRutaInicio(): string {
    switch (this.getRol()) {
      case 'Administrador': return '/admin/dashboard';
      case 'Empleado': return '/empleado/pedidos';
      case 'Proveedor': return '/proveedor/ofertas';
      default: return '/login';
    }
  }

  // ====== PERMISOS POR ROL ======

  puedeCrear(): boolean {
    return this.esAdmin() || this.esEmpleado();
  }

  puedeEditar(): boolean {
    return this.esAdmin() || this.esEmpleado();
  }

  puedeEliminar(): boolean {
    return this.esAdmin();
  }

  puedeGestionarUsuarios(): boolean {
    return this.esAdmin();
  }


}
