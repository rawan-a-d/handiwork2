import { HttpClient } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { BehaviorSubject, map, Observable } from 'rxjs';
import { environment } from 'src/environments/environment';

@Injectable({
	providedIn: 'root'
})
export class ServicesService {
	info = new BehaviorSubject('information');
	http: HttpClient;
	url: string;

	constructor(@Inject(HttpClient) http: HttpClient) {
		this.http = http;
		this.url = environment.apiUrlServices;
	}

	getInfo(): Observable<string> {
		return this.info.asObservable();
	}

	getInfoValue(): string {
		return this.info.getValue();
	}

	setInfo(val: string) {
		this.info.next(val);
	}

	getAll(userId: number) {
		return this.http.get(this.url + '/' + userId + '/services')
			.pipe(
				map(
					response => response
				)
			)
	}

	get(userId: number, serviceId: number) {
		return this.http.get(this.url + '/' + userId + '/services/' + serviceId)
			.pipe(
				map(
					response => response
				)
			)
	}

	create(userId: number, resource: any) {
		return this.http.post(this.url + '/' + userId + '/services', JSON.stringify(resource))
			.pipe(
				map(
					response => response
				)
			)
	}

	update(userId: number, resource: any) {
		this.setInfo('Object updated');

		return this.http.put(this.url + '/' + userId + '/services/' + resource.id, JSON.stringify(resource))
			.pipe(
				map(
					response => response
				)
			)
	}


	delete(userId: number, serviceId: number) {
		this.setInfo('Object deleted');

		return this.http.delete(this.url + '/' + userId + '/services/' + serviceId)
			.pipe(
				map(
					response => response
				)
			)
	}

	deletePhoto(userId: number, serviceId: number, photoId: number) {
		return this.http.delete(this.url + '/' + userId + '/services/' + serviceId + '/photos/' + photoId);
	}

	search(userId: number, keyword: string) {
		return this.http.get(this.url + '/' + userId + '/services/' + 'search?keyword=' + keyword);
	}
}
