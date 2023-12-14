import { Component, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { ServiceCreateDto } from 'src/app/_models/dtos/ServiceCreateDto';
import { Service } from 'src/app/_models/Service';
import { AuthService } from 'src/app/_services/auth.service';
import { ServicesService } from 'src/app/_services/services.service';
import { ManageSkillDialogComponent } from '../manage-skill-dialog/manage-skill-dialog.component';

export interface SkillDialogData {
	title: string;
	info: string;
	serviceCategoryId: number;
}

@Component({
	selector: 'app-manage-skills',
	templateUrl: './manage-skills.component.html',
	styleUrls: ['./manage-skills.component.css']
})
export class ManageSkillsComponent implements OnInit {
	services: Service[];
	userId: number;

	constructor(
		public dialog: MatDialog,
		private servicesService: ServicesService,
		private authService: AuthService
	) { }


	ngOnInit(): void {
		this.userId = this.authService.currentUser.Id;

		this.getServices();
	}

	openDialog(title: string, service?: Service): void {
		let manageSkillData;

		if (title == "New service") {
			manageSkillData = {
				title: title,
				serviceCategoryId: 0,
				info: ""
			}
		}
		else if (title == "Edit service") {
			manageSkillData = {
				title: title,
				serviceCategoryId: service.serviceCategoryId,
				info: service.info
			}
		}

		const dialogRef = this.dialog.open(ManageSkillDialogComponent, {
			width: '300px',
			data: manageSkillData,
		});

		dialogRef.afterClosed().subscribe(result => {
			let info = result?.info;
			let serviceCategoryId = result?.serviceCategoryId;

			if (result !== undefined) {

				// send request to backend
				if (title == "New service") {
					let newService = new ServiceCreateDto(info, serviceCategoryId);

					this.createService(newService);
				}
				else if (title == "Edit service") {
					service.info = info;

					this.updateService(service);
				}
			}
		});
	}

	// create service
	createService(service: ServiceCreateDto) {
		this.servicesService.create(this.userId, service)
			.subscribe((newService) => {
				this.services.push(<Service>newService);
			});
	}

	// get services for a user
	getServices() {
		this.servicesService.getAll(this.userId)
			.subscribe(data => {
				this.services = <Service[]>data;
			})
	}

	// update service
	updateService(service: Service) {
		this.servicesService.update(this.userId, service)
			.subscribe(() => {
				// replace item in array
				var index = this.services.findIndex(s => s.id == service.id);
				this.services.splice(index, 1, service);
			});
	}

	// delete service
	deleteService(id: number) {
		this.servicesService.delete(this.userId, id)
			.subscribe(() => {
				// remove item from array
				var index = this.services.findIndex(s => s.id == id);
				this.services.splice(index, 1);
			});
	}

}
