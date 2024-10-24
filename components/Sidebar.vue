<script setup lang="ts">

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
                    <a href="#" class="nav-link active" aria-current="page">
                        Add New Section
                    </a>
                </li>
                <li>
                    <a href="#" class="nav-link text-white">
                        Show All Sections
                    </a>
                </li>
            </ul>
            <hr>
        </div>
    </div>
</template>

<style scoped>
 .h100{
    height:100vh;
 }
</style>