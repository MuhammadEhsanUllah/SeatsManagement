<script lang="ts" setup>
import { ref } from 'vue'
import type { ISeat } from '../interfaces/ISeat'
import { useSeatingStore } from '../store/SeatsStore'
const seatingStore = useSeatingStore();

// Reactive properties
const rows = ref<number>(0); 
const columns = ref<number>(0);
let seats = reactive<ISeat[]>([]); 
const originalSeats: { [key: number]: ISeat[] } = {}; 
const rowPrices: { [key: number]: number } = {}; 
const rowCheckboxes: { [key: number]: boolean } = {}; 
const defaultPrice = ref<number>(100); 
const seatingChart = ref<HTMLCanvasElement | null>(null); 
const selectAll = ref<boolean>(false);
let selectedSeats = ref<ISeat[]>([]);
// Function to generate seats
const generateSeats = () => {
    const spacing = 45;
    seats.length = 0; 

    let secNumber = JSON.parse(localStorage.getItem("SectionsCount") || '0');

    for (let r = 1; r <= rows.value; r++) {
        originalSeats[r] = [];
        rowCheckboxes[r] = false;
        rowPrices[r] = defaultPrice.value;
        for (let c = 0; c < columns.value; c++) {
            const seat: ISeat = {
                x: c * spacing + 30,
                y: r * spacing - 15,
                radius: 20,
                price: defaultPrice.value,
                id: `${secNumber}S${seats.length + 1}`,
                row: r,
                isSelected: false,
                color: getColorByPrice(defaultPrice.value), 
                isDeleted: false
            }
            seats.push(seat);
            originalSeats[r].push(seat);
        }
    }

    renderSeats();
};

const handleSeatClick = (seat: ISeat) => {

    if (!seat.isSelected) {
        seat.isSelected = true;
        seat.color = 'red';
    } else {
        seat.isSelected = false;
        seat.color = getColorByPrice(seat.price);

    }
    // const seatToUpdate = seats.find(x => x.id === seat.id);
    // // Store the selected seat value in localStorage
    // const storedSelectedSeats = JSON.parse(localStorage.getItem('selectedSeats') || '[]');
    // if (seat.isSelected) {
    //     storedSelectedSeats.push(seat);
    //     if (seatToUpdate) {
    //         seatToUpdate.isSelected = true; // Update the isSelected value
    //     }

    // } else {
    //     const index = storedSelectedSeats.findIndex((s: ISeat) => s.id === seat.id);
    //     if (index > -1) storedSelectedSeats.splice(index, 1);
    //     if (seatToUpdate) {
    //         seatToUpdate.isSelected = false; // Update the isSelected value
    //     }
    // }
    renderSeats();
};
const deleteSeat = () => {

    seats = seats.filter(seat => !seat.isSelected);

    selectedSeats.value = [];

    renderSeats();

};
const registerClickHandler = () => {
    const canvas = seatingChart.value;
    if (canvas) {
        canvas.addEventListener('click', (event) => {
            const rect = canvas.getBoundingClientRect();
            const x = event.clientX - rect.left;
            const y = event.clientY - rect.top;

            seats.forEach((seat) => {
                const dist = Math.sqrt((x - seat.x) ** 2 + (y - seat.y) ** 2);
                if (dist < seat.radius) {
                    handleSeatClick(seat);
                }
            });
        });
    }
};

const renderSeats = () => {
    const canvas = seatingChart.value;
    if (canvas) {
        const ctx = canvas.getContext('2d');
        if (ctx) {
            ctx.clearRect(0, 0, canvas.width, canvas.height);
            seats.filter(seat => !seat.isDeleted).forEach((seat) => {
                ctx.beginPath();
                ctx.arc(seat.x, seat.y, seat.radius, 0, Math.PI * 2);
                ctx.fillStyle = seat.isSelected ? seat.color : seat.color; 
                ctx.fill();
                ctx.closePath();
            });
        }
    }
};

const getColorByPrice = (price: number): string => {
    switch (price) {
        case 100: return '#008000';
        case 150: return '	#0000FF';
        case 200: return '#808080';
        default: return '#008000';
    }
};

const toggleAllSeats = () => {
    const newState = selectAll.value;
    for (const row in rowCheckboxes) {
        rowCheckboxes[row] = newState;
    }
    seats.forEach((seat) => {
        seat.isSelected = selectAll.value;
        if (selectAll.value) {
            seat.color = "red";

        } else {
            seat.color = getColorByPrice(seat.price);
        }
    });
    //   updateSelectedSeats();
    renderSeats();
};

const clearSeats = () => {
    seats.length = 0;
    for (const row in rowCheckboxes) {
        rowCheckboxes[row] = false;
    }

    selectAll.value = false;
    renderSeats();
    //   selectedSeats.value.length = 0;
};

const saveSelection = async () => {

    if (rows.value === undefined || columns.value === undefined || seats === undefined) {
        console.error('Rows, columns, or seats are not defined');
        return;
    }

    seatingStore.rowsCount = rows.value;
    seatingStore.columnsCount = columns.value;
    seatingStore.seats = seats;

    try {
        const result = await seatingStore.saveSeats();
        console.log('Seats saved successfully:', result);
        clearSeats();
        rows.value = 0;
        columns.value = 0;
    } catch (error) {
        console.error('Failed to save seats:', error);
    }
};

onMounted(() => {
    registerClickHandler();
})
</script>

<template>
    <div class="w-100 mt-2">
        <button type="button" class="btn btn-primary float-end" data-bs-toggle="modal" data-bs-target="#exampleModal">
            Add Section
        </button>
    </div>

    <div class="modal fade" id="exampleModal" tabindex="-1" aria-labelledby="exampleModalLabel" aria-hidden="true">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <h1 class="modal-title fs-5" id="exampleModalLabel">Generate Seats</h1>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                </div>
                <div class="modal-body">
                    <form @submit.prevent="generateSeats">
                        <div class="mb-3">
                            <label for="rows" class="form-label">Number of Rows:</label>
                            <input type="number" id="rows" v-model.number="rows" class="form-control" min="1" required />
                        </div>
                        <div class="mb-3">
                            <label for="columns" class="form-label">Number of Columns:</label>
                            <input type="number" id="columns" v-model.number="columns" class="form-control" min="1"
                                required />
                        </div>
                        <div class="mb-3">
                            <label for="defaultPrice" class="form-label">Default Seat Price:</label>
                            <select id="defaultPrice" v-model.number="defaultPrice" class="form-control">
                                <option value="100">100$</option>
                                <option value="150">150$</option>
                                <option value="200">200$</option>
                            </select>
                        </div>
                        <button type="submit" data-bs-dismiss="modal" class="btn btn-primary">Generate Seats</button>
                    </form>
                </div>
            </div>
        </div>
    </div>

    <canvas ref="seatingChart" width="800" height="500" class="border border-dark"></canvas>

    <div class="mt-3">
        <input type="checkbox" class="form-check-input me-2" v-model="selectAll" @change="toggleAllSeats" />
        <label class="form-label">Select All Seats</label>
    </div>
    <div class="mt-3">
        <button type="button" @click="saveSelection" class="btn btn-success">Save Selection</button>
        <button type="button" @click="clearSeats" class="btn btn-warning ms-4">Clear All Seats</button>
        <button type="button" @click="deleteSeat" class="btn btn-danger ms-4">Delete Selected Seats</button>
    </div>
</template>
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                      