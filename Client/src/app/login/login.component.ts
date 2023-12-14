import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthService } from '../_services/auth.service';

@Component({
	selector: 'app-login',
	templateUrl: './login.component.html',
	styleUrls: ['./login.component.css'],
})
export class LoginComponent implements OnInit {
	hide = true;
	invalidLogin: boolean = false;

	constructor(private router: Router,
		private route: ActivatedRoute,
		private authService: AuthService) { }

	ngOnInit(): void { }

	login(credentials: {}) {
		this.authService.login(credentials)
			.subscribe({
				// receive values from an observable
				next: (result) => {
					console.log(result);
					let returnUrl = this.route.snapshot.queryParamMap.get('returnUrl');
					this.router.navigate([returnUrl || '/']);
				},
				// handle any errors
				error: () => {
					this.invalidLogin = true;
				},
				// perform any tasks when the observable completes
				complete: () => {
				}
			});
	}
}
