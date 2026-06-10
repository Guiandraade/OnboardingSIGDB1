import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { HttpClient, HttpParams } from '@angular/common/http';
import {
  EmployeeResponse,
  EmployeeFilter,
  EmployeeRequest,
  EmployeeUpdateRequest,
  EmployeeAndPositionsResponse
} from '../_models/employee.model';
import { Observable } from 'rxjs';
import { PagedResponse } from '../_models/pagination.model';

@Injectable({ providedIn: 'root' })
export class EmployeeService {
  private readonly apiUrl = `${environment.apiUrl}/employees`;

  constructor(private http: HttpClient) {}

  getAll(filter: EmployeeFilter): Observable<PagedResponse<EmployeeResponse>> {
    let params = new HttpParams();
    if (filter) {
      if (filter.name) params = params.set('name', filter.name);
      if (filter.cpf)  params = params.set('cpf', filter.cpf);
      if (filter.pageNumber) params = params.set('pageNumber', filter.pageNumber.toString());
      if (filter.pageSize)   params = params.set('pageSize', filter.pageSize.toString());
    }
    return this.http.get<PagedResponse<EmployeeResponse>>(this.apiUrl, { params });
  }

  getById(id: number): Observable<EmployeeResponse> {
    return this.http.get<EmployeeResponse>(`${this.apiUrl}/${id}`);
  }

  getPositionHistory(id: number): Observable<EmployeeAndPositionsResponse> {
    return this.http.get<EmployeeAndPositionsResponse>(`${this.apiUrl}/${id}/positions`);
  }

  create(request: EmployeeRequest): Observable<EmployeeResponse> {
    return this.http.post<EmployeeResponse>(this.apiUrl, request);
  }

  update(id: number, request: EmployeeUpdateRequest): Observable<EmployeeResponse> {
    return this.http.put<EmployeeResponse>(`${this.apiUrl}/${id}`, request);
  }

  changePosition(id: number, positionId: number): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/${id}/positions`, { positionId });
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
