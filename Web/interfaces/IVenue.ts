import type { ISeat } from "./ISeat";

export interface IVenue {
    id: number;
    sectionNumber: number;
    rowsCount: number;
    columnsCount: number;
    seats: ISeat[];
}

export interface IVenueSection {
    id:number;
    name: string;
    sections: IVenue[];
}

export interface IUpdateVenue {
    id:number;
    name: string;
    sectionIds: number[];
}