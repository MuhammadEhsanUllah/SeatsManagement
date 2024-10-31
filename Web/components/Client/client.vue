<template>
  <div>
    <div class="color-indicator">
      <span class="color-box bg-success"></span>
      <label class="fw-bold">$100 Price</label>
    </div>
    <div class="color-indicator">
      <span class="color-box bg-primary"></span>
      <label class="fw-bold">$150 Price</label>
    </div>
    <div class="color-indicator">
      <span class="color-box bg-secondary"></span>
      <label class="fw-bold">$200 Price</label>
    </div>
  </div>

  <div class="container-fluid">
    <div class="row">
      <div class="col-9">
        <div v-for="venue in venues" :key="venue.id" class="venue-container" :data-id="venue.id">
          <div class="scrollable-canvas-container">
            <canvas :id="`canvas-${venue.id}`" class="venue-canvas" width="1000" height="400"></canvas>
          </div>
        </div>
      </div>
      <div class="col-3">

      </div>
    </div>
  </div>
</template>

<script lang="ts" setup>
import { ref, onMounted, nextTick } from 'vue';
import { useGetAllVenuesStore } from '../store/GetAllVenuesStore';

import type { IVenueSection } from '~/interfaces/IVenue';
import type { ISeat } from '~/interfaces/ISeat';
const seatingStore = useGetAllVenuesStore();
const venues = ref<IVenueSection[]>([]);

onMounted(async () => {
  await seatingStore.getVenues();
  venues.value = seatingStore.venues;

  await nextTick();
  drawVenues();
});

const drawVenues = () => {
  venues.value.forEach((venue) => {
    const canvas = document.getElementById(`canvas-${venue.id}`) as HTMLCanvasElement;
    if (canvas) {
      const ctx = canvas.getContext('2d');
      if (ctx) {
        ctx.clearRect(0, 0, canvas.width, canvas.height);
        ctx.save();

        // Draw venue name at the top center
        ctx.fillStyle = 'black';
        ctx.font = '24px Arial';
        ctx.fillText(
          venue.name,
          (canvas.width / 2 - ctx.measureText(venue.name).width / 2),
          30
        );

        // Calculate total canvas height based on section positioning and height
        const gap = 20;
        const totalHeight = Math.max(
          ...venue.sections.map((section) => section.y + Math.max(...section.seats.map(seat => seat.y)) + section.seats[0].radius * 2 + gap)
        ) + 50;
        canvas.height = totalHeight;

        // Draw each section
        venue.sections.forEach((section) => {
          // Calculate the section's width and height based on seat positions
          const sectionWidth = Math.max(...section.seats.map(seat => seat.x)) + section.seats[0].radius * 2 + 20;
          const sectionHeight = Math.max(...section.seats.map(seat => seat.y)) + section.seats[0].radius * 2 + 20;

          // Use section.x and section.y for section placement on canvas
          ctx.fillStyle = 'lightblue';
          ctx.fillRect(section.x, section.y, sectionWidth, sectionHeight);
          ctx.strokeStyle = 'black';
          ctx.strokeRect(section.x, section.y, sectionWidth, sectionHeight);

          // Draw each seat within the section
          section.seats.forEach((seat) => {
            ctx.beginPath();
            ctx.arc(
              section.x + seat.x,
              section.y + seat.y,
              seat.radius,
              0,
              Math.PI * 2
            );
            ctx.fillStyle = seat.color;
            ctx.fill();
            ctx.closePath();
          });
        });

        ctx.restore();
      }
    }
  });
};




</script>

<style scoped>
.color-indicator {
  display: flex;
  align-items: center;
}

.color-box {
  width: 20px;
  height: 20px;
  margin-right: 8px;
  border: 1px solid #ccc;
  border-radius: 2px;
}

.venue-container {
  margin: 20px 0;
  border: 2px solid black;
  position: relative;
}

.scrollable-canvas-container {
  max-height: 400px;
  overflow-y: auto;
  overflow-x: hidden;
}
.venue-canvas {
  display: block;
  height: auto;
  width: 100%;
  overflow-y: auto;
}
</style>