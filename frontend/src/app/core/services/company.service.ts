import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { PagedResponse } from '../models/pagination.model';
import { CompanyFilter, CompanyRequest, CompanyResponse } from '../models/company.model';
import { environment } from 'src/environments/environment';
import { BaseApiService } from './base-api.service';

@Injectable({ providedIn: 'root' })
export class CompanyService extends BaseApiService<CompanyResponse, CompanyRequest> {

  protected apiUrl = `${environment.apiUrl}/companies`;

  constructor(http: HttpClient) { super(http); }

  getCompanies(filter?: CompanyFilter): Observable<PagedResponse<CompanyResponse>> {
    return this.getAll(filter ? this.buildParams(filter as Record<string, unknown>) : undefined);
  }
}
