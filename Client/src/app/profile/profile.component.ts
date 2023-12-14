import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Profile } from '../_models/Profile';
import { Service } from '../_models/Service';
import { AuthService } from '../_services/auth.service';
import { ServicesService } from '../_services/services.service';
import { UsersService } from '../_services/users.service';

interface Chip {
	name: string;
	selected: boolean;
}

@Component({
	selector: 'app-profile',
	templateUrl: './profile.component.html',
	styleUrls: ['./profile.component.css']
})
export class ProfileComponent implements OnInit {
	profile!: Profile;
	services: Service[];
	chips: Chip[] = [];
	selectedService: Service;
	photos: string[] = [];

	constructor(private servicesService: ServicesService,
		private usersService: UsersService,
		public authService: AuthService,
		private route: ActivatedRoute) { }

	ngOnInit(): void {
		// get id from url
		this.route.paramMap.subscribe(params => {
			var userId = + params.get("id");

			this.getUser(userId);

			this.getServices(userId);
		});
	}

	getUser(id: number) {
		this.usersService.get(id)
			.subscribe(result => {
				this.profile = <Profile>result;
			});
	}

	getServices(id: number) {
		this.servicesService.getAll(id)
			.subscribe((result) => {
				this.services = <Service[]>result;

				// fill in chips
				this.services.forEach((service) =>
					this.chips.push({ name: service.serviceCategory.name, selected: false })
				);

				this.chips[0].selected = true;
				this.selectedService = this.services[0];
				this.selectedService.photos.forEach(photo => this.photos.push(photo.url));
			})
	}

	updateSelectedService(categoryName: string) {
		this.selectedService = this.services.find(s => s.serviceCategory.name == categoryName);

		if (this.selectedService.photos) {
			this.photos = [];
			this.selectedService.photos.forEach(photo => this.photos.push(photo.url));
		}
	}

	selectChip(chip: Chip) {
		var chipState = chip.selected;

		// Unselect all execpt for
		this.chips.forEach(chip => chip.selected = false);

		// reverse select state
		chip.selected = !chipState;

		this.updateSelectedService(chip.name);
	}
}

