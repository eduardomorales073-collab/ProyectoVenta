export interface UnidadMedida {
  id: number;
  nombre: string;
  abreviatura: string;
  descripcion: string;
}

export interface CreateUnidadMedidaDTO {
  nombre: string;
  abreviatura: string;
  descripcion: string;
}

export interface UpdateUnidadMedidaDTO {
  id: number;
  nombre: string;
  abreviatura: string;
  descripcion: string;
}