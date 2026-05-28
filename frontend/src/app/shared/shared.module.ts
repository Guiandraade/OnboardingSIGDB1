import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ConfirmModal } from './confirm-modal/confirm-modal';
import { ToastComponent } from './toast/toast';

@NgModule({
  declarations: [ConfirmModal, ToastComponent],
  imports: [CommonModule],
  exports: [ConfirmModal, ToastComponent]
})
export class SharedModule {}
