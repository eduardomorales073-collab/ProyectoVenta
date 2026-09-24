import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { Usuario, CreateUsuarioDTO, UpdateUsuarioDTO } from '../models/usuario.model';

import { PermisoUsuario, UpdatePermisosDTO } from '../models/permiso.model';

@Injectable({ providedIn: 'root' })
export class UsuarioService {
  private readonly url = `${environment.apiUrl}/UsuarioControlador`;

  constructor(private http: HttpClient) { }

  listar(): Observable<Usuario[]> {
    return this.http.get<Usuario[]>(this.url);
  }

  obtener(id: number): Observable<Usuario> {
    return this.http.get<Usuario>(`${this.url}/${id}`);
  }

  crear(dto: CreateUsuarioDTO): Observable<void> {
    return this.http.post<void>(this.url, dto);
  }

  actualizar(dto: UpdateUsuarioDTO): Observable<void> {
    return this.http.put<void>(this.url, dto);
  }

  eliminar(id: number): Observable<void> {
    return this.http.delete<void>(`${this.url}/${id}`);
  }

  obtenerPermisos(idUsuario: number): Observable<PermisoUsuario> {
    return this.http.get<PermisoUsuario>(`${this.url}/${idUsuario}/permisos`);
  }

  actualizarPermisos(idUsuario: number, dto: UpdatePermisosDTO): Observable<void> {
    return this.http.put<void>(`${this.url}/${idUsuario}/permisos`, dto);
  }
}
