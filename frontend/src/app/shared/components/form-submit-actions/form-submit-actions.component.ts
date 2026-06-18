import { ChangeDetectionStrategy, Component, EventEmitter, Input, Output } from '@angular/core';

@Component({
  selector: 'app-form-submit-actions',
  templateUrl: './form-submit-actions.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class FormSubmitActionsComponent {
  @Input() isSaving = false;
  @Input() submitLabel = 'Save';
  @Output() cancelled = new EventEmitter<void>();
}
