<script lang="ts" setup>
import { ref, onMounted } from 'vue';
import { useGetAllSeatingStore } from '../store/GetAllSeatingStore';
import { useSaveVenueStore } from '../store/SaveVenueStore';
import type { ISection } from '~/interfaces/ISection';
import type { ISeat } from '~/interfaces/ISeat';

const seatingStore = useGetAllSeatingStore();
const saveVenueStore = useSaveVenueStore();
const mainCanvas = ref<HTMLCanvasElement | null>(null);
const isModalOpen = ref(false);
const selectedSections = ref<ISection[]>([]);

onMounted(async () => {
    await seatingStore.getSections();
});

const openModal = () => {
    isModalOpen.value = true;
};

const closeModal = () => {
    isModalOpen.value = false;
};

const drawSelectedSections = () => {
    closeModal();
    drawCanvas(selectedSections.value);
};

const drawCanvas = (sectionsToDraw: ISection[]) => {
    const canvas = mainCanvas.value;
    if (canvas) {
        const ctx = canvas.getContext('2d');
        if (ctx) {
            
            const gap = 20;
            const maxWidth = 800;
            let currentY = 50;
            let currentX = 20;
            let totalHeight = currentY;

            sectionsToDraw.forEach((section) => {
                const maxX = Math.max(...section.seats.map(seat => seat.x));
                const canvasWidth = maxX + 2 * section.seats[0].radius + 20;
                const sectionHeight = Math.max(...section.seats.map(seat => seat.y)) + 2 * section.seats[0].radius + 20;

                if (currentX + canvasWidth > maxWidth) {
                    currentX = 20;
                    currentY += sectionHeight + gap;
                }

                currentX += canvasWidth + gap;
                totalHeight = Math.max(totalHeight, currentY + sectionHeight + gap);
            });

            
            canvas.height = totalHeight;
            
            currentY = 50;
            currentX = 20;

            ctx.clearRect(0, 0, canvas.width, canvas.height);

            sectionsToDraw.forEach((section) => {
                const maxX = Math.max(...section.seats.map(seat => seat.x));
                const canvasWidth = maxX + 2 * section.seats[0].radius + 20;
                const sectionHeight = Math.max(...section.seats.map(seat => seat.y)) + 2 * section.seats[0].radius + 20;

                if (currentX + canvasWidth > maxWidth) {
                    currentX = 20;
                    currentY += sectionHeight + gap;
                }

                // Draw the section container.
                ctx.fillStyle = 'lightblue';
                ctx.fillRect(currentX, currentY, canvasWidth, sectionHeight);
                ctx.strokeStyle = 'black';
                ctx.strokeRect(currentX, currentY, canvasWidth, sectionHeight);

                // Draw each seat in the section.
                section.seats.forEach((seat: ISeat) => {
                    ctx.beginPath();
                    ctx.arc(currentX + seat.x, currentY + seat.y, seat.radius, 0, Math.PI * 2);
                    ctx.fillStyle = seat.color;
                    ctx.fill();
                    ctx.closePath();
                });

                currentX += canvasWidth + gap;
            });
        }
    }
};


const saveVenue = async () => {
    saveVenueStore.selectedSections = selectedSections.value;
    console.log("fakhiooer", saveVenueStore.selectedSections);
    try {
        await saveVenueStore.saveVenue();
        selectedSections.value = [];
        saveVenueStore.venueName = '';
        clearCanvas();
    } catch (error) {
        console.error('Failed to save venue:', error);

        alert('Failed to save venue. Please try again.');
    }
};
const clearCanvas = () => {
    const canvas = mainCanvas.value;
    if (canvas) {
        const ctx = canvas.getContext('2d');
        if (ctx) {
            ctx.clearRect(0, 0, canvas.width, canvas.height);
        }
    }
};
</script>

<template>
    <div>
        <button class="btn btn-secondary my-3" @click="openModal">Select Sections</button>

        <div v-if="isModalOpen" class="modal">
            <div class="modal-content">
                <span class="close" @click="closeModal">&times;</span>
                <h3>Select Sections</h3>
                <div v-for="section in seatingStore.sections" :key="section.id">
                    <input type="checkbox" :value="section" class="form-check-input me-2" v-model="selectedSections" />
                    <label class="form-label">{{ section.name }}</label>
                </div>
                <button @click="drawSelectedSections" class="btn btn-primary">OK</button>
            </div>
        </div>

        <div class="mb-3">
            <label class="form-label">Venue Name</label>
            <input type="text" v-model="saveVenueStore.venueName" placeholder="Enter Venue Name" class="form-control w-25"
                required />
        </div>

        <div class="canvas-container">
            <canvas ref="mainCanvas" class="main-canvas" width="1000"></canvas>
        </div>
        <div class="mt-3">
            <button @click="saveVenue" class="btn btn-success">Save Venue</button>
        </div>
    </div>
</template>

<style scoped>
.canvas-container {
    overflow-y: auto;
    max-height: 400px;
    border: 2px solid black;
}

.main-canvas {
    display: block;
    width: 100%;
    height: auto;
}

.modal {
    display: block;
    position: fixed;
    z-index: 1;
    left: 0;
    top: 0;
    width: 100%;
    height: 100%;
    overflow: auto;
    background-color: rgba(0, 0, 0, 0.4);
}

.modal-content {
    background-color: #fefefe;
    margin: 5% auto;
    padding: 20px;
    border: 1px solid #888;
    width: 40%;
    overflow-y: auto;
}

.close {
    color: #aaa;
    float: right;
    font-size: 28px;
    font-weight: bold;
}

.close:hover,
.close:focus {
    color: black;
    text-decoration: none;
    cursor: pointer;
}
</style>
