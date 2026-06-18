import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { CompanyResponse, CompanyFilter, CompanyRequest, CompanyAndEmployeesResponse } from '../models/company.model';
import { Observable } from 'rxjs';
import { PagedResponse } from '../models/pagination.model';
import { ApiService } from '../../core/services/api.service';
import { buildPaginationParams } from '../utils/pagination.util';

@Injectable({ providedIn: 'root' })
export class CompanyService {
  private readonly apiUrl = `${environment.apiUrl}/companies`;

  constructor(private api: ApiService) {}

  getAll(filter: CompanyFilter): Observable<PagedResponse<CompanyResponse>> {
    let params = buildPaginationParams(filter);
    if (filter.name)         params = params.set('name', filter.name);
    if (filter.cnpj)         params = params.set('cnpj', filter.cnpj);
    if (filter.foundedIn)    params = params.set('foundedIn', filter.foundedIn);
    if (filter.foundedUntil) params = params.set('foundedUntil', filter.foundedUntil);
    return this.api.get<PagedResponse<CompanyResponse>>(this.apiUrl, params);
  }

  getById(id: number): Observable<CompanyResponse> {
    return this.api.get<CompanyResponse>(`${this.apiUrl}/${id}`);
  }

  getEmployees(id: number): Observable<CompanyAndEmployeesResponse> {
    return this.api.get<CompanyAndEmployeesResponse>(`${this.apiUrl}/${id}/employees`);
  }

  create(request: CompanyRequest): Observable<CompanyResponse> {
    return this.api.post<CompanyResponse>(this.apiUrl, request);
  }

  update(id: number, request: CompanyRequest): Observable<CompanyResponse> {
    return this.api.put<CompanyResponse>(`${this.apiUrl}/${id}`, request);
  }

  delete(id: number): Observable<void> {
    return this.api.delete(`${this.apiUrl}/${id}`);
  }
}
