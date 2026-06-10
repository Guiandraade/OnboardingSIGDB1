import { CompanyDetailsComponent } from './company-details/company-details.component';
import { CompanyFormComponent } from './company-form/company-form.component';
import { NgModule } from "@angular/core";
import { RouterModule, Routes } from "@angular/router";
import { CompanyListComponent } from "./company-list/company-list.component";

const routes: Routes = [
  { path: '',         component: CompanyListComponent,    data: { breadcrumb: 'Companies' } },
  { path: 'new',      component: CompanyFormComponent,    data: { breadcrumb: 'New Company' } },
  { path: 'edit/:id', component: CompanyFormComponent,    data: { breadcrumb: 'Edit Company' } },
  { path: ':id',      component: CompanyDetailsComponent, data: { breadcrumb: 'Company Details' } }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class CompaniesRoutingModule {}
