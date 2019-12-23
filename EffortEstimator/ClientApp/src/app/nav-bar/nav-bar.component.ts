import { Component, OnInit, Inject } from '@angular/core';
import { Router, NavigationEnd } from '@angular/router';
import { UserManagementService } from '../user-management.service'

@Component({
    selector: 'app-nav-bar',
    templateUrl: './nav-bar.component.html',
    styleUrls: ['./nav-bar.component.scss'],
})
export class NavBarComponent implements OnInit {

    baseUrl: string;
    service: UserManagementService;


    constructor(@Inject('BASE_URL') _baseUrl: string, private router: Router, private _service: UserManagementService ) {
        this.baseUrl = _baseUrl;
        this.service = _service;
    }

    ngOnInit() {}

    logout() {
        sessionStorage.clear();
        this.router.navigate(['']);

    }
}
