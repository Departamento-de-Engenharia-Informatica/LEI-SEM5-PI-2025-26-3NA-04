import { ComponentFixture, TestBed } from '@angular/core/testing';

import { StorageAreas } from './storage-areas';

describe('StorageAreas', () => {
  let component: StorageAreas;
  let fixture: ComponentFixture<StorageAreas>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [StorageAreas]
    })
    .compileComponents();

    fixture = TestBed.createComponent(StorageAreas);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
