<template>
  <div>
    <div class="">
      <button @click="zoom(true)" class="btn btn-secondary m-3 btn-lg ">Zoom In</button>
      <button class="btn btn-secondary m-3 btn-lg " @click="zoom(false)">Zoom Out</button>
    </div>
    <div v-for="venue in venues" :key="venue.id" class="venue-container" :data-id="venue.id">
      <div class="scrollable-canvas-container">
        <canvas :id="`canvas-${venue.id}`" class="venue-canvas" width="1000" height="400"
        ></canvas>
      </div>
      <div class="actions">
        <button @click="openEditModal(venue)">Edit</button>
        <button @click="openConfirmationModal(venue.id)">Delete</button>
      </div>
    </div>

    <!-- Edit Modal -->
    <div v-if="showModal" class="modal-overlay" @click.self="closeModal">
      <div class="modal-content">
        <h3>Edit Venue</h3>
        <label>
          Venue Name:
          <input class="form-control w-100" v-model="editableVenue.name" type="text" />
        </label>

        <h4>Sections</h4>
        <div v-for="section in allSections" :key="section.id">
          <input type="checkbox" class="form-check-input" :value="section.id" v-model="selectedSections" />
          <label class="form-label ms-2">
            {{ section.name }}
          </label>
        </div>

        <div class="modal-actions">
          <button @click="UpdateVenue" class="btn btn-success">Save</button>
          <button @click="closeModal" class="btn btn-secondary">Cancel</button>
        </div>
      </div>
    </div>

    <!-- Confirmation Modal -->
    <div v-if="showConfirmationModal" class="modal-overlay" @click.self="closeConfirmationModal">
      <div class="modal-content">
        <h3>Confirm Deletion</h3>
        <p>Are you sure you want to delete this venue?</p>
        <div class="modal-actions">
          <button @click="deleteVenue(venueToDelete)" class="btn btn-danger">Delete</button>
          <button @click="closeConfirmationModal" class="btn btn-secondary">Cancel</button>
        </div>
      </div>
    </div>
  </div>
</template>

<script lang="ts" setup>
import { ref, onMounted, nextTick } from 'vue';
import { useGetAllVenuesStore } from '../store/GetAllVenuesStore';
import { useGetAllSeatingStore } from '../store/GetAllSeatingStore';
import type { IVenueSection } from '~/interfaces/IVenue';
import type { ISection } from '~/interfaces/ISection';
import type { ISeat } from '~/interfaces/ISeat';

const seatingStore = useGetAllVenuesStore();
const sectionStore = useGetAllSeatingStore();
const venues = ref<IVenueSection[]>([]);
const allSections = ref<ISection[]>([]);
const showModal = ref(false);
const showConfirmationModal = ref(false);
const editableVenue = ref<IVenueSection>({ id: 0, name: '', sections: [] });
const selectedSections = ref<number[]>([]);
const venueToDelete = ref<number | null>(null);
let zoomer = 1;

onMounted(async () => {
  await seatingStore.getVenues();
  venues.value = seatingStore.venues;

  await sectionStore.getSections();
  allSections.value = sectionStore.sections;

  await nextTick();
  drawVenues();
});

const openEditModal = (venue: IVenueSection) => {
  editableVenue.value = { ...venue };
  selectedSections.value = venue.sections.map(section => section.id);
  showModal.value = true;
};

const closeModal = () => {
  showModal.value = false;
};

const openConfirmationModal = (venueId: number) => {
  venueToDelete.value = venueId;
  showConfirmationModal.value = true;
};

const closeConfirmationModal = () => {
  showConfirmationModal.value = false;
  venueToDelete.value = null;
};

const UpdateVenue = async () => {
  const updatedVenue = {
    ...editableVenue.value,
    sectionIds: allSections.value
      .filter(section => selectedSections.value.includes(section.id))
      .map(section => section.id),
  };

  try {
    await seatingStore.updateVenue(updatedVenue);

    await seatingStore.getVenues();
    venues.value = seatingStore.venues;
    showModal.value = false;
    drawVenues();
  } catch (error) {
    console.error('Error saving changes:', error);
    toastr.error('Failed to save changes!', 'Error');
  }
};

const deleteVenue = async (venueId: number) => {
  try {
    await seatingStore.deleteVenue(venueId);

    venues.value = venues.value.filter(venue => venue.id !== venueId);

    showConfirmationModal.value = false;
    venueToDelete.value = null;

    drawVenues();
  } catch (error) {
    console.error('Error deleting venue:', error);
  }
};

const drawVenues = (scaleFactor: number = 1) => {
  venues.value.forEach((venue) => {
    const canvas = document.getElementById(`canvas-${venue.id}`) as HTMLCanvasElement;
    if (canvas) {
      const ctx = canvas.getContext('2d');
      if (ctx) {
        ctx.clearRect(0, 0, canvas.width, canvas.height);

        ctx.save();
        ctx.scale(scaleFactor, scaleFactor);

        ctx.fillStyle = 'black';
        ctx.font = '24px Arial';
        ctx.fillText(
          venue.name,
          (canvas.width / 2 - ctx.measureText(venue.name).width / 2) / scaleFactor,
          30 / scaleFactor
        );

        const gap = 20 / scaleFactor;
        let currentY = 50 / scaleFactor;
        let currentX = 20 / scaleFactor;
        const maxWidth = 800 / scaleFactor;
        let totalHeight = currentY;

        venue.sections.forEach((section) => {
          const maxX = Math.max(...section.seats.map(seat => seat.x));
          const canvasWidth = (maxX + 2 * section.seats[0].radius + 20) * scaleFactor;
          const sectionHeight = (Math.max(...section.seats.map(seat => seat.y)) + 2 * section.seats[0].radius + 20) * scaleFactor;

          if (currentX + canvasWidth > maxWidth) {
            currentX = 20 / scaleFactor;
            currentY += sectionHeight + gap;
          }

          totalHeight = Math.max(totalHeight, currentY + sectionHeight + gap);

          currentX += canvasWidth + gap;
        });

        canvas.height = totalHeight * scaleFactor;

        currentY = 50 / scaleFactor;
        currentX = 20 / scaleFactor;

        ctx.clearRect(0, 0, canvas.width, canvas.height);

        venue.sections.forEach((section) => {
          const maxX = Math.max(...section.seats.map(seat => seat.x));
          const canvasWidth = (maxX + 2 * section.seats[0].radius + 20) * scaleFactor;
          const sectionHeight = (Math.max(...section.seats.map(seat => seat.y)) + 2 * section.seats[0].radius + 20) * scaleFactor;

          if (currentX + canvasWidth > maxWidth) {
            currentX = 20 / scaleFactor;
            currentY += sectionHeight + gap;
          }

          ctx.fillStyle = 'lightblue';
          ctx.fillRect(currentX, currentY, canvasWidth, sectionHeight);
          ctx.strokeStyle = 'black';
          ctx.strokeRect(currentX, currentY, canvasWidth, sectionHeight);

          section.seats.forEach((seat: ISeat) => {
            ctx.beginPath();
            ctx.arc(currentX + seat.x * scaleFactor, currentY + seat.y * scaleFactor, seat.radius * scaleFactor, 0, Math.PI * 2);
            ctx.fillStyle = seat.color;
            ctx.fill();
            ctx.closePath();
          });

          currentX += canvasWidth + gap;
        });

        // Restore the default scaling
        ctx.restore();
      }
    }
  });
};


const zoom = (val: boolean) => {
      if (val == true) {
        if (parseFloat(zoomer.toFixed(1)) == 1.4) return;
        zoomer += 0.2;
      } else if (val == false) {

        if (parseFloat(zoomer.toFixed(1)) === 0.6) return;
        zoomer -= 0.2;
      }
      drawVenues(zoomer);
    }

</script>

<style scoped>
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

.actions {
  display: flex;
  gap: 10px;
  margin-top: 10px;
}

.venue-canvas {
  display: block;
  height: auto;
  width: 100%;
  overflow-y: auto;
}

.modal-overlay {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background-color: rgba(0, 0, 0, 0.5);
  display: flex;
  align-items: center;
  justify-content: center;
}

.modal-content {
  background: white;
  padding: 20px;
  width: 400px;
  border-radius: 8px;
}

.modal-actions {
  display: flex;
  gap: 10px;
  justify-content: flex-end;
}

h4 {
  margin-top: 15px;
}
</style>
