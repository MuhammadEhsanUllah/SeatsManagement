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
        const payload = {
            name: String(venueName.value), 
            sectionIds: selectedSections.value.map(section => Number(section.id)), 
        };

        console.log("Payload", payload);

        try {
            const response = await $api<any>(`${API_BASE_URL}/api/Venue/venue`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify(payload),
            });
            toastr.success('Venue has been successfully saved!', 'Success');
            return response;
        } catch (error) {
            console.error('Error saving venue:', error);
            throw error;
        }
    };

    return { venueName, selectedSections, saveVenue };
});
