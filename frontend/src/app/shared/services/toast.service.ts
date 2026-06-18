import { HttpErrorResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Subject } from 'rxjs';
import { extractErrorMessages } from '../utils/server-error.util';

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

  handleHttpError(err: HttpErrorResponse, fallback = 'An unexpected error occurred.'): void {
    const messages = extractErrorMessages(err.error);
    const message = messages.length ? messages.join(', ') : fallback;
    this.error(message);
  }
}
