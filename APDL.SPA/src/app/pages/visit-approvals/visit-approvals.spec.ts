import { ComponentFixture, TestBed } from '@angular/core/testing';

import { VisitApprovals } from './visit-approvals';

describe('VisitApprovals', () => {
  let component: VisitApprovals;
  let fixture: ComponentFixture<VisitApprovals>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [VisitApprovals]
    })
    .compileComponents();

    fixture = TestBed.createComponent(VisitApprovals);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
