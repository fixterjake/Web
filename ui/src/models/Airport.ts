export type Airport = {
    id?: string;
    name: string;
    icao: string;
    arrivals: number;
    departures: number;
    wind?: string;
    altimeter?: string;
    temperature?: string;
    metarRaw?: string;
    created?: Date;
    updated?: Date;
}