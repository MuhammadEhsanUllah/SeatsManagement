import { defineStore } from 'pinia';
import { ref } from 'vue';
import type { ISeat } from '~/interfaces/ISeat';
import { $api } from '~/composables/api';
import toastr from 'toastr';
import 'toastr/build/toastr.min.css';

export const useSeatingStore = defineStore('seating', () => {
    const sectionNumber = ref<string>('1');
    const rowsCount = ref<number>(0);
    const columnsCount = ref<number>(0);
    const seats = ref<ISeat[]>([]);
    const canvasData = ref<{ width: number; height: number; x: number; y: number }>({
        width: 0,
        height: 0,
        x: 0,
        y: 0
    });

    // Retrieve API base URL from runtime config
    const { public: { API_BASE_URL } } = useRuntimeConfig();

    // Save section data
    const saveSection = async (): Promise<any> => {
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
        console.log("Payload", payload); // Log payload for debugging

        try {
            const response = await $api<any>(`${API_BASE_URL}/api/Section/section`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify(payload),
            });
            toastr.success('Section data saved successfully!', 'Success');
            return response; 
        } catch (error) {
            console.error('Error saving section:', error);
            throw error; 
        }
    };

    // Save canvas data
    const saveCanvas = async (newCanvas: { width: number; height: number; x: number; y: number }): Promise<any> => {
        const payload = {
            width: newCanvas.width,
            height: newCanvas.height,
            x: newCanvas.x,
            y: newCanvas.y
        };

        try {
            const response = await $api<any>(`${API_BASE_URL}/api/Canvas/canvas`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify(payload),
            });
            toastr.success('Canvas data saved successfully!', 'Success');
            return response; 
        } catch (error) {
            console.error('Error saving canvas:', error);
            throw error; 
        }
    };

    // Save both section and canvas data with updated positions
    const saveSeats = async (newCanvas: { width: number; height: number; x: number; y: number }): Promise<any> => {
        try {
            // Fetch existing sections to calculate positions
            const existingSections = await fetchExistingSections();
            let canvasX = 50; // Default starting position for X
            let canvasY = 50; // Default starting position for Y

            if (existingSections.length > 0) {
                existingSections.forEach((section:any) => {
                    canvasX += (section.columnsCount * 50) + 50; // Adjust X based on existing sections
                });
            }

            // Update the canvas data with calculated positions
            const updatedCanvas = {
                ...newCanvas,
                x: canvasX, // Updated X position
                y: canvasY  // Default Y position
            };

            await saveSection(); // Save section data first
            await saveCanvas(updatedCanvas);  // Then save canvas data with updated positions
        } catch (error) {
            console.error('Error saving seats:', error);
            toastr.error('Failed to save seats and canvas.', 'Error');
            throw error; 
        }
    };

    const fetchExistingSections = async () => {
        try {
            const response = await $api<any>(`${API_BASE_URL}/api/Section/sections`, {
                method: 'GET',
                headers: {
                    'Content-Type': 'application/json',
                },
            });
            return response.data; // Adjust based on your API response structure
        } catch (error) {
            console.error('Error fetching existing sections:', error);
            return [];
        }
    };

    return { sectionNumber, rowsCount, columnsCount, seats, canvasData, saveSeats };
});
