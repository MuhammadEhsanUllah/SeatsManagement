import type { ISeat } from "./ISeat";

export interface IVenue {
    id: string;
    sectionNumber: number;
    rowsCount: number;
    columnsCount: number;
    seats: ISeat[];
}

export interface IVenueSection {
    id:string;
    name: string;
    sections: IVenue[];
}

export interface IUpdateVenue {
    id:string;
    name: string;
    sectionIds: number[];
}