// store/SaveVenueStore.ts
import { defineStore } from 'pinia';
import { ref } from 'vue';
import { $api } from '~/composables/api';
import toastr from 'toastr';
import 'toastr/build/toastr.min.css';
import type { ISection } from '~/interfaces/ISection';

export const useSaveVenueStore = defineStore('saveVenue', () => {
    const venueName = ref<string>('');
    const selectedSections = ref<ISection[]>([]);
    const { public: { API_BASE_URL } } = useRuntimeConfig();

    const saveVenue = async (): Promise<any> => {
        // Build the payload based on the selected sections with correct x and y positions
        const payload = {
            name: String(venueName.value),
            sections: selectedSections.value.map(section => ({
                sectionId: Number(section.id),
                x: Number(section.x),
                y: Number(section.y)
            }))
        };
        
        console.log("Payload", payload); // For debugging to verify payload structure
    
        try {
            const response = await $api<any>(`${API_BASE_URL}/api/admin/Venue/venue`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify(payload),
            });
            if (!response.status) {
                toastr.error(response.errors[0]);
                return;
            }
            toastr.success('Venue has been successfully saved!', 'Success');
            return response;
        } catch (error) {
            console.error('Error saving venue:', error);
            throw error;
        }
    };
    
    

    return { venueName, selectedSections, saveVenue };
});
