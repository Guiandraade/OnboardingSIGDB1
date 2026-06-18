import { ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';

import { FormErrorBannerComponent } from './form-error-banner.component';

describe('FormErrorBannerComponent', () => {
  let component: FormErrorBannerComponent;
  let fixture: ComponentFixture<FormErrorBannerComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [FormErrorBannerComponent]
    }).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(FormErrorBannerComponent);
    component = fixture.componentInstance;
  });

  it('should render message when provided', () => {
    component.message = 'Server error';
    fixture.detectChanges();

    const text = fixture.nativeElement.textContent as string;
    expect(text).toContain('Server error');
  });

  it('should emit close event when close button is clicked', () => {
    component.message = 'Server error';
    spyOn(component.closed, 'emit');
    fixture.detectChanges();

    const button = fixture.debugElement.query(By.css('.form-error-banner-close'));
    button.triggerEventHandler('click', null);

    expect(component.closed.emit).toHaveBeenCalled();
  });
});
