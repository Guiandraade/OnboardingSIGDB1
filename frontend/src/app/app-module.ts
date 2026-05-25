import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { HttpClientModule } from '@angular/common/http';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { AppRoutingModule } from './app-routing-module';
import { App } from './app';
import { PositionList } from './features/positions/position-list/position-list';
import { PositionForm } from './features/positions/position-form/position-form';

@NgModule({
  declarations: [App, PositionList, PositionForm],
  imports: [BrowserModule, AppRoutingModule, HttpClientModule, FormsModule, ReactiveFormsModule],
  providers: [],
  bootstrap: [App]
})
export class AppModule {}
