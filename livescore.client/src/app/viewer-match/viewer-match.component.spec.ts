import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ViewerMatchComponent } from './viewer-match.component';

describe('ViewerMatchComponent', () => {
  let component: ViewerMatchComponent;
  let fixture: ComponentFixture<ViewerMatchComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ViewerMatchComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ViewerMatchComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
