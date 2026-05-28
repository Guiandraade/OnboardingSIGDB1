import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

export interface Toast {
  message: string;
  type: 'success' | 'error';
}

@Injectable({ providedIn: 'root' })
export class ToastService {
  private subject = new BehaviorSubject<Toast | null>(null);
  toast$ = this.subject.asObservable();

  private timer: ReturnType<typeof setTimeout> | null = null;

  show(message: string, type: 'success' | 'error' = 'success'): void {
    if (this.timer) {
      clearTimeout(this.timer);
    }

    this.subject.next({ message, type });
    this.timer = setTimeout(() => this.subject.next(null), 3500);
  }

  clear(): void {
    if (this.timer) {
      clearTimeout(this.timer);
    }

    this.subject.next(null);
  }
}
