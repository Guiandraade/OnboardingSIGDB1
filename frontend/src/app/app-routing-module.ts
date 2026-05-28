import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

<<<<<<< HEAD
const routes: Routes = [
  { path: '', redirectTo: '/positions', pathMatch: 'full' },
  {
    path: 'positions',
    loadChildren: () => import('./features/positions/positions.module').then(m => m.PositionsModule)
  },
  {
    path: 'companies',
    loadChildren: () => import('./features/companies/companies.module').then(m => m.CompaniesModule)
  },
  { path: '**', redirectTo: '/positions' }
=======
const routes: Routes = [{
  path: 'positions', component: PositionList},
{
  path: 'positions/new', component: PositionForm},
{
  path: 'positions/edit/:id', component: PositionForm},
{
  path: '', redirectTo: '/positions', pathMatch: 'full'},
{
  path: 'companies', component: CompanyList},
{
  path: 'companies/new', component: CompanyForm},
{
  path: 'companies/edit/:id',  component: CompanyForm},
{
  path: 'companies/:id/employees',  component: EmployeeList},
}
>>>>>>> origin/main
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule {}
