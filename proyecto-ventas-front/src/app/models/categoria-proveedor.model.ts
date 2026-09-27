export interface CategoriaProveedor {
  id: number;
  nombre: string;
  descripcion: string;
}

export interface CreateCategoriaProveedorDTO {
  nombre: string;
  descripcion: string;
}

export interface UpdateCategoriaProveedorDTO {
  id: number;
  nombre: string;
  descripcion: string;
}