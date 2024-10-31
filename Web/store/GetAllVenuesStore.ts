// store/GetAllSectionsStore.ts
import { defineStore } from 'pinia';
import { ref } from 'vue';
import type { IUpdateSectionPosition } from '~/interfaces/ISection';
import { $api } from '~/composables/api';
import toastr from 'toastr';
import 'toastr/build/toastr.min.css';
import type { IVenueSection,IUpdateVenue } from '~/interfaces/IVenue';

export const useGetAllVenuesStore = defineStore('getallvenues', () => {
    const venues = ref<IVenueSection[]>([]);
    const { public: { API_BASE_URL } } = useRuntimeConfig();

    const getVenues = async (): Promise<void> => {
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
                // toastr.error('Failed to load sections!', 'Error');
            }
        } catch (error) {
            console.error('Error fetching sections:', error);
            // toastr.error('Failed to load sections!', 'Error');
        }
    };

    const updateVenue = async (updatedData: IUpdateVenue): Promise<void> => {
        try {
            const response = await $api<any>(`${API_BASE_URL}/api/admin/Venue/venue`, {
                method: 'PUT',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify(updatedData),
            });
            
            if (response) {
                if (!response.status) {
                    toastr.error(response.errors[0]);
                    return;
                }
                toastr.success('Venue updated successfully!', 'Success');
            } else {
                console.error('API response is not structured as expected:', response);
                toastr.error('Failed to update venue!', 'Error');
            }
        } catch (error) {
            console.error('Error updating venue:', error);
            toastr.error('Failed to update venue!', 'Error');
        }
    };

    const deleteVenue = async (venueId: number): Promise<void> => {
        try {
            const response = await $api(`${API_BASE_URL}/api/admin/Venue/venue`, {
                method: 'DELETE',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify([venueId]),
            });

            if (response) {
                // Remove the deleted venue from the venues array
               // venues.value = venues.value.filter(venue => venue.id !== venueId);
                console.log('Venue deleted successfully:', venueId);
                toastr.success('Venue deleted successfully!', 'Success');
            } else {
                console.error('API response is not structured as expected:', response);
                toastr.error('Failed to delete venue!', 'Error');
            }
        } catch (error) {
            console.error('Error deleting venue:', error);
            toastr.error('Failed to delete venue!', 'Error');
        }
    };
    const updateSectionPosition = async (UpdatedPosition: IUpdateSectionPosition): Promise<void> => {
        try {
            const response = await $api(`${API_BASE_URL}/api/admin/Venue/section/position`, {
                method: 'PUT',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify(UpdatedPosition),
            });

            if (response) {
                
            } else {
                console.error('API response is not structured as expected:', response);
                toastr.error('Failed to delete venue!', 'Error');
            }
        } catch (error) {
            console.error('Error deleting venue:', error);
            toastr.error('Failed to delete venue!', 'Error');
        }
    };

    return { venues, getVenues, updateVenue, deleteVenue,updateSectionPosition };
});
