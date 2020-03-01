import { Component, OnInit } from '@angular/core';

@Component({
    selector: 'app-cocomo',
    templateUrl: './cocomo.component.html',
    styleUrls: ['./cocomo.component.scss']
})
export class CocomoComponent implements OnInit {


    Part: number = 0;
    Levels: string[] = ['Bardzo niski', 'Niski', 'Normalny', 'Wysoki', 'Bardzo wysoki', 'Extra wysoki'];

    SimpleProject: boolean = true;
    Size = 0;

    SFNames: string[] = ['Typowość', 'Elastyczność', 'Zarządzanie ryzykiem', 'Spójność zespołu', 'Dojrzałość procesu'];
    SFMins: number[] = [0, 0, 0, 0, 0];
    SFMaxs: number[] = [5, 5, 5, 5, 5];
    FSValues: number[][] = [[6.20, 4.96, 3.72, 2.48, 1.24, 0.00],
                            [5.07, 4.05, 3.04, 2.03, 1.01, 0.00],
                            [7.07, 5.65, 4.24, 2.83, 1.41, 0.00],
                            [5.48, 4.38, 3.29, 2.19, 1.10, 0.00],
                            [7.80, 6.24, 4.68, 3.12, 1.56, 0.00]]
    SFi: number[] = [2, 2, 2, 2, 2];

    EMNames: string[] = ['Wymagana niezawodność systemu', 'Rozmiar użytej bazy danych', 'Re - używalność', 'Zakres wymaganej dokumentacji', 'Złożoność','Wymagania czasu wykonania',
        'Ograniczenia pamięciowe', 'Płynność platformy tworzenia', 'Możliwości analityków', 'Możliwości programistów','Doświadczenie z aplikacją', 'Doświadczenie z platformą',
        'Doświadczenie z językiem i narzędziami', 'Ciągłość zatrudnienia personelu', 'Użycie narzędzi programowych', 'Jakość komunikacji zespołu', 'Napięty harmonogram'];
    EMMins: number[] = [0, 1, 0, 1, 0, 2, 2, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0];
    EMMaxs: number[] = [4, 4, 5, 5, 4, 5, 5, 4, 4, 4, 4, 4, 4, 4, 4, 5, 4];
    EMValues: number[][] = [[0.82, 0.92, 1.00, 1.10, 1.26, null],
                            [null, 0.90, 1.00, 1.14, 1.28, null],
                            [0.73, 0.87, 1.00, 1.17, 1.34, 1.74],
                            [null, 0.95, 1.00, 1.07, 1.15, 1.24],
                            [0.81, 0.91, 1.00, 1.11, 1.23, null],
                            [null, null, 1.00, 1.11, 1.29, 1.63],
                            [null, null, 1.00, 1.05, 1.17, 1.46],
                            [null, 0.87, 1.00, 1.15, 1.30, null],
                            [1.42, 1.19, 1.00, 0.85, 0.71, null],
                            [1.34, 1.15, 1.00, 0.88, 0.76, null],
                            [1.29, 1.12, 1.00, 0.90, 0.81, null],
                            [1.22, 1.10, 1.00, 0.88, 0.81, null],
                            [1.19, 1.09, 1.00, 0.91, 0.85, null],
                            [1.20, 1.09, 1.00, 0.91, 0.84, null],
                            [1.17, 1.09, 1.00, 0.90, 0.78, null],
                            [1.22, 1.09, 1.00, 0.93, 0.86, 0.80],
                            [1.43, 1.14, 1.00, 1.00, 1.00, null]]
    EMi: number[] = [2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2];


    constructor() { }

    ngOnInit() {
    }

CalculateValue() {
    let SF = 0;
    let EM = 1;

    if (!isNaN(this.Size)) {
        for (let i = 0; i < this.SFi.length; i++) {
            SF += this.FSValues[i][this.SFi[i]];
        }

        if (!this.SimpleProject) {
            for (let i = 0; i < this.EMi.length; i++) {
                let a = this.EMValues[i][this.EMi[i]];

                EM *= this.EMValues[i][this.EMi[i]];
            }
        }

        return 2.94 * Math.pow(this.Size, (0.91 + 0.01 * SF)) * EM;
    }
    else
        return 0;
}

}
