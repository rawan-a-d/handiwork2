import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ManageServiceCategoryDialogComponent } from './manage-service-category-dialog.component';

describe('NewServiceCategoryDialogComponent', () => {
	let component: ManageServiceCategoryDialogComponent;
	let fixture: ComponentFixture<ManageServiceCategoryDialogComponent>;

	beforeEach(async () => {
		await TestBed.configureTestingModule({
			declarations: [ManageServiceCategoryDialogComponent]
		})
			.compileComponents();
	});

	beforeEach(() => {
		fixture = TestBed.createComponent(ManageServiceCategoryDialogComponent);
		component = fixture.componentInstance;
		fixture.detectChanges();
	});

	it('should create', () => {
		expect(component).toBeTruthy();
	});
});
