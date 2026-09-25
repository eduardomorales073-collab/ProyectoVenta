export interface Rubro {
  id: number;
  nombre: string;
  descripcion: string;
}

export interface CreateRubroDTO {
  nombre: string;
  descripcion: string;
}

export interface UpdateRubroDTO {
  id: number;
  nombre: string;
  descripcion: string;
}
