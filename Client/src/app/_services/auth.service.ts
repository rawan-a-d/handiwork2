import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { JwtHelperService } from '@auth0/angular-jwt';
import { map } from 'rxjs';
import { environment } from 'src/environments/environment';
import { AuthResponse } from '../_models/AuthResponse';
import { UserAuth } from '../_models/UserAuth';

@Injectable({
	providedIn: 'root'
})
export class AuthService {
	url: string;

	constructor(private http: HttpClient) {
		this.url = environment.apiUrlAuth;
	}

	register(userAuth: UserAuth) {
		return this.http.post(this.url + '/register', JSON.stringify(userAuth), { responseType: 'json' })
			.pipe(
				map((response: AuthResponse) => {
					if (response) {
						localStorage.setItem('token', response.token);

						return true;
					}

					return false;
				})
			)
	}

	login(credentials: any) {
		return this.http.post(this.url + '/login', JSON.stringify(credentials), { responseType: 'json' })
			.pipe(
				map((response: AuthResponse) => {
					if (response) {
						localStorage.setItem('token', response.token);

						return true;
					}

					return false;
				})
			)
	}

	logout() {
		localStorage.removeItem('token');
	}

	isLoggedIn() {
		let token = localStorage.getItem('token');

		if (token) {
			return true;
		}
		return false;
	}

	get currentUser() {
		let token = localStorage.getItem('token');

		if (!token) {
			return null;
		}

		let jwtHelper = new JwtHelperService();
		let decodedToken = jwtHelper.decodeToken(token);

		return decodedToken;
	}

	get token() {
		let token = localStorage.getItem('token');

		return token;
	}

	get userId() {
		return this.currentUser.Id;
	}
}
