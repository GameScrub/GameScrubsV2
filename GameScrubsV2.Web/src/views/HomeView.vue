<template>
  <div class="flex h-screen overflow-hidden">
    
    <!-- Sidebar -->
    <Sidebar 
      :sidebarOpen="sidebarOpen" 
      @close-sidebar="sidebarOpen = false"
      variant="v2"
    />

    <!-- Content area -->
    <div class="relative flex flex-col flex-1 overflow-y-auto overflow-x-hidden">
      
      <!-- Header with mobile menu button -->
      <header class="sticky top-0 bg-white dark:bg-gray-800 border-b border-gray-200 dark:border-gray-700/60 z-30">
        <div class="px-4 sm:px-6 lg:px-8">
          <div class="flex items-center justify-between h-16 -mb-px">
            
            <!-- Hamburger button (mobile) -->
            <button
              class="text-gray-500 hover:text-gray-600 lg:hidden"
              @click.stop="sidebarOpen = !sidebarOpen"
              aria-controls="sidebar"
              :aria-expanded="sidebarOpen"
            >
              <span class="sr-only">Open sidebar</span>
              <svg class="w-6 h-6 fill-current" viewBox="0 0 24 24" xmlns="http://www.w3.org/2000/svg">
                <rect x="4" y="5" width="16" height="2" />
                <rect x="4" y="11" width="16" height="2" />
                <rect x="4" y="17" width="16" height="2" />
              </svg>
            </button>

            <div class="flex items-center space-x-3">
              <!-- Your header content -->
            </div>

          </div>
        </div>
      </header>

      <!-- Main content -->
      <main class="grow">
        <div class="px-4 sm:px-6 lg:px-8 py-8 w-full max-w-9xl mx-auto">
          
          <!-- Your page content / router-view goes here -->
          <router-view />
          
        </div>
      </main>

    </div>

  </div>
</template>

<script setup lang="ts">
import { ref, type Ref } from 'vue'
import Sidebar from '@/partials/Sidebar.vue'

const sidebarOpen: Ref<boolean> = ref(false)
</script>