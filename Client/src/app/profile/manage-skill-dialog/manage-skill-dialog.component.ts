import { Component, Inject, OnInit } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { Service } from 'src/app/_models/Service';
import { ServiceCategory } from 'src/app/_models/ServiceCategory';
import { ServiceCategoriesService } from 'src/app/_services/service-categories.service';
import { SkillDialogData } from '../manage-skills/manage-skills.component';

@Component({
	selector: 'app-manage-skill-dialog',
	templateUrl: './manage-skill-dialog.component.html',
	styleUrls: ['./manage-skill-dialog.component.css']
})
export class ManageSkillDialogComponent implements OnInit {
	serviceCategories: ServiceCategory[];

	constructor(
		public dialogRef: MatDialogRef<ManageSkillDialogComponent>,
		@Inject(MAT_DIALOG_DATA) public data: SkillDialogData,
		private serviceCategoriesService: ServiceCategoriesService
	) {
	}

	ngOnInit(): void {
		this.getServiceCategories();
	}

	onNoClick(): void {
		this.dialogRef.close();
	}

	getServiceCategories() {
		this.serviceCategoriesService.getAll().subscribe(data => {
			this.serviceCategories = <ServiceCategory[]>data;
		})
	}
}
