import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { Departamento, CreateDepartamentoDTO, UpdateDepartamentoDTO } from '../models/departamento.model';

@Injectable({ providedIn: 'root' })
export class DepartamentoService {
  private readonly url = `${environment.apiUrl}/DepartamentoControlador`;

  constructor(private http: HttpClient) { }

  listar(): Observable<Departamento[]> {
    return this.http.get<Departamento[]>(this.url);
  }

  obtener(id: number): Observable<Departamento> {
    return this.http.get<Departamento>(`${this.url}/${id}`);
  }

  crear(dto: CreateDepartamentoDTO): Observable<void> {
    return this.http.post<void>(this.url, dto);
  }

  actualizar(dto: UpdateDepartamentoDTO): Observable<void> {
    return this.http.put<void>(this.url, dto);
  }

  eliminar(id: number): Observable<void> {
    return this.http.delete<void>(`${this.url}/${id}`);
  }
}
