import { ComponentFixture, TestBed } from '@angular/core/testing';
import { NO_ERRORS_SCHEMA } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { of } from 'rxjs';

import { CompanyService } from 'src/app/shared/services/company.service';
import { ToastService } from 'src/app/shared/services/toast.service';

import { CompanyFormComponent } from './company-form.component';

describe('CompanyFormComponent', () => {
  let component: CompanyFormComponent;
  let fixture: ComponentFixture<CompanyFormComponent>;
  const companyServiceMock = {
    getById: jasmine.createSpy('getById').and.returnValue(of({})),
    update: jasmine.createSpy('update').and.returnValue(of({})),
    create: jasmine.createSpy('create').and.returnValue(of({}))
  };
  const toastServiceMock = {
    success: jasmine.createSpy('success')
  };
  const routeMock = {
    snapshot: {
      paramMap: {
        get: () => null
      }
    }
  };
  const routerMock = {
    navigate: jasmine.createSpy('navigate')
  };

  beforeEach(async () => {
    TestBed.overrideComponent(CompanyFormComponent, { set: { template: '' } });

    await TestBed.configureTestingModule({
      imports: [ReactiveFormsModule],
      declarations: [ CompanyFormComponent ],
      providers: [
        { provide: CompanyService, useValue: companyServiceMock },
        { provide: ToastService, useValue: toastServiceMock },
        { provide: ActivatedRoute, useValue: routeMock },
        { provide: Router, useValue: routerMock }
      ],
      schemas: [NO_ERRORS_SCHEMA]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(CompanyFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
