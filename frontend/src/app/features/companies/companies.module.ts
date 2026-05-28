import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { CompaniesRoutingModule } from './companies-routing.module';
import { CompanyList } from './company-list/company-list';
import { CompanyForm } from './company-form/company-form';
import { CnpjPipe } from 'src/app/core/pipes/cnpj.pipe';
import { DateBrPipe } from 'src/app/core/pipes/date-br.pipe';
import { SharedModule } from 'src/app/shared/shared.module';

@NgModule({
  declarations: [CompanyList, CompanyForm, CnpjPipe, DateBrPipe],
  imports: [CommonModule, FormsModule, ReactiveFormsModule, CompaniesRoutingModule, SharedModule]
})
export class CompaniesModule {}
