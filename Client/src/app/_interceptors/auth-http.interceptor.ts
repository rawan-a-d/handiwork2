import { Injectable } from '@angular/core';
import {
	HttpRequest,
	HttpHandler,
	HttpEvent,
	HttpInterceptor
} from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable()
export class AuthHttpInterceptor implements HttpInterceptor {

	constructor() { }

	intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
		// user token
		const token = localStorage.getItem('token');
		// check if there is a token
		let isLoggedIn = token != null;
		let contentType;
		// request headers
		let headers = request.headers;

		// add authorization header
		if (isLoggedIn) {
			headers = headers.append('Authorization', `Bearer ${token}`);
		}

		if (headers.has('Content-Type')) {
			contentType = headers.get('Content-Type');
		}

		contentType == undefined ?
			(headers = headers.append('Content-Type', 'application/json')) :
			(headers = headers.delete('Content-Type'));

		const clonedRequest = request.clone({ headers });

		return next.handle(clonedRequest);
	}
}
