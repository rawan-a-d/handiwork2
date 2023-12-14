import { Component, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { ServiceCategory } from 'src/app/_models/ServiceCategory';
import { ServiceCategoriesService } from 'src/app/_services/service-categories.service';
import { ManageServiceCategoryDialogComponent } from '../manage-service-category-dialog/manage-service-category-dialog.component';

export interface DialogData {
	serviceCategory: ServiceCategory;
	action: string;
}

@Component({
	selector: 'app-service-categories',
	templateUrl: './service-categories.component.html',
	styleUrls: ['./service-categories.component.css']
})
export class ServiceCategoriesComponent implements OnInit {
	serviceCategories: ServiceCategory[] = [];
	action = "New";

	constructor(public dialog: MatDialog,
		private serviceCategoriesService: ServiceCategoriesService) { }

	ngOnInit(): void {
		this.getAll();
	}

	create(serviceCategory: ServiceCategory) {
		this.serviceCategoriesService.create(serviceCategory)
			.subscribe((newServiceCategory) => {
				// add item to array
				this.serviceCategories.push(<ServiceCategory>newServiceCategory);
			});
	}

	getAll() {
		this.serviceCategoriesService.getAll().subscribe(data => {
			this.serviceCategories = <ServiceCategory[]>data;
		})
	}

	update(serviceCategory: ServiceCategory) {
		this.serviceCategoriesService.update(serviceCategory)
			.subscribe(() => {
				// replace item in array
				var index = this.serviceCategories.findIndex(sc => sc.id == serviceCategory.id);
				this.serviceCategories.splice(index, 1, serviceCategory);
			});
	}

	delete(id: number) {
		this.serviceCategoriesService.delete(id)
			.subscribe(() => {
				// remove item from array
				var index = this.serviceCategories.findIndex(sc => sc.id == id);
				this.serviceCategories.splice(index, 1);
			});
	}

	openDialog(action: string, serviceCategory?: ServiceCategory): void {
		this.action = action;

		// create new service category object that can be used in the dialog without affecting the original
		var serviceCategoryCopy = serviceCategory ? { ...serviceCategory } : new ServiceCategory(0, '');

		const dialogRef = this.dialog.open(ManageServiceCategoryDialogComponent, {
			width: '300px',
			data: {
				action: this.action,
				serviceCategory: serviceCategoryCopy
			},
		});

		// when dialog is closed
		dialogRef.afterClosed().subscribe(result => {
			if (result) {
				// send request to backend
				if (this.action == "New") {
					this.create(result);
				}
				else if (this.action == "Edit") {
					this.update(result);
				}
			}
		});
	}
}
