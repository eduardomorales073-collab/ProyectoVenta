export interface Proveedor {
  id: number;
  nombre: string;
  descripcion: string;
  telefono: string;
  direccion: string;
  nit: string | null;
  categoria: string | null;
  id_Rubros: number[];
}

export interface CreateProveedorDTO {
  nombre: string;
  descripcion: string;
  telefono: string;
  direccion: string;
  nit: string | null;
  categoria: string | null;
  id_Rubros: number[];
}

export interface UpdateProveedorDTO {
  id: number;
  nombre: string;
  descripcion: string;
  telefono: string;
  direccion: string;
  nit: string | null;
  categoria: string | null;
  id_Rubros: number[];
}