import { ComponentFixture, TestBed } from '@angular/core/testing';
import { PossibleServicesCategoriesComponent } from './possible-services-categories.component';

describe('PossibleServicesComponent', () => {
	let component: PossibleServicesCategoriesComponent;
	let fixture: ComponentFixture<PossibleServicesCategoriesComponent>;

	beforeEach(async () => {
		await TestBed.configureTestingModule({
			declarations: [PossibleServicesCategoriesComponent]
		})
			.compileComponents();
	});

	beforeEach(() => {
		fixture = TestBed.createComponent(PossibleServicesCategoriesComponent);
		component = fixture.componentInstance;
		fixture.detectChanges();
	});

	it('should create', () => {
		expect(component).toBeTruthy();
	});
});
