import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ValidationRule, ValidationRulesComponent } from './validation-rules.component';

describe('ValidationRulesComponent', () => {
  let component: ValidationRulesComponent;
  let fixture: ComponentFixture<ValidationRulesComponent>;

  const rules: ValidationRule[] = [
    { text: 'At least 3 characters', passed: true, failed: false },
    { text: 'Maximum 100 characters', passed: false, failed: true }
  ];

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ValidationRulesComponent]
    }).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ValidationRulesComponent);
    component = fixture.componentInstance;
  });

  it('should render rules when visible', () => {
    component.visible = true;
    component.rules = rules;
    fixture.detectChanges();

    const text = fixture.nativeElement.textContent as string;
    expect(text).toContain('At least 3 characters');
    expect(text).toContain('Maximum 100 characters');
  });

  it('should return correct icon for rule states', () => {
    expect(component.getIcon({ text: '', passed: true, failed: false })).toBe('check_circle');
    expect(component.getIcon({ text: '', passed: false, failed: true })).toBe('cancel');
    expect(component.getIcon({ text: '', passed: false, failed: false })).toBe('radio_button_unchecked');
  });
});
