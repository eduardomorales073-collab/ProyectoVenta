export interface Sucursal {
  id: number;
  nombre: string;
  telefonos: string[];
}

export interface CreateSucursalDTO {
  nombre: string;
  telefonos: string[];
}

export interface UpdateSucursalDTO {
  id: number;
  nombre: string;
  telefonos: string[];
}