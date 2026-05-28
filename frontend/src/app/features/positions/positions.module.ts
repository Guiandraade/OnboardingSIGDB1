import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { PositionsRoutingModule } from './positions-routing.module';
import { PositionList } from './position-list/position-list';
import { PositionForm } from './position-form/position-form';
import { SharedModule } from 'src/app/shared/shared.module';

@NgModule({
  declarations: [PositionList, PositionForm],
  imports: [CommonModule, FormsModule, ReactiveFormsModule, PositionsRoutingModule, SharedModule]
})
export class PositionsModule {}
