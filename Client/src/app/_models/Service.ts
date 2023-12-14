import { Photo } from "./Photo";
import { ServiceCategory } from "./ServiceCategory";
import { User } from "./User";

export class Service {
	constructor(public id: number, public info: string, public userId: number, public user: User, public serviceCategoryId: number, public serviceCategory: ServiceCategory, public photos: Photo[]) {
	}
}
