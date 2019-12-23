import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { RouterModule } from '@angular/router';
import { MatButtonModule, MatMenuModule, MatFormFieldModule, MatInputModule, MatIconModule, MatTableModule, MatPaginatorModule } from '@angular/material';

import { AppComponent } from './app.component';
import { NavBarComponent } from './nav-bar/nav-bar.component';
import { HomeComponent } from './home/home.component';
import { LoginComponent } from './login/login.component';
import { RegisterComponent } from './register/register.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { environment } from 'src/environments/environment';
import { NgxAgoraModule } from 'ngx-agora';
import { CallComponent } from './call/call.component';
import { GroupPanelComponent } from './group-panel/group-panel.component';
import { ConferenceCreatorComponent } from './conference-creator/conference-creator.component';

@NgModule({
    declarations: [
        AppComponent,
        NavBarComponent,
        HomeComponent,
        LoginComponent,
        RegisterComponent,
        CallComponent,
        GroupPanelComponent,
        ConferenceCreatorComponent
    ],
    imports: [
        BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
        HttpClientModule,
        FormsModule,
        RouterModule.forRoot([
            { path: '', component: LoginComponent },
            { path: 'register', component: RegisterComponent },
            { path: 'home', component: HomeComponent },
            { path: 'call', component: CallComponent },
            { path: 'groups', component: GroupPanelComponent },
            { path: 'conference', component: ConferenceCreatorComponent },
        ], { useHash: true }),
        BrowserAnimationsModule,
        MatButtonModule,
        MatMenuModule,
        MatFormFieldModule,
        MatInputModule,
        MatIconModule,
        MatTableModule,
        MatPaginatorModule,
        NgxAgoraModule.forRoot({ AppID: environment.agora.appId }),
    ],
    providers: [],
    bootstrap: [AppComponent]
})
export class AppModule { }
