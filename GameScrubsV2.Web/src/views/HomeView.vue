<template>
  <div class="flex h-screen overflow-hidden">
    <!-- Sidebar -->
    <Sidebar :sidebarOpen="sidebarOpen" @close-sidebar="sidebarOpen = false" variant="v2" />

    <!-- Content area -->
    <div class="relative flex flex-col flex-1 overflow-y-auto overflow-x-hidden">
      <!-- Header -->
      <Header
        :sidebarOpen="sidebarOpen"
        :variant="HeaderVariant.Bracket"
        @toggle-sidebar="sidebarOpen = !sidebarOpen"
      />

      <!-- Main content -->
      <main class="grow">
        <div class="px-4 sm:px-6 lg:px-8 py-8 w-full max-w-9xl mx-auto"></div>
      </main>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, type Ref } from 'vue';
import Sidebar from '@/partials/Sidebar.vue';
import Header from '@/partials/Header.vue';
import { HeaderVariant } from '@/models/HeaderVariant';

const sidebarOpen: Ref<boolean> = ref(false);
</script>
