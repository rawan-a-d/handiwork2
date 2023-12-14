import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthService } from '../_services/auth.service';
import { PasswordValidators } from '../_validators/password.validators';

@Component({
	selector: 'app-register',
	templateUrl: './register.component.html',
	styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
	hide = true;

	form = new FormGroup(
		{
			name: new FormControl('',
				[
					Validators.required,
					Validators.minLength(5),
				]),
			email: new FormControl('',
				[
					Validators.required,
					Validators.email
				]),
			password: new FormControl('',
				Validators.required
			),
			confirmPassword: new FormControl('',
				Validators.required
			),
		},
		// custom validators
		PasswordValidators.passwordsShouldMatch
	);

	constructor(private router: Router,
		private route: ActivatedRoute,
		private authService: AuthService) { }

	ngOnInit(): void {
	}

	get name() {
		return this.form.get('name');
	}

	get email() {
		return this.form.get('email');
	}

	get password() {
		return this.form.get('password');
	}

	get confirmPassword() {
		return this.form.get('confirmPassword');
	}

	register() {
		this.authService.register(this.form.value)
			.subscribe((result) => {
				if (result) {
					let returnUrl = this.route.snapshot.queryParamMap.get('returnUrl'); // The url the user wanted to go to before he logged in
					this.router.navigate([returnUrl || '/']);
				}
			})
	}

	onStrengthChanged(score: any) {
		console.log('new score', score);
	}
}
