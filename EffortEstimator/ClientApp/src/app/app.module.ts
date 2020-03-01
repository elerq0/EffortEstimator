import { BrowserModule } from '@angular/platform-browser';
import 'hammerjs'
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { RouterModule } from '@angular/router';
import { MatButtonModule, MatMenuModule, MatFormFieldModule, MatInputModule, MatIconModule, MatTableModule, MatPaginatorModule, MatSliderModule, MatCheckboxModule, MatDialogModule } from '@angular/material';
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
import { CocomoComponent } from './calculators/cocomo/cocomo.component';
import { UsecasepointsComponent } from './calculators/usecasepoints/usecasepoints.component';
import { AskForValueComponent } from './dialogs/ask-for-value/ask-for-value.component';
import { InfoComponent } from './dialogs/info/info.component';

@NgModule({
    declarations: [
        AppComponent,
        NavBarComponent,
        HomeComponent,
        LoginComponent,
        RegisterComponent,
        CallComponent,
        GroupPanelComponent,
        ConferenceCreatorComponent,
        CocomoComponent,
        UsecasepointsComponent,
        AskForValueComponent,
        InfoComponent
    ],
    imports: [
        BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
        HttpClientModule,
        FormsModule,
        RouterModule.forRoot([
            { path: 'login', component: LoginComponent },
            { path: 'register', component: RegisterComponent },
            { path: '', component: HomeComponent },
            { path: 'call', component: CallComponent },
            { path: 'groups', component: GroupPanelComponent },
            { path: 'conference', component: ConferenceCreatorComponent },
            { path: 'cocomo', component: CocomoComponent },
            { path: 'usecasepoints', component: UsecasepointsComponent }
        ], { useHash: true }),
        BrowserAnimationsModule,
        MatButtonModule,
        MatMenuModule,
        MatFormFieldModule,
        MatInputModule,
        MatIconModule,
        MatTableModule,
        MatPaginatorModule,
        MatSliderModule,
        MatCheckboxModule,
        MatDialogModule,
        NgxAgoraModule.forRoot({ AppID: environment.agora.appId }),
    ],
    providers: [],
    entryComponents: [
        AskForValueComponent,
        InfoComponent
    ],
    bootstrap: [AppComponent]
})
export class AppModule { }
