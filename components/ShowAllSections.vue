<script lang="ts" setup>
import { ref, onMounted } from 'vue';
import { useGetAllSeatingStore } from '../store/GetAllSectionsStore';

const seatingStore = useGetAllSeatingStore();
const mainCanvas = ref<HTMLCanvasElement | null>(null);
const scaleFactor = 1;

onMounted(async () => {
    
    await seatingStore.getSections();
    if (Array.isArray(seatingStore.sections)) {
        drawCanvas(scaleFactor);
    }
});

const drawCanvas = (scaleFactor: number) => {
    const canvas = mainCanvas.value;
    if (canvas) {
        const ctx = canvas.getContext('2d');
        if (ctx) {
            ctx.clearRect(0, 0, canvas.width, canvas.height);
            ctx.save();
            ctx.scale(scaleFactor, scaleFactor);

            // Check if sections is an array before iterating
            if (Array.isArray(seatingStore.sections)) {
                seatingStore.sections.forEach((section, sectionIndex) => {
                    const sectionProps = {
                        width: 200,
                        height: 100,
                        x: sectionIndex * 220,
                        y: 50,
                    };

                    ctx.fillStyle = 'lightblue';
                    ctx.fillRect(sectionProps.x, sectionProps.y, sectionProps.width, sectionProps.height);
                    ctx.strokeStyle = 'black';
                    ctx.strokeRect(sectionProps.x, sectionProps.y, sectionProps.width, sectionProps.height);

                    section.seats.forEach(seat => {
                        ctx.beginPath();
                        ctx.arc(sectionProps.x + seat.x, sectionProps.y + seat.y, seat.radius, 0, Math.PI * 2);
                        ctx.fillStyle = seat.color;
                        ctx.fill();
                        ctx.closePath();
                    });
                });
            }

            ctx.restore();
        }
    }
};
</script>

<template>
    <div>
        <canvas ref="mainCanvas" class="main-canvas" :width="800" :height="600"></canvas>
        <div v-if="Array.isArray(seatingStore.sections) && seatingStore.sections.length > 0">
            <h2>Sections List</h2>
            <ul>
                <li v-for="section in seatingStore.sections" :key="section.id">
                    <strong>Section ID:</strong> {{ section.id }} | 
                    <strong>Seats Count:</strong> {{ section.seats.length }} | 
                    <strong>Price:</strong> {{ section.seats[0]?.price }}
                </li>
            </ul>
        </div>
        <div v-else>
            <p>No sections available.</p>
        </div>
    </div>
</template>

<style scoped>
.main-canvas {
    border: 2px solid black;
}
</style>
