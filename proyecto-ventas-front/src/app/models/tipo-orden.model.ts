export interface TipoOrden {
  id: number;
  grande: boolean;
  urgente: boolean;
}

export interface CreateTipoOrdenDTO {
  grande: boolean;
  urgente: boolean;
}

export interface UpdateTipoOrdenDTO {
  id: number;
  grande: boolean;
  urgente: boolean;
}
