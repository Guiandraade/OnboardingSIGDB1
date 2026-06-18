import { NgModule } from "@angular/core";
import { PositionFormComponent } from "./position-form/position-form.component";
import { PositionListComponent } from "./position-list/position-list.component";
import { RouterModule, Routes } from "@angular/router";

const routes: Routes = [
  { path: '',         component: PositionListComponent, data: { breadcrumb: 'Positions' } },
  { path: 'new',      component: PositionFormComponent, data: { breadcrumb: 'New Position' } },
  { path: 'edit/:id', component: PositionFormComponent, data: { breadcrumb: 'Edit Position' } }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class PositionsRoutingModule {}
