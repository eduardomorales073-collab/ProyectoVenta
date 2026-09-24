export interface LoginRequest {
  email: string;
  password: string;
}

export interface AuthResponse {
  token: string;
  nombre: string;
  email: string;
  rol: string;      // ← NUEVO
  idRol: number;    // ← NUEVO
}

export type Rol = 'Administrador' | 'Empleado' | 'Proveedor';   // ← NUEVO
