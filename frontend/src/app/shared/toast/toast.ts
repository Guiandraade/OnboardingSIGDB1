import { Component, OnDestroy, OnInit } from '@angular/core';
import { Subscription } from 'rxjs';
import { Toast, ToastService } from 'src/app/core/services/toast.service';

@Component({
  selector: 'app-toast',
  templateUrl: './toast.html',
  styleUrls: ['./toast.css']
})
export class ToastComponent implements OnInit, OnDestroy {
  toast: Toast | null = null;
  private sub!: Subscription;

  constructor(private toastService: ToastService) {}

  ngOnInit(): void {
    this.sub = this.toastService.toast$.subscribe(t => this.toast = t);
  }

  ngOnDestroy(): void {
    this.sub.unsubscribe();
  }

  close(): void {
    this.toastService.clear();
  }
}
