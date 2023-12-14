import { Component, OnInit } from '@angular/core';
import { AuthService } from 'src/app/_services/auth.service';
import { environment } from 'src/environments/environment';
import { Photo } from '../../_models/Photo';
import { Service } from '../../_models/Service';
import { User } from '../../_models/User';
import { ServicesService } from '../../_services/services.service';
import { FileUploader } from 'ng2-file-upload';
import { ActivatedRoute } from '@angular/router';

@Component({
	selector: 'app-photo-editor',
	templateUrl: './photo-editor.component.html',
	styleUrls: ['./photo-editor.component.css']
})
export class PhotoEditorComponent implements OnInit {
	service: Service;
	url: string;
	currentUserId: number;
	token: string;

	// photo uploader
	uploader: FileUploader;
	hasBaseDropzoneOver = false;

	constructor(
		private servicesSerive: ServicesService,
		private authService: AuthService,
		private route: ActivatedRoute
	) {
	}

	ngOnInit(): void {
		this.currentUserId = this.authService.userId;

		// get service id from url
		this.route.paramMap.subscribe(params => {
			var serviceId = + params.get("skillId");

			// get service
			this.getService(this.currentUserId, serviceId);

			this.url = environment.apiUrlServices + '/' + this.currentUserId + '/services/' + serviceId + '/photos';
		})

		// get user token
		this.token = this.authService.token;

		// Initialize image uploaded
		this.initializeUploader();
	}

	// Uploaded configuration
	initializeUploader() {
		this.uploader = new FileUploader({
			url: this.url, // end point
			authToken: 'Bearer ' + this.token, // JSON token, this doesn't go through the interceptor
			isHTML5: true,
			allowedFileType: ['image'],
			removeAfterUpload: true, // empty file upload
			autoUpload: false, // use button to confirm
			maxFileSize: 10 * 1024 * 1024,
		});

		this.uploader.onAfterAddingFile = (file) => {
			file.withCredentials = false;
		};

		// if photo was uploaded successfully
		this.uploader.onSuccessItem = (item, response, status, headers) => {
			if (response) {
				const photo: Photo = JSON.parse(response);
				this.service.photos.push(photo);
			}
		};
	}

	fileOverBase(e: any) {
		this.hasBaseDropzoneOver = e;
	}

	// Get service
	getService(userId: number, serviceId: number) {
		this.servicesSerive.get(userId, serviceId)
			.subscribe((data) => {
				this.service = <Service>data;
			});
	}

	// Delete photo
	deletePhoto(photo: Photo) {
		this.servicesSerive.deletePhoto(this.currentUserId, this.service.id, photo.id)
			.subscribe(() =>
				this.service.photos = this.service.photos.filter(
					(x) => x.id !== photo.id
				)
			);
	}
}
