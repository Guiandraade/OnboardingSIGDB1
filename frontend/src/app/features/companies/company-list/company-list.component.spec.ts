import { ComponentFixture, TestBed } from '@angular/core/testing';
import { NO_ERRORS_SCHEMA } from '@angular/core';
import { of } from 'rxjs';

import { CompanyService } from 'src/app/shared/services/company.service';
import { ToastService } from 'src/app/shared/services/toast.service';

import { CompanyListComponent } from './company-list.component';

describe('CompanyListComponent', () => {
  let component: CompanyListComponent;
  let fixture: ComponentFixture<CompanyListComponent>;
  const companyServiceMock = {
    getAll: jasmine.createSpy('getAll').and.returnValue(of({ data: [], total: 0, pageNumber: 1, pageSize: 5 })),
    delete: jasmine.createSpy('delete').and.returnValue(of(undefined))
  };
  const toastServiceMock = {
    handleHttpError: jasmine.createSpy('handleHttpError'),
    success: jasmine.createSpy('success')
  };

  beforeEach(async () => {
    TestBed.overrideComponent(CompanyListComponent, { set: { template: '' } });

    await TestBed.configureTestingModule({
      declarations: [ CompanyListComponent ],
      providers: [
        { provide: CompanyService, useValue: companyServiceMock },
        { provide: ToastService, useValue: toastServiceMock }
      ],
      schemas: [NO_ERRORS_SCHEMA]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(CompanyListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
