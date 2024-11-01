export interface ISeat {
    x: number;
    y: number;
    radius: number;
    price: number;
    id: number;
    row: number;
    isSelected: boolean;
    color: string;
    isDeleted: boolean;
    isReserved:boolean;
}