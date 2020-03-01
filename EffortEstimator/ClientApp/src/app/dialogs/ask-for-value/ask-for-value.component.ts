import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material';

@Component({
    selector: 'app-ask-for-value',
    templateUrl: './ask-for-value.component.html'
})
export class AskForValueComponent {

    value: number;

    constructor(
        public dialogRef: MatDialogRef<AskForValueComponent>,
        @Inject(MAT_DIALOG_DATA) public message: string) { }

    onNoClick(): void {
        this.dialogRef.close();
    }

    onYesClick(value: string): void {
        this.dialogRef.close(value);
    }
}
