<script setup lang="ts">
import { useRoute } from 'vue-router';

const route = useRoute();
</script>

<template>
    <div>
        <div class="d-flex flex-column flex-shrink-0 p-3 text-bg-dark w-100 h100">
            <a href="/" class="d-flex align-items-center mb-3 mb-md-0 me-md-auto text-white text-decoration-none">
                <svg class="bi pe-none me-2" width="40" height="32">
                    <use xlink:href="#bootstrap"></use>
                </svg>
                <span class="fs-4">Admin</span>
            </a>
            <hr>
            <ul class="nav nav-pills flex-column mb-auto">
                <li class="nav-item">
                    <nuxt-link 
                        to="/AddSeats" 
                        :class="{'nav-link text-white': true, 'active text-white': route.path === '/AddSeats'}" 
                        aria-current="page">
                        Add Section
                    </nuxt-link>
                </li>
                <li>
                    <nuxt-link 
                        to="/AddVenue" 
                        :class="{'nav-link text-white': true, 'active': route.path === '/AddVenue'}" 
                        aria-current="page">
                        Add Venue
                    </nuxt-link>
                </li>
            </ul>
            <hr>
        </div>
    </div>
</template>

<style scoped>
.h100 {
    height: 100vh;
}
</style>
