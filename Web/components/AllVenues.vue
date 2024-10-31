<template>
  <div>
    <div class="">
      <button @click="zoom(true)" class="btn btn-secondary m-3 btn-lg ">Zoom In</button>
      <button class="btn btn-secondary m-3 btn-lg " @click="zoom(false)">Zoom Out</button>
    </div>
    <div v-for="venue in venues" :key="venue.id" class="venue-container" :data-id="venue.id">
      <div class="scrollable-canvas-container">
        <!-- <canvas :id="`canvas-${venue.id}`" class="venue-canvas" width="1000" height="400"></canvas> -->
        <canvas :id="`canvas-${venue.id}`" class="venue-canvas" width="1000" height="400"
          @mousedown="(e) => startDragging(e, venue.id)" @mousemove="drag" @mouseup="stopDragging"
          @mouseleave="stopDragging" />


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
import type { ISection, ISectionPosition, IUpdateSectionPosition } from '~/interfaces/ISection';

const seatingStore = useGetAllVenuesStore();
const sectionStore = useGetAllSeatingStore();
const venues = ref<IVenueSection[]>([]);
const allSections = ref<ISection[]>([]);
const showModal = ref(false);
const showConfirmationModal = ref(false);
const editableVenue = ref<IVenueSection>({ id: 0, name: '', sections: [] });
const selectedSections = ref<number[]>([]);
const venueToDelete = ref<number | null>(null);
const sectionPositions = ref<ISectionPosition[]>([]);
let zoomer = 1;
//drag
const isDragging = ref(false);
let draggingIndex = ref(-1);
let dragVenueId = ref<number | null>(0);
let offset = { x: 0, y: 0 };

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
  // Re-calculate positions to include newly checked sections
  await drawSectionsOnCanvas(selectedSections.value);
  console.log("After draw venue");

  const updatedVenue = {
    ...editableVenue.value,
    sectionIds: allSections.value
      .filter(section => selectedSections.value.includes(section.id))
      .map(section => section.id),

    // Send updated section positions to the backend
    sections: sectionPositions.value
      .filter(position => selectedSections.value.includes(position.sectionId))
      .map(position => ({
        sectionId: position.sectionId,
        x: position.x,
        y: position.y,
        width: position.width,
        height: position.height
      }))
  };

  console.log("Updated Venue", updatedVenue);

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

const drawVenues = async (scaleFactor = 1) => {
  console.log("Start draw venue");
  // Clear `sectionPositions` before drawing to avoid duplicates
  sectionPositions.value = [];

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

        // Loop over each section
        venue.sections.forEach((section) => {
          const maxX = Math.max(...section.seats.map(seat => seat.x));
          const maxY = Math.max(...section.seats.map(seat => seat.y));
          const sectionWidth = (maxX + 2 * (section.seats[0]?.radius || 10) + 20) * scaleFactor;
          const sectionHeight = (maxY + 2 * (section.seats[0]?.radius || 10) + 20) * scaleFactor;
          const posX = section.x * scaleFactor;
          const posY = section.y * scaleFactor;

          ctx.fillStyle = 'lightblue';
          ctx.fillRect(posX, posY, sectionWidth, sectionHeight);
          ctx.strokeStyle = 'black';
          ctx.strokeRect(posX, posY, sectionWidth, sectionHeight);

          // Avoid duplicates by checking for existing section ID
          if (!sectionPositions.value.some(pos => pos.sectionId === section.id)) {
            sectionPositions.value.push({
              sectionId: section.id,
              name: section.name,
              x: posX / scaleFactor,
              y: posY / scaleFactor,
              width: sectionWidth / scaleFactor,
              height: sectionHeight / scaleFactor
            });
          }

          section.seats.forEach((seat) => {
            ctx.beginPath();
            ctx.arc(
              posX + seat.x * scaleFactor,
              posY + seat.y * scaleFactor,
              seat.radius * scaleFactor,
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
  console.log("End draw venue");
};

const drawSectionsOnCanvas = async (selectedSectionIds: number[]) => {
  console.log("Drawing sections...");
  const sectionsToDraw = allSections.value.filter(section => selectedSectionIds.includes(section.id));

  initializeCanvasPositions(sectionsToDraw);
};

const initializeCanvasPositions = (sectionsToDraw: ISection[], canvasWidth = 1000) => {
  const offsetX = 20; // horizontal spacing between sections
  const offsetY = 50; // vertical spacing between rows
  let currentX = offsetX;
  let currentY = offsetY;

  sectionsToDraw.forEach((section) => {
    // Calculate dynamic width and height based on seat positions, similar to `drawVenues`
    const maxX = Math.max(...section.seats.map(seat => seat.x));
    const maxY = Math.max(...section.seats.map(seat => seat.y));
    const sectionWidth = maxX + 2 * (section.seats[0]?.radius || 10) + 20; // Dynamic width
    const sectionHeight = maxY + 2 * (section.seats[0]?.radius || 10) + 20; // Dynamic height

    // Check if adding this section would exceed the canvas width
    if (currentX + sectionWidth > canvasWidth) {
      // Move to the next row
      currentX = offsetX;
      currentY += sectionHeight + offsetY;
    }

    sectionPositions.value.push({
      sectionId: section.id,
      name: section.name,
      x: currentX,
      y: currentY,
      width: sectionWidth,
      height: sectionHeight,
    });

    currentX += sectionWidth + offsetX;
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

const startDragging = (event: MouseEvent, venueId: number) => {
  const { offsetX, offsetY } = event;
  const positions = sectionPositions.value;

  positions.forEach((canvasProps, index) => {
    if (
      offsetX >= canvasProps.x &&
      offsetX <= canvasProps.x + canvasProps.width &&
      offsetY >= canvasProps.y &&
      offsetY <= canvasProps.y + canvasProps.height
    ) {
      isDragging.value = true;
      draggingIndex.value = index;
      dragVenueId.value = venueId;
      offset.x = offsetX - canvasProps.x;
      offset.y = offsetY - canvasProps.y;
    }
  });
};

const drag = async (event: MouseEvent) => {
  if (!isDragging.value || draggingIndex.value === -1 || dragVenueId.value === null) return;

  const { offsetX, offsetY } = event;
  const positions = sectionPositions.value;
  const canvasProps = positions[draggingIndex.value];

  canvasProps.x = offsetX - offset.x; 
  canvasProps.y = offsetY - offset.y; 
  console.log(positions);

  const updatePayload: IUpdateSectionPosition = {
    sectionId: canvasProps.sectionId,
    venueId: dragVenueId.value, 
    x: canvasProps.x,
    y: canvasProps.y,
  };

  await updateSectionPosition(updatePayload); 
  console.log(canvasProps.sectionId, dragVenueId.value);

  renderCanvas(dragVenueId.value); 
};

const updateSectionPosition = async (updateData: IUpdateSectionPosition) => {
  try {
    await seatingStore.updateSectionPosition(updateData); // Pass the whole object
    console.log(`Section ${updateData.sectionId} updated to new position:`, { x: updateData.x, y: updateData.y });
  } catch (error) {
    console.error('Error updating section position:', error);
    toastr.error('Failed to update section position!', 'Error');
  }
};

const stopDragging = () => {
  isDragging.value = false;
  draggingIndex.value = -1;
  dragVenueId.value = null;
};

const renderCanvas = (venueId: number) => {
  const canvas = document.getElementById(`canvas-${venueId}`) as HTMLCanvasElement;
  if (canvas) {
    const ctx = canvas.getContext('2d');
    if (ctx) {
      // Clear the canvas before rendering
      ctx.clearRect(0, 0, canvas.width, canvas.height);

      // Loop through each section position to render sections and seats
      sectionPositions.value.forEach((canvasProps) => {
        // Render section rectangle
        ctx.fillStyle = 'lightblue';
        ctx.fillRect(canvasProps.x, canvasProps.y, canvasProps.width, canvasProps.height);
        ctx.strokeStyle = 'black';
        ctx.strokeRect(canvasProps.x, canvasProps.y, canvasProps.width, canvasProps.height);

        // Find the corresponding section data by ID
        const section = allSections.value.find(sec => sec.id === canvasProps.sectionId);
        if (section) {
          section.seats.forEach((seat) => {
            // Render seat as a circle within the section boundaries
            ctx.beginPath();
            ctx.arc(
              canvasProps.x + seat.x,    // Calculate seat position based on section offset
              canvasProps.y + seat.y,
              seat.radius,
              0,
              Math.PI * 2
            );
            ctx.fillStyle = seat.color;
            ctx.fill();
            ctx.closePath();
          });
        }
      });
    }
  }
};


// Add these event listeners to the canvas
// const canvasMouseEvents = {
//   mousedown: (e: MouseEvent) => startDragging(e, venue.id),
//   mousemove: drag,
//   mouseup: stopDragging,
//   mouseleave: stopDragging,
// };

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
