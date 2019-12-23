export class ConferenceEntity {
    conferenceId: number;
    groupName: string;
    topic: string;
    description: string;
    startDate: string;

    constructor(conferenceId: number, groupName: string, topic: string, description: string, startDate: string) {
        this.conferenceId = conferenceId;
        this.groupName = groupName;
        this.topic = topic;
        this.description = description;
        this.startDate = startDate;
    }
}
