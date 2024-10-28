// interfaces/ISection.ts
import type { ISeat } from './ISeat';

export interface ISection {
    id: string;        
    rowsCount: number; 
    columnsCount: number;  
    seats: ISeat[];          
}
