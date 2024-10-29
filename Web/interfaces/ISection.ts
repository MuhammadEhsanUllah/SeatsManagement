
import type { ISeat } from './ISeat';

export interface ISection {
    id: number; 
    name:string;       
    rowsCount: number; 
    columnsCount: number;  
    seats: ISeat[];          
}
