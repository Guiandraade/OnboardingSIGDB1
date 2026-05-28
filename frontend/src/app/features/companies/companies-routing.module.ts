import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { CompanyList } from './company-list/company-list';
import { CompanyForm } from './company-form/company-form';

const routes: Routes = [
  { path: '',         component: CompanyList },
  { path: 'new',      component: CompanyForm },
  { path: 'edit/:id', component: CompanyForm }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class CompaniesRoutingModule {}
