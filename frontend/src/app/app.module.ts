import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { HttpClientModule } from '@angular/common/http';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { SharedModule } from './_shared/shared.module';
import { ToastComponent } from './_shared/toast/toast.component';

@NgModule({
  declarations: [AppComponent, ToastComponent],
  imports: [BrowserModule, HttpClientModule, AppRoutingModule, SharedModule],
  bootstrap: [AppComponent]
})
export class AppModule { }
