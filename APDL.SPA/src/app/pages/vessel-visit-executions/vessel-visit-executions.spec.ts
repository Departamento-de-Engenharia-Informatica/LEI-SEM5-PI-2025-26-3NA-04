import { ComponentFixture, TestBed } from '@angular/core/testing';
import { VesselVisitExecutions } from './vessel-visit-executions';

describe('VesselVisitExecutions', () => {
  let component: VesselVisitExecutions;
  let fixture: ComponentFixture<VesselVisitExecutions>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [VesselVisitExecutions]
    })
    .compileComponents();

    fixture = TestBed.createComponent(VesselVisitExecutions);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

