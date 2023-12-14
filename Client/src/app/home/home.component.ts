import { Component, OnInit } from '@angular/core';
import { User } from '../_models/User';
import { ServicesService } from '../_services/services.service';

@Component({
	selector: 'app-home',
	templateUrl: './home.component.html',
	styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {
	result: User[];

	constructor(private servicesSerive: ServicesService) { }

	ngOnInit(): void {
	}

	search(form) {
		console.log(form.searchText)
		this.servicesSerive.search(0, form.searchText)
			.subscribe((data) => {
				this.result = <User[]>data;
			})
	}
}
