import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ManageSkillDialogComponent } from './manage-skill-dialog.component';

describe('ManageSkillDialogComponent', () => {
	let component: ManageSkillDialogComponent;
	let fixture: ComponentFixture<ManageSkillDialogComponent>;

	beforeEach(async () => {
		await TestBed.configureTestingModule({
			declarations: [ManageSkillDialogComponent]
		})
			.compileComponents();
	});

	beforeEach(() => {
		fixture = TestBed.createComponent(ManageSkillDialogComponent);
		component = fixture.componentInstance;
		fixture.detectChanges();
	});

	it('should create', () => {
		expect(component).toBeTruthy();
	});
});
