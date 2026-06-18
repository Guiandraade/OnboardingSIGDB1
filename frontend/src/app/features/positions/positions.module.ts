import { NgModule } from "@angular/core";
import { CommonModule } from "@angular/common";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { PositionsRoutingModule } from "./positions-routing.module";
import { PositionListComponent } from "./position-list/position-list.component";
import { PositionFormComponent } from "./position-form/position-form.component";
import { SharedModule } from "../../shared/shared.module";

@NgModule({
  declarations: [PositionListComponent, PositionFormComponent],
  imports: [CommonModule, FormsModule, ReactiveFormsModule, PositionsRoutingModule, SharedModule]
})
export class PositionsModule {}
