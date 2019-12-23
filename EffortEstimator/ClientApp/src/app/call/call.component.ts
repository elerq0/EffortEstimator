import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { NgxAgoraService, AgoraClient, Stream, ClientEvent, StreamEvent } from 'ngx-agora';

@Component({
    selector: 'app-call',
    templateUrl: './call.component.html',
    styleUrls: ['./call.component.scss']
})
export class CallComponent implements OnInit {

    localCallId = 'agora_local';
    remoteCalls: string[] = [];
    users: string[] = [];

    private client: AgoraClient;
    private localStream: Stream;
    private uid: string;

    private isMuted: boolean = false;

    constructor(private ngxAgoraService: NgxAgoraService, private router: Router) {
        this.uid = sessionStorage.getItem('EE-email');
        //this.ngxAgoraService.AgoraRTC.Logger.setLogLevel(this.ngxAgoraService.AgoraRTC.Logger.NONE);
    }

    channel: string;

    ngOnInit() {
        if (sessionStorage.getItem('EE-channel-name')) {
            this.channel = sessionStorage.getItem('EE-channel-name');
            sessionStorage.removeItem('EE-channel-name');

            this.client = this.ngxAgoraService.createClient({ mode: 'rtc', codec: 'h264' });

            this.assignClientHandlers();

            this.localStream = this.ngxAgoraService.createStream({ streamID: this.uid, audio: true, video: false, screen: false });
            this.assignLocalStreamHandlers();

            this.initLocalStream();
        } else {
            this.router.navigate(['/home']);
        }
    }

    ngOnDestroy() {
        if (this.localStream != undefined)
            this.localStream.close();
        if (this.client != undefined)
            this.client.leave();
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
            this.router.navigate(['/home']);
        }
    }
}
