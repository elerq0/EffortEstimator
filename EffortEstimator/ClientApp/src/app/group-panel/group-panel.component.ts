import { Component, OnInit, Inject, ViewChild } from '@angular/core';
import { Router } from '@angular/router';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { GroupEntity } from '../objects/group-entity';
import { ConferenceEntity } from '../objects/conference-entity';
import { MatTableDataSource, MatPaginator } from '@angular/material';

@Component({
    selector: 'app-group-panel',
    templateUrl: './group-panel.component.html',
    styleUrls: ['./group-panel.component.scss']
})
export class GroupPanelComponent implements OnInit {

    @ViewChild(MatPaginator, { static: false }) paginator: MatPaginator;

    baseUrl: string;
    displayedColumns: string[] = ['startDate', 'topic', 'description', 'join'];
    dataSource: MatTableDataSource<ConferenceEntity>;

    groups: GroupEntity[];
    conferences: ConferenceEntity[];
    actualGroup: GroupEntity;

    input: string;
    output: string;

    msg: string;

    constructor(@Inject('BASE_URL') _baseUrl: string, private router: Router, private httpClient: HttpClient) {
        this.baseUrl = _baseUrl;

        this.getGroups();
    }

    ngOnInit() {}


    getGroups(): void {

        this.groups = [];
        this.groups.push(new GroupEntity('', 'api'));
        this.actualGroup = this.groups.find(x => x.name == '' && x.role == 'api');

        this.conferences = [];
        this.conferences.push(new ConferenceEntity(0, '', '', '', ''));

        let url = this.baseUrl + 'Groups'

        const headers = new HttpHeaders({
            'Authorization': 'Bearer ' + sessionStorage.getItem('EE-token')
        })

        this.httpClient.get(url, { headers }).subscribe(
            (res: any[]) => {
                this.groups.push(new GroupEntity('Nowa', 'api'));
                res.forEach(group => {
                    this.groups.push(new GroupEntity(group.Name, group.Role));
                })
            },
            (err) => {
                if (err.status == 401) {
                    sessionStorage.clear();
                    this.router.navigate(['']);
                }
                else
                    this.msg = err.error;
            });
    }

    chooseGroup(group: GroupEntity) {
        this.actualGroup = group;
        this.input = '';
        this.output = '';
        this.msg = '';

        if (group.role != 'api')
            this.getConferences();
    }

    createNewGroup() {
        let url = this.baseUrl + 'Group/Create'

        const formData = new FormData();
        formData.append('groupName', this.input);

        const headers = new HttpHeaders({
            'Authorization': 'Bearer ' + sessionStorage.getItem('EE-token')
        })

        this.httpClient.post(url, formData, { headers }).subscribe(
            (res: string) => {
                this.msg = ''
                this.output = 'Your group was successfully created.'
                this.input = '';
            },
            (err) => {
                if (err.status == 401) {
                    sessionStorage.clear();
                    this.router.navigate(['']);
                }
                else
                    this.msg = err.error;
            },
            () => this.getGroups());
    }

    getJoiningKey() {
        let url = this.baseUrl + 'Group/CreateJoiningKey'

        const formData = new FormData();
        formData.append('groupName', this.actualGroup.name);

        const headers = new HttpHeaders({
            'Authorization': 'Bearer ' + sessionStorage.getItem('EE-token')
        })

        this.httpClient.post(url, formData, { headers }).subscribe(
            (res: string) => {
                this.msg = ''
                this.output = res;
            },
            (err) => {
                if (err.status == 401) {
                    sessionStorage.clear();
                    this.router.navigate(['']);
                }
                else
                    this.msg = err.error;
            });
    }

    joinGroup() {
        let url = this.baseUrl + 'Group/Join'

        const formData = new FormData();
        formData.append('joiningKey', this.input);

        const headers = new HttpHeaders({
            'Authorization': 'Bearer ' + sessionStorage.getItem('EE-token')
        })

        this.httpClient.post(url, formData, { headers }).subscribe(
            (res: string) => {
                this.output = 'You have successfully joined ' + res;
            },
            (err) => {
                if (err.status == 401) {
                    sessionStorage.clear();
                    this.router.navigate(['']);
                }
                else
                    this.msg = err.error;
            },
            () => this.getGroups());
    }

    joinChannel() {
        let url = this.baseUrl + 'Channel/' + this.actualGroup.name;

        const headers = new HttpHeaders({
            'Authorization': 'Bearer ' + sessionStorage.getItem('EE-token')
        })

        this.httpClient.get(url, { headers }).subscribe(
            (res: string) => {
                sessionStorage.setItem('EE-channel-name', res);
            },
            (err) => {
                if (err.status == 401) {
                    sessionStorage.clear();
                    this.router.navigate(['']);
                }
                else
                    this.msg = err.error;
            },
            () => this.router.navigate(['/call'])
        )
    }

    joinConference(conferenceId: number) {
        let url = this.baseUrl + 'Conference/' + this.actualGroup.name + '/' + conferenceId;

        const headers = new HttpHeaders({
            'Authorization': 'Bearer ' + sessionStorage.getItem('EE-token')
        })

        this.httpClient.get(url, { headers }).subscribe(
            (res: string) => {
                sessionStorage.setItem('EE-channel-name', res);
                sessionStorage.setItem('EE-group-role', this.actualGroup.role);
            },
            (err) => {
                if (err.status == 401) {
                    sessionStorage.clear();
                    this.router.navigate(['']);
                }
                else
                    this.msg = err.error;
            },
            () => this.router.navigate(['/call'])
        )
    }

    createConference() {
        sessionStorage.setItem('EE-group-name', this.actualGroup.name);
        this.router.navigate(['/conference']);
    }

    getConferences() {
        let url = this.baseUrl + 'Conferences/' + this.actualGroup.name;

        const headers = new HttpHeaders({
            'Authorization': 'Bearer ' + sessionStorage.getItem('EE-token')
        })

        this.httpClient.get(url, { headers }).subscribe(
            (res: any[]) => {
                this.conferences = [];
                res.forEach(conference => {
                    this.conferences.push(new ConferenceEntity(conference.ConferenceId, conference.GroupName, conference.Topic, conference.Description, conference.StartDate));
                })
            },
            (err) => {
                if (err.status == 401) {
                    sessionStorage.clear();
                    this.router.navigate(['']);
                }
                else
                    this.msg = err.error;
            },
            () => {
                this.dataSource = new MatTableDataSource<ConferenceEntity>(this.conferences);
                this.dataSource.paginator = this.paginator;
            }
        )
    }

    getConferenceAccess(startDate) {
        if (new Date(startDate) <= new Date(new Date().getTime() + 20 * 60000))
            return true;
        else
            return false;
    }
}
