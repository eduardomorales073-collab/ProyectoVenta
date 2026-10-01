export interface Usuario {
  id: number;
  nombre: string;
  email: string;
  activo: boolean;
  id_Rol: number;
  id_Proveedor?: number;       
  id_Departamento?: number;    
}

export interface CreateUsuarioDTO {
  nombre: string;
  email: string;
  contrasena: string;
  activo: boolean;
  id_Rol: number;
  id_Proveedor?: number;      
  id_Departamento?: number;   
}

export interface UpdateUsuarioDTO {
  id: number;
  nombre: string;
  email: string;
  contrasena?: string;
  activo: boolean;
  id_Rol: number;
  id_Proveedor?: number;       
  id_Departamento?: number;   
}