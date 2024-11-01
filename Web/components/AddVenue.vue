<script lang="ts" setup>
import { ref, onMounted } from 'vue';
import { useGetAllSeatingStore } from '../store/GetAllSeatingStore';
import { useSaveVenueStore } from '../store/SaveVenueStore';
import type { ISection, ISectionPosition } from '~/interfaces/ISection';
import type { ISeat } from '~/interfaces/ISeat';

const seatingStore = useGetAllSeatingStore();
const saveVenueStore = useSaveVenueStore();
const mainCanvas = ref<HTMLCanvasElement | null>(null);
const isModalOpen = ref(false);
const selectedSections = ref<ISection[]>([]);
const sectionPositions = ref<ISectionPosition[]>([]);
let isDragging = ref(false);
let draggingIndex = ref(-1);
let offset = { x: 0, y: 0 };

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
    initializeCanvasPositions(selectedSections.value); 
    renderCanvas(); 
};

const initializeCanvasPositions = (sectionsToDraw: ISection[]) => {
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
            sectionPositions.value = [];
            
            sectionsToDraw.forEach((section) => {
                const maxX = Math.max(...section.seats.map(seat => seat.x));
                const canvasWidth = maxX + 2 * section.seats[0].radius + 20;
                const sectionHeight = Math.max(...section.seats.map(seat => seat.y)) + 2 * section.seats[0].radius + 20;

                if (currentX + canvasWidth > maxWidth) {
                    currentX = 20;
                    currentY += sectionHeight + gap;
                }

                sectionPositions.value.push({
                    sectionId:section.id,
                    name: section.name,
                    x: currentX,
                    y: currentY,
                    width: canvasWidth,
                    height: sectionHeight,
                });

                currentX += canvasWidth + gap;
            });
        }
    }
};

const renderCanvas = () => {
    const canvas = mainCanvas.value;

    if (canvas) {
        const ctx = canvas.getContext('2d');
        if (ctx) {
            ctx.clearRect(0, 0, canvas.width, canvas.height);

            sectionPositions.value.forEach((canvasProps, index) => {
                const section = selectedSections.value[index];

                ctx.fillStyle = 'lightblue';
                ctx.fillRect(canvasProps.x, canvasProps.y, canvasProps.width, canvasProps.height);
                ctx.strokeStyle = 'black';
                ctx.strokeRect(canvasProps.x, canvasProps.y, canvasProps.width, canvasProps.height);

                section.seats.forEach((seat: ISeat) => {
                    ctx.beginPath();
                    ctx.arc(canvasProps.x + seat.x, canvasProps.y + seat.y, seat.radius, 0, Math.PI * 2);
                    ctx.fillStyle = seat.color;
                    ctx.fill();
                    ctx.closePath();
                });
            });
        }
    }
};

const startDragging = (event: MouseEvent) => {
    const { offsetX, offsetY } = event;
    sectionPositions.value.forEach((canvasProps, index) => {
        if (
            offsetX >= canvasProps.x &&
            offsetX <= canvasProps.x + canvasProps.width &&
            offsetY >= canvasProps.y &&
            offsetY <= canvasProps.y + canvasProps.height
        ) {
            isDragging.value = true;
            draggingIndex.value = index;
            offset.x = offsetX - canvasProps.x;
            offset.y = offsetY - canvasProps.y;
        }
    });
    renderCanvas();
};

const drag = (event: MouseEvent) => {
    if (!isDragging.value || draggingIndex.value === -1) return;

    const { offsetX, offsetY } = event;
    const canvasProps = sectionPositions.value[draggingIndex.value];

    canvasProps.x = offsetX - offset.x;
    canvasProps.y = offsetY - offset.y;

    renderCanvas();
};

const stopDragging = () => {
    isDragging.value = false;
    draggingIndex.value = -1;
};

const saveVenue = async () => {
    saveVenueStore.selectedSections = selectedSections.value.map((section) => {
        const position = sectionPositions.value.find(pos => pos.name === section.name);
        return {
            ...section,
            x: position ? position.x : 0,
            y: position ? position.y : 0
        };
    });
    
    saveVenueStore.venueName = saveVenueStore.venueName || ''; 

    try {
        await saveVenueStore.saveVenue();
        // Reset fields after saving
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
            <canvas ref="mainCanvas" class="main-canvas" width="1000" @mousedown="startDragging"
            @mousemove="drag" @mouseup="stopDragging" @mouseleave="stopDragging"></canvas>
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
