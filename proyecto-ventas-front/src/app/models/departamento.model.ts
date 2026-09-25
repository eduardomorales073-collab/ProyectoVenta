export interface Departamento {
  id: number;
  nombre: string;
  descripcion: string;
  id_Sucursal: number;
}

export interface CreateDepartamentoDTO {
  nombre: string;
  descripcion: string;
  id_Sucursal: number;
}

export interface UpdateDepartamentoDTO {
  id: number;
  nombre: string;
  descripcion: string;
  id_Sucursal: number;
}
