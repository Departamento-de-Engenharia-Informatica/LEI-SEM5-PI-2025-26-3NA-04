import { ComponentFixture, TestBed } from '@angular/core/testing';
import { OperationPlans } from './operation-plans';

describe('OperationPlans', () => {
  let component: OperationPlans;
  let fixture: ComponentFixture<OperationPlans>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [OperationPlans]
    })
    .compileComponents();

    fixture = TestBed.createComponent(OperationPlans);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

