export class EstimationForm {
    topic: string;
    description: string;
    iteration: number;
    userResultValue: number;
    minResultValue: number;
    maxResultValue: number;
    avgResultValue: number;
    proposedValue: number;
    resultValues: number[];

    constructor(topic: string, description: string, iteration: number, userResultValue: number, minResultValue: number,
        maxResultValue: number, avgResultValue: number, proposedValue: number, resultValues: number[]) {
        this.topic = topic;
        this.description = description;
        this.iteration = iteration;
        this.userResultValue = userResultValue;
        this.minResultValue = minResultValue;
        this.maxResultValue = maxResultValue;
        this.avgResultValue = avgResultValue;
        this.proposedValue = proposedValue;
        this.resultValues = resultValues;
    }
}
