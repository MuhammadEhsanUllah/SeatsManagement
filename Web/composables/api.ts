// composables/api.ts
export const $api = async <T>(url: string, options: RequestInit): Promise<T> => {
    const response = await fetch(url, options);
    
    if (!response.ok) {
        const errorBody = await response.json();
        throw new Error(errorBody.message || 'Error occurred while fetching data');
    }

    return await response.json() as T;
};
