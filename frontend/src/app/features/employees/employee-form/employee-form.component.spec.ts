import { ComponentFixture, TestBed } from '@angular/core/testing';
import { NO_ERRORS_SCHEMA } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { of } from 'rxjs';

import { CompanyService } from 'src/app/shared/services/company.service';
import { EmployeeService } from 'src/app/shared/services/employee.service';
import { PositionService } from 'src/app/shared/services/position.service';
import { ToastService } from 'src/app/shared/services/toast.service';

import { EmployeeFormComponent } from './employee-form.component';

describe('EmployeeFormComponent', () => {
  let component: EmployeeFormComponent;
  let fixture: ComponentFixture<EmployeeFormComponent>;
  const employeeServiceMock = {
    getById: jasmine.createSpy('getById').and.returnValue(of({})),
    update: jasmine.createSpy('update').and.returnValue(of({})),
    create: jasmine.createSpy('create').and.returnValue(of({}))
  };
  const companyServiceMock = {
    getAll: jasmine.createSpy('getAll').and.returnValue(of({ data: [], total: 0, pageNumber: 1, pageSize: 100 }))
  };
  const positionServiceMock = {
    getAll: jasmine.createSpy('getAll').and.returnValue(of({ data: [], total: 0, pageNumber: 1, pageSize: 100 }))
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
    TestBed.overrideComponent(EmployeeFormComponent, { set: { template: '' } });

    await TestBed.configureTestingModule({
      imports: [ReactiveFormsModule],
      declarations: [ EmployeeFormComponent ],
      providers: [
        { provide: EmployeeService, useValue: employeeServiceMock },
        { provide: CompanyService, useValue: companyServiceMock },
        { provide: PositionService, useValue: positionServiceMock },
        { provide: ToastService, useValue: toastServiceMock },
        { provide: ActivatedRoute, useValue: routeMock },
        { provide: Router, useValue: routerMock }
      ],
      schemas: [NO_ERRORS_SCHEMA]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(EmployeeFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
