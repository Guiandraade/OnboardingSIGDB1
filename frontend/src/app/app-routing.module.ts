import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

const routes: Routes = [
{ path: '', redirectTo: '/home', pathMatch: 'full' },
{
  path: 'home',
  loadChildren: () => import('./home/home.module').then(m => m.HomeModule)
},
{
  path: 'positions',
  loadChildren: () => import('./positions/positions.module').then(m => m.PositionsModule)
},
{
  path: 'employees',
  loadChildren: () => import('./employees/employees.module').then(m => m.EmployeesModule)
},
{
  path: 'companies',
  loadChildren: () => import('./companies/companies.module').then(m => m.CompaniesModule)
},
{ path: '**', redirectTo: '/positions'}
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
