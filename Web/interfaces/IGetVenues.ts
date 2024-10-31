import type {  IVenueSection } from "./IVenue";

export interface IGetVenues {
    message: string;
    status: boolean;
    errors: string | null;
    data: IVenueSection[];
}