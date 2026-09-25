export interface Proveedor {
  id: number;
  nombre: string;
  descripcion: string;
  telefono: string;
  direccion: string;
  id_Rubros: number[];
}

export interface CreateProveedorDTO {
  nombre: string;
  descripcion: string;
  telefono: string;
  direccion: string;
  id_Rubros: number[];
}

export interface UpdateProveedorDTO {
  id: number;
  nombre: string;
  descripcion: string;
  telefono: string;
  direccion: string;
  id_Rubros: number[];
}
