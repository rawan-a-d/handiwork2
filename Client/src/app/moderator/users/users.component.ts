import { Component, OnInit } from '@angular/core';
import { User } from 'src/app/_models/User';
import { UsersService } from 'src/app/_services/users.service';

@Component({
	selector: 'app-users',
	templateUrl: './users.component.html',
	styleUrls: ['./users.component.css']
})
export class UsersComponent implements OnInit {
	users: User[] = [];

	constructor(private usersService: UsersService) { }

	delete(id: number) {
		this.usersService.delete(id)
			.subscribe(() => {
				// remove item from array
				var index = this.users.findIndex(u => u.id == id);
				this.users.splice(index, 1);
			});
	}

	ngOnInit(): void {
		this.usersService.getAll().subscribe(data => {
			this.users = <User[]>data;
		})
	}

	public trackUser(index: number, user: User) {
		return user ? user.id : undefined;
	}
}
