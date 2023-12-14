import { Component, Input, OnInit } from '@angular/core';
import { User } from 'src/app/_models/User';
import { ServicesService } from 'src/app/_services/services.service';

@Component({
	selector: 'app-search-result',
	templateUrl: './search-result.component.html',
	styleUrls: ['./search-result.component.css']
})
export class SearchResultComponent implements OnInit {
	@Input() result: User[];

	constructor(private servicesSerive: ServicesService) { }

	ngOnInit(): void {
	}
}
