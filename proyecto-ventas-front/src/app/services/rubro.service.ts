import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { Rubro, CreateRubroDTO, UpdateRubroDTO } from '../models/rubro.model';

@Injectable({ providedIn: 'root' })
export class RubroService {
  private readonly url = `${environment.apiUrl}/RubroControlador`;

  constructor(private http: HttpClient) { }

  listar(): Observable<Rubro[]> {
    return this.http.get<Rubro[]>(this.url);
  }

  obtener(id: number): Observable<Rubro> {
    return this.http.get<Rubro>(`${this.url}/${id}`);
  }

  crear(dto: CreateRubroDTO): Observable<void> {
    return this.http.post<void>(this.url, dto);
  }

  actualizar(dto: UpdateRubroDTO): Observable<void> {
    return this.http.put<void>(this.url, dto);
  }

  eliminar(id: number): Observable<void> {
    return this.http.delete<void>(`${this.url}/${id}`);
  }
}
