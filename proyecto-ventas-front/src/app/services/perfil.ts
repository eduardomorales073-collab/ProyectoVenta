import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { Perfil, UpdatePerfilDTO, ChangePasswordDTO } from '../models/perfil.model';

@Injectable({ providedIn: 'root' })
export class PerfilService {
  private readonly url = `${environment.apiUrl}/PerfilControlador`;

  constructor(private http: HttpClient) { }

  obtener(): Observable<Perfil> {
    return this.http.get<Perfil>(this.url);
  }

  actualizar(dto: UpdatePerfilDTO): Observable<{ mensaje: string }> {
    return this.http.put<{ mensaje: string }>(this.url, dto);
  }

  cambiarContrasena(dto: ChangePasswordDTO): Observable<{ mensaje: string }> {
    return this.http.post<{ mensaje: string }>(`${this.url}/cambiar-contrasena`, dto);
  }
}
