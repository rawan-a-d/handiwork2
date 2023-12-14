export class AuthResponse {
	constructor(public token: string, public success: boolean, public errors: []) {
	}
}
