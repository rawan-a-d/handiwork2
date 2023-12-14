import { Component, Input, OnInit } from '@angular/core';

@Component({
	selector: 'app-image-slider',
	templateUrl: './image-slider.component.html',
	styleUrls: ['./image-slider.component.css']
})
export class ImageSliderComponent implements OnInit {
	@Input() images: string[] = [];

	slideIndex: number = 0;

	constructor() { }

	ngOnInit(): void {
	}

	plusSlides(n: number) {
		this.slideIndex += n;
	}
}
