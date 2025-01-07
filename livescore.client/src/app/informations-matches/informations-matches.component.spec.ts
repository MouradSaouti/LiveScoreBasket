import { ComponentFixture, TestBed } from '@angular/core/testing';

import { InformationsMatchesComponent } from './informations-matches.component';

describe('InformationsMatchesComponent', () => {
  let component: InformationsMatchesComponent;
  let fixture: ComponentFixture<InformationsMatchesComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [InformationsMatchesComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(InformationsMatchesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
