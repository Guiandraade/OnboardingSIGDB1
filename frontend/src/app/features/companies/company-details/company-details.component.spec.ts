import { ComponentFixture, TestBed } from '@angular/core/testing';
import { NO_ERRORS_SCHEMA } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { of } from 'rxjs';

import { CompanyService } from 'src/app/shared/services/company.service';
import { EmployeeService } from 'src/app/shared/services/employee.service';
import { ToastService } from 'src/app/shared/services/toast.service';

import { CompanyDetailsComponent } from './company-details.component';

describe('CompanyDetailsComponent', () => {
  let component: CompanyDetailsComponent;
  let fixture: ComponentFixture<CompanyDetailsComponent>;
  const companyServiceMock = {
    getEmployees: jasmine.createSpy('getEmployees').and.returnValue(of({ employeesPositionHistory: [] }))
  };
  const employeeServiceMock = {
    delete: jasmine.createSpy('delete').and.returnValue(of(undefined))
  };
  const toastServiceMock = {
    error: jasmine.createSpy('error'),
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
    TestBed.overrideComponent(CompanyDetailsComponent, { set: { template: '' } });

    await TestBed.configureTestingModule({
      declarations: [ CompanyDetailsComponent ],
      providers: [
        { provide: CompanyService, useValue: companyServiceMock },
        { provide: EmployeeService, useValue: employeeServiceMock },
        { provide: ToastService, useValue: toastServiceMock },
        { provide: ActivatedRoute, useValue: routeMock },
        { provide: Router, useValue: routerMock }
      ],
      schemas: [NO_ERRORS_SCHEMA]
    }).compileComponents();

    fixture = TestBed.createComponent(CompanyDetailsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
