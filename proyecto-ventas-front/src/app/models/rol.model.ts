export interface Rol {
  id: number;
  nombre: string;
  descripcion: string;
  externo: boolean;
}

export interface CreateRolDTO {
  nombre: string;
  descripcion: string;
  externo: boolean;
}

export interface UpdateRolDTO {
  id: number;
  nombre: string;
  descripcion: string;
  externo: boolean;
}
