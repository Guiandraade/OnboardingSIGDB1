import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { HttpClientModule } from '@angular/common/http';
import { AppRoutingModule } from './app-routing-module';
import { App } from './app';
import { SharedModule } from './shared/shared.module';

@NgModule({
  declarations: [App],
  imports: [BrowserModule, HttpClientModule, AppRoutingModule, SharedModule],
  bootstrap: [App]
})
export class AppModule {}
