export interface Perfil {
  id: number;
  nombre: string;
  email: string;
  activo: boolean;
  id_Rol: number;
}

export interface UpdatePerfilDTO {
  nombre: string;
  email: string;
}

export interface ChangePasswordDTO {
  contrasenaActual: string;
  contrasenaNueva: string;
  confirmarContrasena: string;
}
