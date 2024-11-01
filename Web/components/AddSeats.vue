<script lang="ts" setup>
import { ref, reactive, onMounted } from 'vue';
import type { ISeat } from '../interfaces/ISeat';
import { useSeatingStore } from '../store/SeatsStore';
import { useGetAllSeatingStore } from '~/store/GetAllSeatingStore';
import type { ISection } from '../interfaces/ISection'

const seatingStore = useSeatingStore();
const getallseats = useGetAllSeatingStore();

const name = ref<string>("");
const rows = ref<number>(0);
const columns = ref<number>(0);
let seats = reactive<ISeat[]>([]);
const originalSeats: { [key: number]: ISeat[] } = {};
const rowPrices: { [key: number]: number } = {};
const rowCheckboxes: { [key: number]: boolean } = {};
const defaultPrice = ref<number>(100);
const seatingChart = ref<HTMLCanvasElement | null>(null);
const selectAll = ref<boolean>(false);
let selectedSeats = ref<number[]>([]);
const showConfirmationModal = ref(false);
const sectionToDelete = ref<number | null>(null);
const showEditForm = ref(false);
const sectionId = ref<number>(0);
const selectedPrice = ref<number>(0);
let selectedSeatsArray = ref<number[]>([]);

const generateSeats = (sectionNumber: number) => {
    const spacing = 45;
    seats.length = 0;

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
                id: parseInt(`${sectionNumber}S${seats.length + 1}`),
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
    seat.isSelected = !seat.isSelected;
    seat.color = seat.isSelected ? 'red' : getColorByPrice(seat.price);
    if (seat.isSelected) {
        selectedSeats.value.push(seat.id);
        selectedSeatsArray.value.push(seat.id);

    } else {

        selectedSeats.value = selectedSeats.value.filter(id => id !== seat.id);
        selectedSeatsArray.value = selectedSeatsArray.value.filter(id => id !== seat.id);
    }
    console.log("Selected Seats", selectedSeats.value);
    renderSeats();
};

const deleteSeat = () => {
    seats = seats.filter(seat => !seat.isSelected);
    selectedSeats.value = [];
    selectedSeatsArray.value = [];
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
        case 150: return '#0000FF';
        case 200: return '#808080';
        default: return '#008000';
    }
};

const toggleAllSeats = () => {
    const newState = selectAll.value;
    seats.forEach((seat) => {
        seat.isSelected = newState;
        seat.color = newState ? "red" : getColorByPrice(seat.price);
    });
    renderSeats();
};

const clearSeats = () => {
    seats.length = 0;
    Object.keys(rowCheckboxes).forEach(key => rowCheckboxes[Number(key)] = false);
    selectAll.value = false;
    renderSeats();
};

const saveSelection = async () => {
    if (rows.value === undefined || columns.value === undefined || seats === undefined || columns.value == 0 || rows.value == 0) {
        console.error('Rows, columns, or seats are not defined');
        return;
    }

    seatingStore.name = name.value;
    seatingStore.rowsCount = rows.value;
    seatingStore.columnsCount = columns.value;
    seatingStore.seats = seats;

    try {
        const result = await seatingStore.saveSeats();
        console.log(result);
        console.log('Seats saved successfully:', result);
        clearSeats();
        getallseats.getSections();
        ClearForm();
    } catch (error) {
        console.error('Failed to save seats:', error);
    }
};

onMounted(() => {
    registerClickHandler();
    getallseats.getSections();
});

const openConfirmationModal = (sectionId: number) => {
    sectionToDelete.value = sectionId;
    showConfirmationModal.value = true;
};

const closeConfirmationModal = () => {
    showConfirmationModal.value = false;
    sectionToDelete.value = null;
};

const deleteSection = async (sectionId: number) => {
    try {
        await seatingStore.deleteSection(sectionId);

        showConfirmationModal.value = false;
        sectionToDelete.value = null;

        getallseats.getSections();
    } catch (error) {
        console.error('Error deleting section:', error);
    }
};
const RestoreSectionSeats = async (sectionId: number) => {
    try {
        await seatingStore.RestoreSectionSeats(sectionId);

        renderSeats();
        getallseats.getSections();
    } catch (error) {
        console.error('Error deleting section:', error);
    }
};

const EditSeats = (section: ISection) => {

    sectionId.value = section.id;
    name.value = section.name;
    rows.value = section.rowsCount;
    columns.value = section.columnsCount;
    showEditForm.value = true;
    seats.length = 0;

    section.seats.forEach((seatData: ISeat) => {
        seats.push({
            ...seatData,
            color: seatData.color,
            isSelected: false
        });
    });

    renderSeats();
};
const hideEditForm = () => {
    showEditForm.value = false;

    name.value = "";
    defaultPrice.value = 100;
    clearSeats();
    rows.value = 0;
    columns.value = 0;
};

const saveUpdatedSection = async (sectionId: number) => {

    const updatedPrice = selectedPrice.value !== null ? selectedPrice.value : null;

    const updatedSeats = seats.map((seat) => {
        if (selectedSeatsArray.value.includes(seat.id) && updatedPrice !== null) {
            return {
                ...seat,
                price: seat.price,
                color: seat.color
            };
        }
        return seat;
    });
    console.log(updatedSeats);
    const payload = {
        id: sectionId,
        name: name.value,
        rowsCount: rows.value,
        columnsCount: columns.value,
        seats: updatedSeats.map((seat) => ({
            x: Number(seat.x),
            y: Number(seat.y),
            radius: Number(seat.radius),
            price: String(seat.price),
            color: String(seat.color),
            id: Number(seat.id)
        }))
    };

    try {
        await seatingStore.updateSection(sectionId, payload);

        hideEditForm();
        getallseats.getSections();
        selectedSeats.value = [];
        ClearForm();

    } catch (error) {
        console.error('Failed to update section:', error);
    }
};

const updateSeatColors = () => {
    if (selectedPrice.value) {
        const color = getColorByPrice(selectedPrice.value);
        console.log(color);
        selectedSeats.value.forEach(seatId => {
            const seat = seats.find(seat => seat.id === seatId);
            if (seat) {
                seat.color = color;
                seat.price = selectedPrice.value;
            }
        });
        console.log("Before render", seats);
        renderSeats();
        selectedSeats.value = [];
        seats.forEach(s=>s.isSelected =false);

        selectedSeatsArray.value = [];
        console.log("Aftedr render", seats);
    }
};

const ClearForm = () => {
    rows.value = 0;
    columns.value = 0;
    name.value = "";
    defaultPrice.value = 100;
}

</script>

<template>
    <div class="w-100 mt-2" v-if="!showEditForm">
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
                    <form @submit.prevent="generateSeats(1)">
                        <div class="mb-3">
                            <label for="name" class="form-label">Section Name :</label>
                            <input type="text" id="name" v-model.number="name" class="form-control" required />
                        </div>
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

    <div class="d-flex flex-row">
        <div class="w-75">
            <canvas ref="seatingChart" width="800" height="500" class="border border-dark"></canvas>
        </div>
        <div v-if="showEditForm" class="w-25 p-2">
            <div class="mb-3">
                <label for="name" class="form-label">Section Name :</label>
                <input type="text" id="name" v-model.number="name" class="form-control" required />
            </div>
            <div class="mb-3">
                <label for="defaultPrice" class="form-label">Default Seat Price:</label>
                <select id="defaultPrice" class="form-control" v-model.number="selectedPrice" @change="updateSeatColors()">
                    <option value="0">Select a Price</option>
                    <option value="100">100$</option>
                    <option value="150">150$</option>
                    <option value="200">200$</option>
                </select>
            </div>

            <button @click="hideEditForm" class="btn btn-secondary mt-2">Cancel</button>
        </div>
    </div>

    <div class="mt-3">
        <input type="checkbox" class="form-check-input me-2" v-model="selectAll" @change="toggleAllSeats" />
        <label class="form-label">Select All Seats</label>
    </div>
    <div class="mt-3">
        <button type="button" @click="saveUpdatedSection(sectionId)" class="btn btn-success" v-if="showEditForm">Update
            Selection</button>
        <button type="button" @click="RestoreSectionSeats(sectionId)" class="btn btn-secondary" v-if="showEditForm">Restore
            Seats</button>
        <button type="button" @click="saveSelection" class="btn btn-success" v-if="!showEditForm">Save Selection</button>
        <button type="button" @click="clearSeats" class="btn btn-warning ms-4" v-if="!showEditForm">Clear All Seats</button>
        <button type="button" @click="deleteSeat" class="btn btn-danger ms-4">Delete Selected Seats</button>
    </div>

    <div class="mt-5">
        <h2 class="mb-4">Section List</h2>
        <table class="table table-striped w-100">
            <tr>
                <th>Name</th>
                <th>Rows</th>
                <th>Columns</th>
                <th>Actions</th>
            </tr>
            <tr v-for="item in getallseats.sections">
                <td>{{ item.name }}</td>
                <td>{{ item.rowsCount }}</td>
                <td>{{ item.columnsCount }}</td>
                <td>
                    <div>
                        <button @click="EditSeats(item)" class="btn btn-success me-2">Edit</button>
                        <button @click="openConfirmationModal(item.id)" class="btn btn-danger">Delete</button>
                    </div>
                </td>
            </tr>
        </table>
    </div>

    <div v-if="showConfirmationModal" class="modal-overlay" @click.self="closeConfirmationModal">
        <div class="modal-content">
            <h3>Confirm Deletion</h3>
            <p>Are you sure you want to delete this Section?</p>
            <div class="modal-actions">
                <button @click="deleteSection(sectionToDelete)" class="btn btn-danger">Delete</button>
                <button @click="closeConfirmationModal" class="btn btn-secondary">Cancel</button>
            </div>
        </div>
    </div>
</template>


<style scoped>
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
</style>