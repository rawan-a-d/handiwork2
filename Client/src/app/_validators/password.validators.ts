import { AbstractControl } from '@angular/forms'

export class PasswordValidators {
	// Pass entire Form
	static passwordsShouldMatch(control: AbstractControl) {
		let password = control.get('password');
		let confirmPassword = control.get('confirmPassword');

		if (password?.value !== confirmPassword?.value) {
			return {
				passwordsShouldMatch: true
			}
		}

		return null;
	}
}
