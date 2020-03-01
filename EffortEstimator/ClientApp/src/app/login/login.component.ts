import { Component, OnInit, Inject } from '@angular/core';
import { Router } from '@angular/router';
import { HttpClient, HttpParams } from '@angular/common/http';

@Component({
    selector: 'app-login',
    templateUrl: './login.component.html',
    styleUrls: ['./login.component.scss']
})
export class LoginComponent implements OnInit {

    baseUrl: string;

    needAccountActivation = false;
    hidePassword = true;

    email: string = '';
    password: string = '';
    activationKey: string;

    msg: string;

    constructor(@Inject('BASE_URL') _baseUrl: string, private router: Router, private httpClient: HttpClient) {
        this.baseUrl = _baseUrl;
    }

    ngOnInit() {
    }

    Login() {
        let url = this.baseUrl + 'User/Login'

        const formData = new FormData();
        formData.append('email', this.email);
        formData.append('password', this.password);

        this.httpClient.post(url, formData).subscribe(
            (res: string) => {
                sessionStorage.setItem('EE-token', res);
                sessionStorage.setItem('EE-email', this.email);
                this.msg = ''
            },
            (err) => {
                if (err.error == 'User is not activated!') {
                    this.needAccountActivation = true;
                    this.msg = ''
                }
                else
                    this.msg = err.error;
            },
            () => {
                this.router.navigate(['/']);
            }
        );
    }

    ActivateAccount() {
        let url = this.baseUrl + 'User/Activate'

        const formData = new FormData();
        formData.append('email', this.email);
        formData.append('activationKey', this.activationKey);

        this.httpClient.post(url, formData).subscribe(
            (res: string) => {
                this.msg = '';
                this.needAccountActivation = false;
            },
            (err) => {
                this.msg = err.error;
            });
    }
}
