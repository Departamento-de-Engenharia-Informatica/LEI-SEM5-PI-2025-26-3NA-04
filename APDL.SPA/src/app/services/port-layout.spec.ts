import { TestBed } from '@angular/core/testing';

import { PortLayout } from './port-layout';

describe('PortLayout', () => {
  let service: PortLayout;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(PortLayout);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
