import { TestBed } from '@angular/core/testing';

import { CustomParameterService } from './custom-parameter.service';

describe('CustomParameterService', () => {
  let service: CustomParameterService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(CustomParameterService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
