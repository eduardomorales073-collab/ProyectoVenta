export interface Adjudicacion {
  id: number;
  fecha_Resolucion: string;
  orden_Compra: number;
  estado: string;
}

export interface CreateAdjudicacionDTO {
  fecha_Resolucion: string;
  orden_Compra: number;
  estado: string;
}

export interface UpdateAdjudicacionDTO {
  id: number;
  fecha_Resolucion: string;
  orden_Compra: number;
  estado: string;
}
