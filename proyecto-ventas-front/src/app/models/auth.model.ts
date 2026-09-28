export interface LoginRequest {
  email: string;
  password: string;
}

export interface AuthResponse {
  token: string;
  nombre: string;
  email: string;
  rol: string;      
  idRol: number;    
}

export type Rol = 
  | 'Administrador' 
  | 'GestorCompras' 
  | 'AdministradorProveedor' 
  | 'Auditor'
  | 'CreadorPedidos';   
