// store/GetAllSectionsStore.ts
import { defineStore } from 'pinia';
import { ref } from 'vue';
import type { ISection } from '~/interfaces/ISection';
import { $api } from '~/composables/api';
import toastr from 'toastr';
import 'toastr/build/toastr.min.css';

export const useGetAllSeatingStore = defineStore('seating', () => {
    const sections = ref<ISection[]>([]);
    const { public: { API_BASE_URL } } = useRuntimeConfig();

    interface ApiResponse {
        message: string;
        status: boolean;
        errors: any;
        data: ISection[]; 
    }

    const getSections = async (): Promise<void> => {
        try {
            const response = await $api<ApiResponse>(`${API_BASE_URL}/api/Section/sections`, {
                method: 'GET',
                headers: {
                    'Content-Type': 'application/json',
                },
            });

            if (response && Array.isArray(response.data)) {
                sections.value = response.data; 
                // toastr.success('Sections loaded successfully!', 'Success');
            } else {
                console.error('API response is not structured as expected:', response);
                toastr.error('Failed to load sections!', 'Error');
            }
        } catch (error) {
            console.error('Error fetching sections:', error);
            toastr.error('Failed to load sections!', 'Error');
        }
    };

    return { sections, getSections };
});
