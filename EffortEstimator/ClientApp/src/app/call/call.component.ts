import { Component, OnInit, Inject } from '@angular/core';
import { Router } from '@angular/router';
import { NgxAgoraService, AgoraClient, Stream, ClientEvent, StreamEvent } from 'ngx-agora';
import { HttpClient, HttpHeaders, HttpEvent, HttpResponse, HttpEventType } from '@angular/common/http';
import { Observable } from 'rxjs';
import { AskForValueComponent } from '../dialogs/ask-for-value/ask-for-value.component';
import { MatDialog } from '@angular/material';
import { InfoComponent } from '../dialogs/info/info.component';
import { EstimationForm } from '../objects/estimation-form'

@Component({
    selector: 'app-call',
    templateUrl: './call.component.html',
    styleUrls: ['./call.component.scss']
})
export class CallComponent implements OnInit {

    private baseUrl: string;

    localCallId = 'agora_local';
    remoteCalls: string[] = [];
    users: string[] = [];

    private client: AgoraClient;
    private localStream: Stream;
    private uid: string;
    private Part: number = 0;

    private isMuted: boolean = false;
    private hasFile: boolean = false;
    private isConference: boolean = true;
    private fileName: string = '';
    private estimationForm: EstimationForm;

    constructor(@Inject('BASE_URL') _baseUrl: string, public dialog: MatDialog, private ngxAgoraService: NgxAgoraService, private router: Router, private httpClient: HttpClient) {
        this.uid = sessionStorage.getItem('EE-email');
        this.baseUrl = _baseUrl;
        //this.ngxAgoraService.AgoraRTC.Logger.setLogLevel(this.ngxAgoraService.AgoraRTC.Logger.NONE);
    }

    channel: string;
    role: string;

    ngOnInit() {
        if (sessionStorage.getItem('EE-channel-name')) {
            this.channel = sessionStorage.getItem('EE-channel-name');
            sessionStorage.removeItem('EE-channel-name');

            this.role = sessionStorage.getItem('EE-group-role');
            sessionStorage.removeItem('EE-group-role');

            this.client = this.ngxAgoraService.createClient({ mode: 'rtc', codec: 'h264' });

            this.assignClientHandlers();

            this.localStream = this.ngxAgoraService.createStream({ streamID: this.uid, audio: true, video: false, screen: false });
            this.assignLocalStreamHandlers();

            this.initLocalStream();

            this.checkIfHasFile();
            this.getEstimationForm();
            
        } else {
            this.router.navigate(['/']);
        }
    }

    ngOnDestroy() {
        if (this.localStream != undefined)
            this.localStream.close();
        if (this.client != undefined)
            this.client.leave();
    }

    checkIfHasFile() {
        let url = this.baseUrl + 'Conference/CheckFile/' + this.channel;

        const headers = new HttpHeaders({
            'Authorization': 'Bearer ' + sessionStorage.getItem('EE-token')
        })

        this.httpClient.get(url, { headers }).subscribe(
            (res: any) => {
                this.hasFile = res.item1;
                this.fileName = res.item2;
            },
            (err) => {
                if (err.status == 401) {
                    sessionStorage.clear();
                    this.router.navigate(['']);
                }
            });
    }

    downloadFile() {
        let url = this.baseUrl + 'Conference/GetFile/' + this.channel;

        const headers = new HttpHeaders({
            'Authorization': 'Bearer ' + sessionStorage.getItem('EE-token')
        })

        this.httpClient.get(url, {
            headers, responseType: 'blob',
            observe: 'response'
        }).subscribe(
            data => {
                const downloadedFile = new Blob([data.body], { type: data.body.type });
                const a = document.createElement('a');
                a.setAttribute('style', 'display:none;');
                document.body.appendChild(a);
                a.download = this.fileName;
                a.href = URL.createObjectURL(downloadedFile);
                a.target = '_blank';
                a.click();
                document.body.removeChild(a);
            }
        );
    }

    async vote() {
        let result: number;
        const dialogRef = this.dialog.open(AskForValueComponent, {
            data: 'Podaj wybraną przez Ciebie wartość pracochłonności dla tego projektu'
        });

        dialogRef.afterClosed().subscribe(
            voteResult => {
                result = voteResult;
            },
            () => {
            },
            async () => {
                if (result != null) {
                    let url = this.baseUrl + 'Conference/Vote'

                    const formData = new FormData();
                    formData.append('chaName', this.channel);
                    formData.append('result', result.toString());

                    const headers = new HttpHeaders({
                        'Authorization': 'Bearer ' + sessionStorage.getItem('EE-token')
                    })

                    this.httpClient.post(url, formData, { headers }).subscribe(
                        () => { },
                        (err) => {
                            if (err.status == 401) {
                                sessionStorage.clear();
                                this.router.navigate(['']);
                            }
                            else
                                this.dialog.open(InfoComponent, {
                                    data: err.error
                                });
                        }
                    );
                }
            });
    }

    getEstimationForm() {
        let url = this.baseUrl + 'Conference/GetEstimationForm/' + this.channel;

        const headers = new HttpHeaders({
            'Authorization': 'Bearer ' + sessionStorage.getItem('EE-token')
        })

        this.httpClient.get(url, { headers }).subscribe(
            (res: EstimationForm) => {
                this.estimationForm = res;
            },
            (err) => {
                if (err.error == 'Not found!') {
                    this.isConference = false;
                }
                else
                this.dialog.open(InfoComponent, {
                    data: err.error
                }); },
            () => {}
        );
    }

    incrementConferenceState() {
        let url = this.baseUrl + 'Conference/Increment'

        const formData = new FormData();
        formData.append('channelName', this.channel);

        const headers = new HttpHeaders({
            'Authorization': 'Bearer ' + sessionStorage.getItem('EE-token')
        })

        this.httpClient.post(url, formData, { headers }).subscribe(
            () => {},
            (err) => {
                if (err.status == 401) {
                    sessionStorage.clear();
                    this.router.navigate(['']);
                }
                else
                    this.dialog.open(InfoComponent, {
                        data: err.error
                    });
            },
            () => {
                this.dialog.open(InfoComponent, {
                    data: 'Rozpoczęto kolejną iterację'
                });
            }
        );
    }

    finishConference() {
        let url = this.baseUrl + 'Conference/Finish'

        const formData = new FormData();
        formData.append('channelName', this.channel);

        const headers = new HttpHeaders({
            'Authorization': 'Bearer ' + sessionStorage.getItem('EE-token')
        })

        this.httpClient.post(url, formData, { headers }).subscribe(
            () => { },
            (err) => {
                if (err.status == 401) {
                    sessionStorage.clear();
                    this.router.navigate(['']);
                }
                else
                    this.dialog.open(InfoComponent, {
                        data: err.error
                    });
            },
            () => {
                this.router.navigate(['/groups'])
            }
        );
    }


    private assignClientHandlers(): void {
        this.client.on(ClientEvent.LocalStreamPublished, evt => { console.log('Publish local stream successfully'); });

        this.client.on(ClientEvent.Error, error => {
            console.log('Got error msg:', error.reason);
            if (error.reason === 'DYNAMIC_KEY_TIMEOUT') {
                this.client.renewChannelKey(
                    '',
                    () => console.log('Renewed the channel key successfully.'),
                    (err) => console.error('Renew channel key failed: ', err)
                );
            }
        });

        this.client.on(ClientEvent.RemoteStreamAdded, evt => {
            const stream = evt.stream as Stream;
            this.client.subscribe(stream, { audio: true }, err => { console.log('Subscribe stream failed', err); });
        });

        this.client.on(ClientEvent.RemoteStreamSubscribed, evt => {
            const stream = evt.stream as Stream;
            const id = `${stream.getId()}`;
            this.users.push(`${stream.getId()}`);
            if (!this.remoteCalls.length) {
                this.remoteCalls.push(id);
                setTimeout(() => stream.play(id), 1000);
            }
        });

        this.client.on(ClientEvent.RemoteStreamRemoved, evt => {
            const stream = evt.stream as Stream;
            if (stream) {
                stream.stop();
                this.remoteCalls = [];
                console.log(`Remote stream is removed ${stream.getId()}`);
            }
        });

        this.client.on(ClientEvent.PeerLeave, evt => {
            const stream = evt.stream as Stream;
            this.users.filter(user => user !== (`${stream.getId()}`));
            if (stream) {
                stream.stop();
                this.remoteCalls = this.remoteCalls.filter(call => call !== `${stream.getId()}`);
                console.log(`${evt.uid} left from this channel`);
            }
        });
    }

    private assignLocalStreamHandlers(): void {
        this.localStream.on(StreamEvent.MediaAccessAllowed, () => {
            console.log('accessAllowed');
        });

        // The user has denied access to the camera and mic.
        this.localStream.on(StreamEvent.MediaAccessDenied, () => {
            console.log('accessDenied');
        });
    }

    private initLocalStream(): void {
        this.localStream.init(
            () => {
                this.localStream.play(this.localCallId);
                this.client.join(null, this.channel, this.uid,
                    () => this.client.publish(this.localStream, (err) => console.log('Publish local stream error: ' + err)),
                    (err) => { console.error('Join channel error ' + err); });
                this.users.push(this.uid);
            },
            err => console.error('getUserMedia failed', err)
        );
    }

    private mute(): void {
        if (!this.isMuted)
            this.localStream.muteAudio();
        else
            this.localStream.unmuteAudio();

        this.isMuted = !this.isMuted;
    }

    private disconnect(): void {
        try {
            this.client.leave();
            this.localStream.close();
        } finally {
            this.router.navigate(['/']);
        }
    }
}
