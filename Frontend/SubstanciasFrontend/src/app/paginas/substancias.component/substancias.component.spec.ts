import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SubstanciasComponent } from './substancias.component';

describe('SubstanciasComponent', () => {
  let component: SubstanciasComponent;
  let fixture: ComponentFixture<SubstanciasComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SubstanciasComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(SubstanciasComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
