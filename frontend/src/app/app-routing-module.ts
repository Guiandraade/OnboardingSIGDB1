import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

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
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule {}
