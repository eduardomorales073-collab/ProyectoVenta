export interface Sucursal {
  id: number;
  nombre: string;
}

export interface CreateSucursalDTO {
  nombre: string;
}

export interface UpdateSucursalDTO {
  id: number;
  nombre: string;
}
