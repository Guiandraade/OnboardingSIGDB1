import {BaseFilter} from './pagination.model';

export interface CompanyRequest{
  name: string;
  cnpj: string;
  foundationDate?: string;
}

export interface CompanyResponse{
  id: number;
  name: string;
  cnpj: string;
  foundationDate: string | null;
}

export interface CompanyFilter extends BaseFilter{
  name?: string;
  cnpj?: string;
  foundedIn?: string;
  foundedUntil?: string;
}

