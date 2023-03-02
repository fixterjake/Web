
export type Event = {
    id: number;
    title: string;
    description: string;
    host: string;
    bannerUrl?: string;
    start: Date;
    end: Date;
    isOpen: boolean;
    created: Date;
    updated: Date;
};

export type EventPosition = {
    id: number;
    eventId: number;
    name: string;
    minRating: number;
    available: boolean;
}

export type EventRegistration = {
    id: number;
    userId?: number;
    eventId: number;
    eventPositionId: number;
    status: number
    start: Date;
    end: Date;
    created: Date;
    updated: Date;
}