import { ComponentFixture, TestBed } from '@angular/core/testing';
import { NO_ERRORS_SCHEMA } from '@angular/core';
import { of } from 'rxjs';

import { PositionService } from 'src/app/shared/services/position.service';
import { ToastService } from 'src/app/shared/services/toast.service';

import { PositionListComponent } from './position-list.component';

describe('PositionListComponent', () => {
  let component: PositionListComponent;
  let fixture: ComponentFixture<PositionListComponent>;
  const positionServiceMock = {
    getAll: jasmine.createSpy('getAll').and.returnValue(of({ data: [], total: 0, pageNumber: 1, pageSize: 5 })),
    delete: jasmine.createSpy('delete').and.returnValue(of(undefined))
  };
  const toastServiceMock = {
    handleHttpError: jasmine.createSpy('handleHttpError'),
    success: jasmine.createSpy('success')
  };

  beforeEach(async () => {
    TestBed.overrideComponent(PositionListComponent, { set: { template: '' } });

    await TestBed.configureTestingModule({
      declarations: [ PositionListComponent ],
      providers: [
        { provide: PositionService, useValue: positionServiceMock },
        { provide: ToastService, useValue: toastServiceMock }
      ],
      schemas: [NO_ERRORS_SCHEMA]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(PositionListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
