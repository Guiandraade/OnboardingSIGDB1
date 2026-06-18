import { ChangeDetectionStrategy, Component, EventEmitter, Input, Output } from '@angular/core';

@Component({
  selector: 'app-form-error-banner',
  templateUrl: './form-error-banner.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class FormErrorBannerComponent {
  @Input() message = '';
  @Output() closed = new EventEmitter<void>();
}
