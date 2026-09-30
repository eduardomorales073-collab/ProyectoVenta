export interface TelefonoInfo {
  id: number;
  telefono: string;
  fecha: string;
}

export interface Sucursal {
  id: number;
  nombre: string;
  telefonos: TelefonoInfo[];
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