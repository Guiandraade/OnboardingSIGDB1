import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { HttpClient, HttpParams } from '@angular/common/http';
import { CompanyResponse, CompanyFilter, CompanyRequest, CompanyAndEmployeesResponse } from '../_models/company.model';
import { Observable } from 'rxjs';
import { PagedResponse } from '../_models/pagination.model';

@Injectable({ providedIn: 'root' })
export class CompanyService {
  private readonly apiUrl = `${environment.apiUrl}/companies`;

  constructor(private http: HttpClient) {}

  getAll(filter: CompanyFilter): Observable<PagedResponse<CompanyResponse>> {
    let params = new HttpParams();
    if (filter) {
      if (filter.name) params = params.set('name', filter.name);
      if (filter.cnpj) params = params.set('cnpj', filter.cnpj);
      if (filter.pageNumber) params = params.set('pageNumber', filter.pageNumber.toString());
      if (filter.pageSize) params = params.set('pageSize', filter.pageSize.toString());
    }
    return this.http.get<PagedResponse<CompanyResponse>>(this.apiUrl, { params });
  }

  getById(id: number): Observable<CompanyResponse> {
    return this.http.get<CompanyResponse>(`${this.apiUrl}/${id}`);
  }

  getEmployees(id: number): Observable<CompanyAndEmployeesResponse> {
    return this.http.get<CompanyAndEmployeesResponse>(`${this.apiUrl}/${id}/employees`);
  }

  create(request: CompanyRequest): Observable<CompanyResponse> {
    return this.http.post<CompanyResponse>(this.apiUrl, request);
  }

  update(id: number, request: CompanyRequest): Observable<CompanyResponse> {
    return this.http.put<CompanyResponse>(`${this.apiUrl}/${id}`, request);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
