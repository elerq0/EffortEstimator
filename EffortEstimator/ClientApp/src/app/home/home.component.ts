import { Component, OnInit, Inject } from '@angular/core';
import { Router } from '@angular/router';
import { UserManagementService } from '../user-management.service'
import { MatDialog } from '@angular/material';
import { InfoComponent } from '../dialogs/info/info.component';

@Component({
    selector: 'app-home',
    templateUrl: './home.component.html',
    styleUrls: ['./home.component.scss']
})
export class HomeComponent implements OnInit{

    service: UserManagementService;

    constructor(private router: Router, private _service: UserManagementService, public dialog: MatDialog) {
        this.service = _service;
    }

    ngOnInit() { }

    negotiations() {
        if (this.service.isLoggedIn()) {
            this.router.navigate(['/groups'])
        } else {
            const dialogRef = this.dialog.open(InfoComponent, {
                data: "Aby skorzystać z opcji negocjacji musisz się najpierw zalogować, a następnie utworzyć lub dołączyć do grupy. Negocjacje są prowadzone przy użyciu konferencji głosowych. Zostaniesz teraz przekierowany do okna logowania."
            });

            dialogRef.afterClosed().subscribe(
                () => {
                    this.router.navigate(['/login'])
                })
        }
    }

}
