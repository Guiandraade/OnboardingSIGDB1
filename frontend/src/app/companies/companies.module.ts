import { NgModule } from "@angular/core";
import { CommonModule } from "@angular/common";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { SharedModule } from "../_shared/shared.module";
import { CompanyListComponent } from "./company-list/company-list.component";
import { CompanyFormComponent } from "./company-form/company-form.component";
import { CompanyDetailsComponent } from "./company-details/company-details.component";
import { CompaniesRoutingModule } from "./companies-routing.module";

@NgModule({
  declarations: [CompanyListComponent, CompanyFormComponent, CompanyDetailsComponent],
  imports: [CommonModule, FormsModule, ReactiveFormsModule, CompaniesRoutingModule, SharedModule]
})
export class CompaniesModule {}
