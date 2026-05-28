import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CompanyForm } from './company-form';

describe('CompanyForm', () => {
  let component: CompanyForm;
  let fixture: ComponentFixture<CompanyForm>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ CompanyForm ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(CompanyForm);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
