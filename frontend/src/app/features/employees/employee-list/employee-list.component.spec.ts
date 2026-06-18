import { ComponentFixture, TestBed } from '@angular/core/testing';
import { NO_ERRORS_SCHEMA } from '@angular/core';
import { of } from 'rxjs';

import { EmployeeService } from 'src/app/shared/services/employee.service';
import { ToastService } from 'src/app/shared/services/toast.service';

import { EmployeeListComponent } from './employee-list.component';

describe('EmployeeListComponent', () => {
  let component: EmployeeListComponent;
  let fixture: ComponentFixture<EmployeeListComponent>;
  const employeeServiceMock = {
    getAll: jasmine.createSpy('getAll').and.returnValue(of({ data: [], total: 0, pageNumber: 1, pageSize: 5 })),
    delete: jasmine.createSpy('delete').and.returnValue(of(undefined))
  };
  const toastServiceMock = {
    handleHttpError: jasmine.createSpy('handleHttpError'),
    success: jasmine.createSpy('success')
  };

  beforeEach(async () => {
    TestBed.overrideComponent(EmployeeListComponent, { set: { template: '' } });

    await TestBed.configureTestingModule({
      declarations: [ EmployeeListComponent ],
      providers: [
        { provide: EmployeeService, useValue: employeeServiceMock },
        { provide: ToastService, useValue: toastServiceMock }
      ],
      schemas: [NO_ERRORS_SCHEMA]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(EmployeeListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
