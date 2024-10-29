// store/seating.ts
import { defineStore } from 'pinia';
import { ref } from 'vue';
import type { ISeat } from '~/interfaces/ISeat';
import { $api } from '~/composables/api';
import toastr from 'toastr'
import 'toastr/build/toastr.min.css';

export const useSeatingStore = defineStore('seating', () => {
    const name = ref<string>("");
    const rowsCount = ref<number>(0);
    const columnsCount = ref<number>(0);
    const seats = ref<ISeat[]>([]);
    const { public: { API_BASE_URL } } = useRuntimeConfig();
    const saveSeats = async (): Promise<any> => {
        const payload = {
            name: String(name.value),
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
    const updateSection = async (sectionId: number): Promise<any> => {
        const payload = {
            id: sectionId, // Add the section ID here
            name: String(name.value),
            seats: seats.value.map(seat => ({
                x: Number(seat.x),
                y: Number(seat.y),
                radius: Number(seat.radius),
                price: String(seat.price),
                color: String(seat.color),
                id: Number(seat.id)
            }))
        };

        console.log("Payload", payload);

        try {
            const response = await $api<any>(`${API_BASE_URL}/api/admin/Section/section`, {
                method: 'PUT',
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


    const deleteSection = async (sectionId: number): Promise<void> => {
        try {
            const response = await $api(`${API_BASE_URL}/api/admin/Section/section`, {
                method: 'DELETE',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify([sectionId]),
            });

            if (response) {
                // Remove the deleted venue from the venues array
                // venues.value = venues.value.filter(venue => venue.id !== venueId);
                toastr.success('Section deleted successfully!', 'Success');
            } else {
                console.error('API response is not structured as expected:', response);
                toastr.error('Failed to delete venue!', 'Error');
            }
        } catch (error) {
            console.error('Error deleting venue:', error);
            toastr.error('Failed to delete venue!', 'Error');
        }
    };
    return { name, rowsCount, columnsCount, seats, saveSeats, deleteSection, updateSection };
});
