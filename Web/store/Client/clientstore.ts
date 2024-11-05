import { defineStore } from 'pinia';
import type { IVenueSection } from "~/interfaces/IVenue";
import toastr from 'toastr';
import 'toastr/build/toastr.min.css';
interface ReserveSeats {
    seatId: number;
    isReserved: boolean;
}

export const useClientStore = defineStore('clientstore', () => {
    const venues = ref<IVenueSection[]>([]);
    const clientId = ref<number>(1);
    const { public: { API_BASE_URL } } = useRuntimeConfig();

    // Fetch all venues
    const getAllVenues = async (): Promise<void> => {
        try {
            const response = await $api<IVenueSection>(`${API_BASE_URL}/api/admin/Venue/venues`, {
                method: 'GET',
                headers: {
                    'Content-Type': 'application/json',
                },
            });
            console.log("Data from API:", response);
            if (response) {
                venues.value = response.data;
            } else {
                console.error('API response is not structured as expected:', response);
            }
        } catch (error) {
            console.error('Error fetching sections:', error);
        }
    };

    const reserveSeats = async (clientId: number, reservedSeats: ReserveSeats[]): Promise<void> => {
        try {
            const response = await $api<any>(`${API_BASE_URL}/api/client/seat/reserve`, {
                method: 'PUT',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify({ clientId, seats: reservedSeats }),
            });
            console.log("Data from API:", response);
            if (!response.status) {
                toastr.error(response.errors[0]);
                return;
            }
            toastr.success("Seats Reserved Successfully", "Success");
        } catch (error) {
            console.error('Error reserving seats:', error);
        }
    };

    // Return all necessary properties and functions
    return { venues, clientId, getAllVenues, reserveSeats };
});
