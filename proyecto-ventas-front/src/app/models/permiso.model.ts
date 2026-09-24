export interface PermisoUsuario {
  idUsuario: number;
  nombreUsuario: string;
  idRol: number;
  idPermiso: number;
  crear: boolean;
  leer: boolean;
  actualizar: boolean;
  borrar: boolean;
}

export interface UpdatePermisosDTO {
  id: number;          // ← AÑADIR
  crear: boolean;
  leer: boolean;
  actualizar: boolean;
  borrar: boolean;
  fecha: string;       // ← AÑADIR
}
