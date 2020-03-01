import { Component, OnInit, Inject, ViewChild, ElementRef } from '@angular/core';
import { Router } from '@angular/router';
import { HttpClient, HttpHeaders } from '@angular/common/http';

@Component({
    selector: 'app-conference-creator',
    templateUrl: './conference-creator.component.html',
    styleUrls: ['./conference-creator.component.scss']
})
export class ConferenceCreatorComponent implements OnInit {

    baseUrl: string;

    groupName: string;
    topic: string = '';
    description: string = '';
    startDate: string;
    @ViewChild('file', { static: false }) fileInput: ElementRef;
    msg: string = '';

    constructor(@Inject('BASE_URL') _baseUrl: string, private router: Router, private httpClient: HttpClient) {
        this.baseUrl = _baseUrl;
    }

    ngOnInit() {
        var date = new Date();
        this.startDate = new Date(date.getTime() - date.getTimezoneOffset() * 60 * 1000).toISOString().slice(0, 16);
        if (sessionStorage.getItem('EE-group-name') != null) {
            this.groupName = sessionStorage.getItem('EE-group-name');
            sessionStorage.removeItem('EE-group-name');

        } else {
            this.router.navigate(['/groups']);
        }
    }

    planNewConference() {
        let url = this.baseUrl + 'Conference/Create'

        const formData = new FormData();
        formData.append('groupName', this.groupName);
        formData.append('topic', this.topic);
        formData.append('description', this.description);
        formData.append('startDate', this.startDate);

        var fileBrowser = this.fileInput.nativeElement;
        if(fileBrowser.files && fileBrowser.files[0])
            formData.append('file', fileBrowser.files[0]);

        const headers = new HttpHeaders({
            'Authorization': 'Bearer ' + sessionStorage.getItem('EE-token')
        })

        this.msg = ''
        this.httpClient.post(url, formData, { headers }).subscribe(
            () => { },
            (err) => {
                if (err.status == 401) {
                    sessionStorage.clear();
                    this.router.navigate(['']);
                }
                else
                    this.msg = err.error;
            },
            () => this.router.navigate(['/groups'])
        );
    }
}
