// interfaces/ISection.ts
import type { ISeat } from './ISeat';

export interface ISection {
    id: string;              // Name or title of the section
    rowsCount: number;       // Number of rows in the section
    columnsCount: number;    // Number of columns in the section
    seats: ISeat[];          // Array of seats in the section
}
