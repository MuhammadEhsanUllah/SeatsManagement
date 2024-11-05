<template>
  <div class="text-white">
    <!-- Color Indicators for Seat Pricing -->
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
    <div class="color-indicator">
      <span class="color-box bg-warning"></span>
      <label class="fw-bold">Reserved</label>
    </div>
  </div>

  <!-- Venue Canvases Layout -->
  <div class="container-fluid">
    <div class="row">
      <div class="col-9">
        <div v-for="venue in venues" :key="venue.id" class="venue-container" :data-id="venue.id">
          <div class="scrollable-canvas-container">
            <!-- Canvas for Each Venue -->
            <canvas ref="venuecanvas" :id="`canvas-${venue.id}`" @click="handleMainCanvasClick($event, venue)"
              class="venue-canvas" width="1000" height="400"></canvas>
          </div>
        </div>
      </div>
      <div class="col-3">
        <div class="text-white">
          <ul>
            <li v-for="(selectedSeat, index) in renderSelectedSeats()" :key="selectedSeat.seat.id" class="mb-2">
              Seat {{ selectedSeat.number }} - Price: ${{ selectedSeat.seat.price }}
              <button @click="removeSeat(selectedSeat.seat)">Remove</button>
            </li>
            <h5 v-if="selectedSeats.length > 0">Total Sum: ${{ totalSum }}</h5>
          </ul>
          <button v-if="selectedSeats.length > 0" @click="sendReservedSeats" class="reserve-button">
            Reserve Seats
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script lang="ts" setup>
import { ref, onMounted, nextTick, computed } from 'vue';
import { useClientStore } from '../../store/Client/clientstore';
import type { IVenueSection } from '~/interfaces/IVenue';
import type { ISeat } from '~/interfaces/ISeat';

const clientstore = useClientStore();
const venues = ref<IVenueSection[]>([]);
const selectedSeats = ref<ISeat[]>([]);
const clientId = ref<number>(1);

onMounted(async () => {
  await clientstore.getAllVenues();
  venues.value = clientstore.venues;
  await nextTick();
  drawVenues();
});
const totalSum = computed(() => {
  return selectedSeats.value.reduce((sum, seat) => sum + Number(seat.price), 0);
});

// Function to handle seat click
const handleSeatClick = (seat: ISeat) => {
  seat.isSelected = !seat.isSelected; // Toggle selection state

  if (seat.isSelected) {
    selectedSeats.value.push(seat); // Push the entire seat object
  } else {
    removeSeat(seat); // Call the remove function
  }

  console.log("Selected Seats", selectedSeats.value); // Log selected seats for debugging
  drawVenues(); // Redraw the canvas to reflect seat selection state
};

// Function to remove a seat
const removeSeat = (seat: ISeat) => {
  
  selectedSeats.value = selectedSeats.value.filter(selectedSeat => selectedSeat.id !== seat.id);

  seat.isSelected = false;
  seat.color = getColorByPrice(seat.price); 
  
  drawVenues();

  console.log("Selected Seats after removal", selectedSeats.value);
};

// Render selected seats with sequential numbering
const renderSelectedSeats = () => {
  return selectedSeats.value.map((seat, index) => ({
    number: index + 1, // Sequential numbering starting from 1
    seat,
  }));
};

const sendReservedSeats = async () => {
  const reservedSeats = selectedSeats.value.map(seat => {
    return {
      seatId: seat.id,
      isReserved: true
    };
  });
  console.log("REserve SEats", reservedSeats);
  try {
    await clientstore.reserveSeats(clientstore.clientId, reservedSeats);
    console.log('Seats reserved successfully:', reservedSeats);

    selectedSeats.value = [];
    await clientstore.getAllVenues();
    venues.value = clientstore.venues;
    drawVenues();
  } catch (error) {
    console.error('Error reserving seats:', error);
  }
};

const drawVenues = () => {
  venues.value.forEach((venue) => {
    const canvas = document.getElementById(`canvas-${venue.id}`) as HTMLCanvasElement;
    if (canvas) {
      const ctx = canvas.getContext('2d');
      if (ctx) {
        ctx.clearRect(0, 0, canvas.width, canvas.height);
        ctx.save();

        // Draw venue name
        ctx.fillStyle = 'black';
        ctx.font = '24px Arial';
        ctx.fillText(
          venue.name,
          (canvas.width / 2 - ctx.measureText(venue.name).width / 2),
          30
        );

        const gap = 20;
        const totalHeight = Math.max(
          ...venue.sections.map((section) => section.y + Math.max(...section.seats.map(seat => seat.y)) + section.seats[0].radius * 2 + gap)
        ) + 50;
        canvas.height = totalHeight;

        // Draw each section and seat
        venue.sections.forEach((section) => {
          const sectionWidth = Math.max(...section.seats.map(seat => seat.x)) + section.seats[0].radius * 2 + 20;
          const sectionHeight = Math.max(...section.seats.map(seat => seat.y)) + section.seats[0].radius * 2 + 20;
          ctx.fillStyle = '#4b5563';
          ctx.fillRect(section.x, section.y, sectionWidth, sectionHeight);
          ctx.strokeStyle = 'white';
          ctx.strokeRect(section.x, section.y, sectionWidth, sectionHeight);

          section.seats.forEach((seat) => {
            ctx.beginPath();
            ctx.arc(
              section.x + seat.x,
              section.y + seat.y,
              seat.radius,
              0,
              Math.PI * 2
            );

            ctx.fillStyle = seat.isReserved ? '#F2C010' : seat.color;
            ctx.fill();
            ctx.closePath();
          });
        });

        ctx.restore();
      }
    }
  });
};

// Helper function to get click position relative to the canvas
const getClickPosition = (event: MouseEvent, canvas: HTMLCanvasElement) => {
  const rect = canvas.getBoundingClientRect();
  const clickX = event.clientX - rect.left;
  const clickY = event.clientY - rect.top;
  return { clickX, clickY };
};

// Main canvas click handler
const handleMainCanvasClick = (event: MouseEvent, venue: IVenueSection) => {
  const canvas = event.currentTarget as HTMLCanvasElement;
  const { clickX, clickY } = getClickPosition(event, canvas);

  // Find the section that was clicked based on bounding box and seat layout
  const clickedSection = venue.sections.find(
    (section) =>
      clickX >= section.x &&
      clickX <= section.x + Math.max(...section.seats.map(seat => seat.x)) + section.seats[0].radius * 2 &&
      clickY >= section.y &&
      clickY <= section.y + Math.max(...section.seats.map(seat => seat.y)) + section.seats[0].radius * 2
  );

  if (clickedSection) {
    const clickedSeatId = getSeatIdFromClick(clickX, clickY, clickedSection);
    if (clickedSeatId !== undefined) {
      const clickedSeat = clickedSection.seats.find(seat => seat.id === clickedSeatId);
      if (clickedSeat) {
        toggleSeatSelection(clickedSeat); // Toggle the selection state of the clicked seat
      }
    }
  }
};

// Handle click within a specific section
const handleSectionClick = (section: IVenueSection['sections'][number], clickX: number, clickY: number) => {
  section.seats.forEach((seat) => {
    const seatX = section.x + seat.x;
    const seatY = section.y + seat.y;
    const distance = Math.sqrt((clickX - seatX) ** 2 + (clickY - seatY) ** 2);

    if (distance <= seat.radius) {
      toggleSeatSelection(seat);
    }
  });
};

// Toggle seat selection and update color
const toggleSeatSelection = (seat: ISeat) => {
  if (seat.isReserved) return; // Prevent selection of reserved seats

  // Toggle selection and update color
  seat.isSelected = !seat.isSelected;
  seat.color = seat.isSelected ? 'red' : getColorByPrice(seat.price);

  // Update selected seats list
  if (seat.isSelected) {
    selectedSeats.value.push(seat);
  } else {
    selectedSeats.value = selectedSeats.value.filter(selectedSeat => selectedSeat.id !== seat.id);
  }

  drawVenues(); // Re-render the venue to reflect changes
};
const getSeatIdFromClick = (clickX: number, clickY: number, section: IVenueSection['sections'][number]): number | undefined => {
  return section.seats.find(seat => {
    // Calculate the absolute seat coordinates within the section
    const seatX = section.x + seat.x;
    const seatY = section.y + seat.y;
    const distance = Math.sqrt((clickX - seatX) ** 2 + (clickY - seatY) ** 2);
    return distance <= seat.radius; // Return the seat if within the click radius
  })?.id;
};
// Define color by price
const getColorByPrice = (price: number): string => {
  switch (price) {
    case 100: return '#008000';
    case 150: return '#0000FF';
    case 200: return '#808080';
    default: return '#008000';
  }
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
