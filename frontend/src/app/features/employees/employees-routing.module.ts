import { NgModule } from "@angular/core";
import { RouterModule, Routes } from "@angular/router";
import { EmployeeListComponent } from "./employee-list/employee-list.component";
import { EmployeeFormComponent } from "./employee-form/employee-form.component";
import { EmployeeDetailsComponent } from "./employee-details/employee-details.component";

const routes: Routes = [
  { path: '',         component: EmployeeListComponent,    data: { breadcrumb: 'Employees' } },
  { path: 'new',      component: EmployeeFormComponent,    data: { breadcrumb: 'New Employee' } },
  { path: 'edit/:id', component: EmployeeFormComponent,    data: { breadcrumb: 'Edit Employee' } },
  { path: ':id',      component: EmployeeDetailsComponent, data: { breadcrumb: 'Employee Details' } }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class EmployeesRoutingModule {}
