import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PortScene } from './port-scene';

describe('PortScene', () => {
  let component: PortScene;
  let fixture: ComponentFixture<PortScene>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PortScene]
    })
    .compileComponents();

    fixture = TestBed.createComponent(PortScene);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
