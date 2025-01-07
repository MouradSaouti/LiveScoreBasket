import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ConfigdiversesComponent } from './configdiverses.component';

describe('ConfigdiversesComponent', () => {
  let component: ConfigdiversesComponent;
  let fixture: ComponentFixture<ConfigdiversesComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ConfigdiversesComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ConfigdiversesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
