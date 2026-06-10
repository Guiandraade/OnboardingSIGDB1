import { BaseFilter } from "./pagination.model";

export interface CompanyRequest {
  name: string;
  cnpj: string;
  foundationDate: string;
}

export interface CompanyResponse {
  id: number;
  name: string;
  cnpj: string;
  foundationDate: string;
}

export interface CompanyDetailsResponse {
  employeeId: number;
  employeeName: string;
  positionName: string;
  hiringDate: string;
}

export interface CompanyAndEmployeesResponse {
  id: number;
  name: string;
  cnpj: string;
  foundationDate: string;
  employeesPositionHistory: CompanyDetailsResponse[];
}

export interface CompanyFilter extends BaseFilter {
  name?: string;
  cnpj?: string;
}
