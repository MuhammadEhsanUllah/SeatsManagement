
import type { ISeat } from './ISeat';

export interface ISection {
    id: number; 
    name:string;       
    rowsCount: number; 
    columnsCount: number;  
    x:number;
    y:number;
    width:number;
    height:number;
    seats: ISeat[];          
}

export interface ISectionPosition {
    sectionId:number;
    name:string;        
    x:number;
    y:number;
    width:number;
    height:number;        
}
export interface IUpdateSectionPosition {
    sectionId:number;
    venueId:number;        
    x:number;
    y:number;        
}