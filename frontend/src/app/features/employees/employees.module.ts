import { NgModule } from "@angular/core";
import { CommonModule } from "@angular/common";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { EmployeesRoutingModule } from "./employees-routing.module";
import { EmployeeListComponent } from "./employee-list/employee-list.component";
import { EmployeeFormComponent } from "./employee-form/employee-form.component";
import { EmployeeDetailsComponent } from "./employee-details/employee-details.component";
import { SharedModule } from "../../shared/shared.module";

@NgModule({
  declarations: [EmployeeListComponent, EmployeeFormComponent, EmployeeDetailsComponent],
  imports: [CommonModule, FormsModule, ReactiveFormsModule, EmployeesRoutingModule, SharedModule]
})
export class EmployeesModule {}
