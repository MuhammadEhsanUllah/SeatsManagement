// store/seating.ts
import { defineStore } from 'pinia';
import { ref } from 'vue';
import type { ISeat } from '~/interfaces/ISeat';
import { $api } from '~/composables/api';
import toastr from 'toastr'
import 'toastr/build/toastr.min.css';

export const useSeatingStore = defineStore('seating', () => {
    const sectionNumber = ref<string>('1');
    const rowsCount = ref<number>(0);
    const columnsCount = ref<number>(0);
    const seats = ref<ISeat[]>([]); 
    const { public: { API_BASE_URL } } = useRuntimeConfig();
    const saveSeats = async (): Promise<any> => {
        const payload = {
            rowsCount: Number(rowsCount.value), 
            columnsCount: Number(columnsCount.value), 
            seats: seats.value.map(seat => ({
                x: Number(seat.x),
                y: Number(seat.y), 
                radius: Number(seat.radius),
                price: String(seat.price), 
                color: String(seat.color)
            }))
        };

        console.log("Payload", payload); 
        
        try {
            const response = await $api<any>(`${API_BASE_URL}/api/admin/Section/section`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify(payload),
            });
            toastr.success('Section has been successfully saved!', 'Success');
            return response; 
        } catch (error) {
            console.error('Error saving seats:', error);
            throw error; 
        }
    };
    return { sectionNumber, rowsCount, columnsCount, seats, saveSeats };
});
