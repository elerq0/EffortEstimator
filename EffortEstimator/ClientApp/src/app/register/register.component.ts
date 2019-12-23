import { Component, OnInit, Inject } from '@angular/core';
import { Router } from '@angular/router';
import { HttpClient } from '@angular/common/http';

@Component({
    selector: 'app-register',
    templateUrl: './register.component.html',
    styleUrls: ['./register.component.scss']
})
export class RegisterComponent implements OnInit {

    baseUrl: string;

    hidePassword = true;
    hideConfPassword = true;

    email: string;
    password: string;
    confPassword: string;
    name: string;
    surname: string;

    msg: string;

    constructor(@Inject('BASE_URL') _baseUrl: string, private router: Router, private httpClient: HttpClient) {
        this.baseUrl = _baseUrl;
    }

    ngOnInit() {
    }

    private Register() {
        let url = this.baseUrl + 'User/Register'
        if (!this.MatchPassword())
            return;

        const formData = new FormData();
        formData.append('email', this.email);
        formData.append('password', this.password);
        formData.append('name', this.name);
        formData.append('surname', this.surname);

        this.msg = '';
        this.httpClient.post(url, formData).subscribe(
            () => {},
            (err) => {
                this.msg = err.error;
            },
            () => {
                this.router.navigate(['']);
            }
        );
    }

    private MatchPassword(): boolean {
        if (this.password !== this.confPassword)
            return false;

        return true;
    }

}
