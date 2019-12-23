import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class UserManagementService {

    constructor() {}

    public isLoggedIn() {
        if (sessionStorage.getItem('EE-email') && sessionStorage.getItem('EE-email').length > 1)
            return true;
        else
            return false;
    }
}
