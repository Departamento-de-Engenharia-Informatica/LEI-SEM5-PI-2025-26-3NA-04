import { ComponentFixture, TestBed } from '@angular/core/testing';

import { VisitNotifications } from './visit-notifications';

describe('VisitNotifications', () => {
  let component: VisitNotifications;
  let fixture: ComponentFixture<VisitNotifications>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [VisitNotifications]
    })
    .compileComponents();

    fixture = TestBed.createComponent(VisitNotifications);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
