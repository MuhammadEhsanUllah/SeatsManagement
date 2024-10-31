<script lang="ts" setup></script>
<template>
    <div>
        <div class="row w-100">
            <div class="col-2">
                <Sidebar />
            </div>
            <div class="col-10 main-content">
                <client />
            </div>
        </div>
    </div>
</template>

<style scoped>
   .main-content {
    height: 100vh;
    overflow-y: scroll;
   }
</style>