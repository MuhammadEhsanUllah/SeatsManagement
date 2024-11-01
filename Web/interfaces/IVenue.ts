import type { ISeat } from "./ISeat";

export interface IVenue {
    id: number;
    sectionNumber: number;
    rowsCount: number;
    x:number;
    y:number;
    columnsCount: number;
    seats: ISeat[];
    height:number;
    width:number;
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