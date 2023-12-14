import { Component, Inject, OnInit } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { DialogData } from '../service-categories/service-categories.component';

@Component({
	selector: 'app-manage-service-category-dialog',
	templateUrl: './manage-service-category-dialog.component.html',
	styleUrls: ['./manage-service-category-dialog.component.css']
})
export class ManageServiceCategoryDialogComponent implements OnInit {

	constructor(
		public dialogRef: MatDialogRef<ManageServiceCategoryDialogComponent>,
		@Inject(MAT_DIALOG_DATA) public data: DialogData,
	) {
	}

	ngOnInit(): void {
	}

	onNoClick(): void {
		this.dialogRef.close();
	}
}
