import { ComponentFixture, TestBed } from '@angular/core/testing';
import { IncidentTypes } from './incident-types';

describe('IncidentTypes', () => {
  let component: IncidentTypes;
  let fixture: ComponentFixture<IncidentTypes>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [IncidentTypes]
    })
    .compileComponents();

    fixture = TestBed.createComponent(IncidentTypes);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

