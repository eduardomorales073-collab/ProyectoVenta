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

        // Decodificar el JWT para extraer el IdProveedor (si existe)
        let idProveedor: number | null = null;
        try {
          const payload = JSON.parse(atob(res.token.split('.')[1]));
          const idProv = payload['IdProveedor'] || payload['idProveedor'];
          idProveedor = idProv ? parseInt(idProv, 10) : null;
        } catch (error) {
          console.error('Error al decodificar el token:', error);
        }

        localStorage.setItem('usuario', JSON.stringify({
          nombre: res.nombre,
          email: res.email,
          rol: res.rol,
          idRol: res.idRol,
          id_Proveedor: idProveedor
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

  getUsuario(): { nombre: string; email: string; rol: string; idRol: number; id_Proveedor: number | null } | null {
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
      case 5: return 'CreadorPedidos';
      default: return null;
    }
  }

  getNombreRolBonito(): string {
    switch (this.getRol()) {
      case 'Administrador': return 'Administrador del Sistema';
      case 'GestorCompras': return 'Gestor de Compras';
      case 'AdministradorProveedor': return 'Administrador de Proveedor';
      case 'Auditor': return 'Auditor / Reportes';
      case 'CreadorPedidos': return 'Creador de Pedidos';
      default: return '';
    }
  }

  // ====== VERIFICADORES DE ROL ======
  esAdmin(): boolean { return this.getRol() === 'Administrador'; }
  esGestorCompras(): boolean { return this.getRol() === 'GestorCompras'; }
  esAdminProveedor(): boolean { return this.getRol() === 'AdministradorProveedor'; }
  esAuditor(): boolean { return this.getRol() === 'Auditor'; }
  esCreadorPedidos(): boolean { return this.getRol() === 'CreadorPedidos'; }
  esEmpleado(): boolean { return this.esGestorCompras(); }
  esProveedor(): boolean { return this.esAdminProveedor(); }

  // ====== RUTAS ======

  getRutaInicio(): string {
    switch (this.getRol()) {
      case 'Administrador': return '/admin/dashboard';
      case 'GestorCompras': return '/empleado/pedidos';
      case 'AdministradorProveedor': return '/proveedor/ofertas';
      case 'Auditor': return '/auditor/reportes';
      case 'CreadorPedidos': return '/creador/pedidos';
      default: return '/login';
    }
  }

  getRutaHubReportes(): string {
    switch (this.getRol()) {
      case 'Auditor': return '/auditor/reportes';
      case 'Administrador':
      default: return '/admin/reportes';
    }
  }

  // ====== PERMISOS ESPECÍFICOS POR MÓDULO ======

  puedeGestionarArticulos(): boolean { return this.esAdmin(); }
  puedeGestionarUsuarios(): boolean { return this.esAdmin(); }
  puedeGestionarProveedores(): boolean { return this.esAdmin(); }
  puedeGestionarCategorias(): boolean { return this.esAdmin(); }
  puedeGestionarUnidades(): boolean { return this.esAdmin(); }
  puedeGestionarCatalogos(): boolean { return this.esAdmin(); }
  puedeGestionarRoles(): boolean { return this.esAdmin(); }

  // --- PEDIDOS ---
  puedeCrearPedidos(): boolean {
    return this.esAdmin() || this.esGestorCompras() || this.esCreadorPedidos();
  }

  puedeEditarPedidos(): boolean {
    return this.esAdmin() || this.esGestorCompras();
  }

  puedeEliminarPedidos(): boolean { return this.esAdmin(); }

  // --- ADJUDICACIONES ---
  puedeCrearAdjudicaciones(): boolean { return this.esAdmin() || this.esGestorCompras(); }
  puedeEditarAdjudicaciones(): boolean { return this.esAdmin() || this.esGestorCompras(); }
  puedeEliminarAdjudicaciones(): boolean { return this.esAdmin(); }

  // --- ÓRDENES Y OFERTAS ---
  puedeGestionarOrdenes(): boolean { return this.esAdmin() || this.esGestorCompras(); }
  puedeGestionarOfertas(): boolean { return this.esAdminProveedor() || this.esAdmin(); }

  // --- REPORTES ---
  puedeVerReportes(): boolean { return this.esAdmin() || this.esAuditor() || this.esGestorCompras(); }

  // ====== ALIAS GENÉRICOS (para compatibilidad) ======
  puedeCrear(): boolean { return this.esAdmin() || this.esGestorCompras(); }
  puedeEditar(): boolean { return this.esAdmin() || this.esGestorCompras(); }
  puedeEliminar(): boolean { return this.esAdmin(); }
  puedeGestionarCompras(): boolean { return this.esAdmin() || this.esGestorCompras(); }

  /**
   * Obtener el id_Proveedor del usuario logueado.
   * Primero intenta con localStorage, luego con el JWT (fallback robusto).
   */
  getIdProveedor(): number | null {
    // 1. Intentar con localStorage
    const usuario = this.getUsuario();
    if (usuario && usuario.id_Proveedor) {
      return usuario.id_Proveedor;
    }

    // 2. Fallback: decodificar el JWT
    const token = this.getToken();
    if (!token) return null;

    try {
      const payload = JSON.parse(atob(token.split('.')[1]));
      const idProv = payload['IdProveedor'] || payload['idProveedor'];
      return idProv ? parseInt(idProv, 10) : null;
    } catch (error) {
      console.error('Error al decodificar el token:', error);
      return null;
    }
  }
}