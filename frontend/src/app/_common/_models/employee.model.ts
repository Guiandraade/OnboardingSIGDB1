import { BaseFilter } from "./pagination.model";

export interface EmployeeRequest {
  name: string;
  cpf: string;
  hireDate: string;
  companyId: number;
  positionId: number;
}

export interface EmployeeUpdateRequest {
  name: string;
  cpf: string;
}

export interface EmployeeResponse {
  id: number;
  name: string;
  cpf: string;
  hireDate: string;
  companyName: string;
  currentPosition: string;
}

export interface EmployeePositionHistoryResponse {
  positionName: string;
  startDate: string;
}

export interface EmployeeAndPositionsResponse {
  id: number;
  name: string;
  cpf: string;
  hireDate: string;
  companyName: string;
  currentPosition: string;
  positionHistory: EmployeePositionHistoryResponse[];
}

export interface EmployeeFilter extends BaseFilter {
  name?: string;
  cpf?: string;
}
