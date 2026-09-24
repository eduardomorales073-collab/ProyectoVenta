export interface Usuario {
  id: number;
  nombre: string;
  email: string;
  activo: boolean;
  id_Rol: number;
}

export interface CreateUsuarioDTO {
  nombre: string;
  email: string;
  contrasena: string;
  activo: boolean;
  id_Rol: number;
}

export interface UpdateUsuarioDTO {
  id: number;
  nombre: string;
  email: string;
  contrasena?: string;   // opcional: solo si se cambia
  activo: boolean;
  id_Rol: number;
}
