import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ShippingAgents } from './shipping-agents';

describe('ShippingAgents', () => {
  let component: ShippingAgents;
  let fixture: ComponentFixture<ShippingAgents>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ShippingAgents]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ShippingAgents);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
