import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { PositionList } from './features/positions/position-list/position-list';
import { PositionForm } from './features/positions/position-form/position-form';

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
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
