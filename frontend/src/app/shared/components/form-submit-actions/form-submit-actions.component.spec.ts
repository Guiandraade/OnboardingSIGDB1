import { ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';

import { FormSubmitActionsComponent } from './form-submit-actions.component';

describe('FormSubmitActionsComponent', () => {
  let component: FormSubmitActionsComponent;
  let fixture: ComponentFixture<FormSubmitActionsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [FormSubmitActionsComponent]
    }).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(FormSubmitActionsComponent);
    component = fixture.componentInstance;
  });

  it('should render submit label when not saving and no cooldown', () => {
    component.submitLabel = 'Create company';
    fixture.detectChanges();

    const text = fixture.nativeElement.textContent as string;
    expect(text).toContain('Create company');
  });

  it('should disable submit while saving', () => {
    component.isSaving = true;
    fixture.detectChanges();

    const submitButton = fixture.debugElement.queryAll(By.css('button'))[1].nativeElement as HTMLButtonElement;
    expect(submitButton.disabled).toBeTrue();
  });

  it('should emit cancelled event', () => {
    spyOn(component.cancelled, 'emit');
    fixture.detectChanges();

    const cancelButton = fixture.debugElement.queryAll(By.css('button'))[0];
    cancelButton.triggerEventHandler('click', null);

    expect(component.cancelled.emit).toHaveBeenCalled();
  });
});
