import { Component, Inject } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';

@Component({
    selector: 'app-info',
    templateUrl: './info.component.html'
})
export class InfoComponent {

    constructor(@Inject(MAT_DIALOG_DATA) public message: string) { }

}
