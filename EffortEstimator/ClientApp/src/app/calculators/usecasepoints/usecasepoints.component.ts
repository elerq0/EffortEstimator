import { Component, OnInit } from '@angular/core';

@Component({
    selector: 'app-usecasepoints',
    templateUrl: './usecasepoints.component.html',
    styleUrls: ['./usecasepoints.component.scss']
})
export class UsecasepointsComponent implements OnInit {

    Part: number = 0;

    Names: string[] = ['Prosty', 'Średnio złożony', 'Złożony'];
    
    UCWeights: number[] = [5, 10, 15];
    UCs: number[] = [0, 0, 0];

    ActorWeights: number[] = [1, 2, 3];
    Actors: number[] = [0, 0, 0];

    ECFNames: string[] = ['Zaznajomienie z projektem ', 'Doświadczenie zespołu', 'Znajomość technik obiektowych', 'Umiejętności głównego analityka', 'Motywacja zespołu',
        'Stabilność wymagań', 'Udział niepełnoetatowców', 'Trudność języka programowania'];
    ECFWeights: number[] = [1.5, 0.5, 1.0, 0.5, 1.0, 2.0, -1.0, -1.0];
    ECFs: number[] = [0, 0, 0, 0, 0, 0, 0, 0];

    TCFNames: string[] = ['Rozproszenie systemu', 'Wydajność systemu', 'Wydajność dla użytkownika końcowego', 'Złożone przetwarzanie wewnętrzne',
        'Re - używalność', 'Łatwość instalacji', 'Łatwość użycia', 'Przenośność ', 'Łatwość wprowadzania zmian', 'Współbieżność',
        'Specjalne zabezpieczenia', 'Udostępnianie użytkownikom zewnętrznym', 'Dodatkowe szkolenia użytkowników']
    TCFWeights: number[] = [2.0, 1.0, 1.0, 1.0, 1.0, 0.5, 0.5, 2.0, 1.0, 1.0, 1.0, 1.0, 1.0,]
    TCFs: number[] = [0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0]

    constructor() { }

    ngOnInit() {
    }

    CalculateValue(): number {
        let UUCW = 0;
        let UAW = 0;
        let ECF = 0;
        let ECFSums = 0;
        let TCF = 0;
        let TCFSums = 0;

        for (let i = 0; i < this.UCs.length; i++) {
            UUCW += (!isNaN(this.UCs[i]) ? this.UCs[i] : 0) * this.UCWeights[i];
        }

        for (let i = 0; i < this.Actors.length; i++) {
            UAW += (!isNaN(this.Actors[i]) ? this.Actors[i] : 0) * this.ActorWeights[i];
        }

        for (let i = 0; i < this.ECFs.length; i++) {
            ECFSums += this.ECFs[i]  * this.ECFWeights[i];
        }
        ECF = 1.4 + (-0.03 * ECFSums);

        for (let i = 0; i < this.TCFs.length; i++) {
            TCFSums += this.TCFs[i] * this.TCFWeights[i];
        }
        TCF = 0.6 + (0.01 * TCFSums);

        return (UAW + UUCW) * TCF * ECF * 20;
    }

}
