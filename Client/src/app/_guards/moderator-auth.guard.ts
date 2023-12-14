import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivate, Router, RouterStateSnapshot, UrlTree } from '@angular/router';
import { Observable } from 'rxjs';
import { AuthService } from '../_services/auth.service';

@Injectable({
	providedIn: 'root'
})
export class ModeratorAuthGuard implements CanActivate {

	constructor(
		private router: Router,
		private authService: AuthService) { }

	canActivate(
		route: ActivatedRouteSnapshot,
		state: RouterStateSnapshot): boolean {
		// Check if user is moderator
		let user = this.authService.currentUser;

		let isModerator = user.role.indexOf("Moderator") >= 0;

		if (user && isModerator) {
			return true;
		}
		else {
			// no access page to
			// this.router.navigate(['no-access']);
			return false;
		}
	}

}
