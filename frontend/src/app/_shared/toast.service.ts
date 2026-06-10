import { Injectable } from '@angular/core';
import { Subject } from 'rxjs';

export type ToastType = 'success' | 'error';

export interface ToastItem {
  id: number;
  message: string;
  type: ToastType;
}

@Injectable({ providedIn: 'root' })
export class ToastService {
  private counter = 0;
  readonly toast$ = new Subject<ToastItem>();

  success(message: string): void {
    this.toast$.next({ id: ++this.counter, message, type: 'success' });
  }

  error(message: string): void {
    this.toast$.next({ id: ++this.counter, message, type: 'error' });
  }
}
