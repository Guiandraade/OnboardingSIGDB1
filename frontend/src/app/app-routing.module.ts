import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

const routes: Routes = [
{ path: '', redirectTo: '/home', pathMatch: 'full' },
{
  path: 'home',
  loadChildren: () => import('./features/home/home.module').then(m => m.HomeModule)
},
{
  path: 'positions',
  loadChildren: () => import('./features/positions/positions.module').then(m => m.PositionsModule)
},
{
  path: 'employees',
  loadChildren: () => import('./features/employees/employees.module').then(m => m.EmployeesModule)
},
{
  path: 'companies',
  loadChildren: () => import('./features/companies/companies.module').then(m => m.CompaniesModule)
},
{
  path: '**',
  loadChildren: () => import('./features/not-found/not-found.module').then(m => m.NotFoundModule),
  data: { breadcrumb: '404' }
}
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
