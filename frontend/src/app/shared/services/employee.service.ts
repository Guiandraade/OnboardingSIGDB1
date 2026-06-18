import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import {
  EmployeeResponse,
  EmployeeFilter,
  EmployeeRequest,
  EmployeeUpdateRequest,
  EmployeeAndPositionsResponse
} from '../models/employee.model';
import { Observable } from 'rxjs';
import { PagedResponse } from '../models/pagination.model';
import { ApiService } from '../../core/services/api.service';
import { buildPaginationParams } from '../utils/pagination.util';

@Injectable({ providedIn: 'root' })
export class EmployeeService {
  private readonly apiUrl = `${environment.apiUrl}/employees`;

  constructor(private api: ApiService) {}

  getAll(filter: EmployeeFilter): Observable<PagedResponse<EmployeeResponse>> {
    let params = buildPaginationParams(filter);
    if (filter.name)        params = params.set('name', filter.name);
    if (filter.cpf)         params = params.set('cpf', filter.cpf);
    if (filter.hiredFrom)   params = params.set('hiredFrom', filter.hiredFrom);
    if (filter.hiredUntil)  params = params.set('hiredUntil', filter.hiredUntil);
    return this.api.get<PagedResponse<EmployeeResponse>>(this.apiUrl, params);
  }

  getById(id: number): Observable<EmployeeResponse> {
    return this.api.get<EmployeeResponse>(`${this.apiUrl}/${id}`);
  }

  getPositionHistory(id: number): Observable<EmployeeAndPositionsResponse> {
    return this.api.get<EmployeeAndPositionsResponse>(`${this.apiUrl}/${id}/positions`);
  }

  create(request: EmployeeRequest): Observable<EmployeeResponse> {
    return this.api.post<EmployeeResponse>(this.apiUrl, request);
  }

  update(id: number, request: EmployeeUpdateRequest): Observable<EmployeeResponse> {
    return this.api.put<EmployeeResponse>(`${this.apiUrl}/${id}`, request);
  }

  changePosition(id: number, positionId: number): Observable<void> {
    return this.api.post<void>(`${this.apiUrl}/${id}/positions`, { positionId });
  }

  delete(id: number): Observable<void> {
    return this.api.delete(`${this.apiUrl}/${id}`);
  }
}
