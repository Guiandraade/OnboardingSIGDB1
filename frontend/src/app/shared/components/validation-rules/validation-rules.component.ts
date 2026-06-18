import { ChangeDetectionStrategy, Component, Input } from '@angular/core';

export interface ValidationRule {
  text: string;
  passed: boolean;
  failed: boolean;
}

@Component({
  selector: 'app-validation-rules',
  templateUrl: './validation-rules.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ValidationRulesComponent {
  @Input() visible = false;
  @Input() rules: ValidationRule[] = [];

  getIcon(rule: ValidationRule): string {
    if (rule.passed) {
      return 'check_circle';
    }
    if (rule.failed) {
      return 'cancel';
    }
    return 'radio_button_unchecked';
  }
}
