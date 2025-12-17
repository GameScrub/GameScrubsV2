<template>
  <div class="flex h-screen overflow-hidden">
    <!-- Sidebar -->
    <Sidebar
      :sidebarOpen="sidebarOpen"
      @close-sidebar="sidebarOpen = false"
      :variant="SidebarVariant.V2"
    />

    <!-- Content area -->
    <div class="relative flex flex-col flex-1 overflow-y-auto overflow-x-hidden">
      <!-- Header (hidden on desktop, shows mobile burger menu) -->
      <Header
        :sidebarOpen="sidebarOpen"
        :variant="HeaderVariant.Bracket"
        @toggle-sidebar="sidebarOpen = !sidebarOpen"
        class="lg:hidden"
      />

      <!-- Main content -->
      <main class="grow">
        <div class="h-56 bg-gray-200 dark:bg-gray-900">
          <img
            class="object-cover h-full w-full"
            src="@/images/main-banner.png"
            width="2560"
            height="440"
            alt="Company background"
          />
        </div>

        <!-- Header -->
        <header
          class="text-center bg-white/30 dark:bg-gray-800/30 pb-6 border-b border-gray-200 dark:border-gray-700/60"
        >
          <div class="px-4 sm:px-6 lg:px-8 w-full">
            <div class="max-w-3xl mx-auto">
              <!-- Avatar -->
              <div class="-mt-8 mb-2">
                <div class="inline-flex -ml-1 -mt-1 sm:mb-0">
                  <img
                    class="rounded-full"
                    src="@/images/GameScrubsLogo.png"
                    width="104"
                    height="104"
                    alt="GameScrubs Logo"
                  />
                </div>
              </div>

              <!-- Company name and info -->
              <div class="mb-4">
                <h2 class="text-2xl text-gray-800 dark:text-gray-100 font-bold mb-2">GameScrubs</h2>
                <p>
                  Online brackets for 8, 16 and 32 person single or double elimination tournaments
                </p>
              </div>
            </div>
          </div>
        </header>
        <div class="px-4 sm:px-6 lg:px-8 py-8 w-full max-w-9xl mx-auto">
          <!-- Page content -->
          <div class="mx-auto grid grid-cols-1 lg:grid-cols-12 gap-8">
            <!-- Welcome Section -->
            <div class="lg:col-span-6">
              <header class="mb-6">
                <h1 class="text-2xl md:text-3xl text-gray-800 dark:text-gray-100 font-bold mb-3">
                  Welcome to GameScrubs!
                </h1>
                <p class="text-lg text-gray-600 dark:text-gray-400">
                  The easiest way to create and manage tournament brackets for your gaming
                  competitions, sports events, and esports tournaments.
                </p>
              </header>

              <!-- Core Features -->
              <div class="space-y-5">
                <div class="flex items-start gap-3">
                  <div
                    class="shrink-0 w-10 h-10 bg-blue-500/10 dark:bg-blue-500/20 rounded-lg flex items-center justify-center"
                  >
                    <svg
                      class="w-6 h-6 text-blue-600 dark:text-blue-400"
                      fill="none"
                      stroke="currentColor"
                      viewBox="0 0 24 24"
                    >
                      <path
                        stroke-linecap="round"
                        stroke-linejoin="round"
                        stroke-width="2"
                        d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z"
                      />
                    </svg>
                  </div>
                  <div>
                    <h3 class="text-lg font-semibold text-gray-800 dark:text-gray-100 mb-1">
                      Simple Tournament Creation
                    </h3>
                    <p class="text-gray-600 dark:text-gray-400 text-sm">
                      Create single or double elimination brackets for 8, 16, or 32 players in
                      seconds. No complex setup required.
                    </p>
                  </div>
                </div>

                <div class="flex items-start gap-3">
                  <div
                    class="shrink-0 w-10 h-10 bg-green-500/10 dark:bg-green-500/20 rounded-lg flex items-center justify-center"
                  >
                    <svg
                      class="w-6 h-6 text-green-600 dark:text-green-400"
                      fill="none"
                      stroke="currentColor"
                      viewBox="0 0 24 24"
                    >
                      <path
                        stroke-linecap="round"
                        stroke-linejoin="round"
                        stroke-width="2"
                        d="M13 10V3L4 14h7v7l9-11h-7z"
                      />
                    </svg>
                  </div>
                  <div>
                    <h3 class="text-lg font-semibold text-gray-800 dark:text-gray-100 mb-1">
                      Real-Time Updates
                    </h3>
                    <p class="text-gray-600 dark:text-gray-400 text-sm">
                      Track matches live with instant bracket updates. Click any match to record
                      winners and watch the bracket progress automatically.
                    </p>
                  </div>
                </div>

                <div class="flex items-start gap-3">
                  <div
                    class="shrink-0 w-10 h-10 bg-purple-500/10 dark:bg-purple-500/20 rounded-lg flex items-center justify-center"
                  >
                    <svg
                      class="w-6 h-6 text-purple-600 dark:text-purple-400"
                      fill="none"
                      stroke="currentColor"
                      viewBox="0 0 24 24"
                    >
                      <path
                        stroke-linecap="round"
                        stroke-linejoin="round"
                        stroke-width="2"
                        d="M12 15v2m-6 4h12a2 2 0 002-2v-6a2 2 0 00-2-2H6a2 2 0 00-2 2v6a2 2 0 002 2zm10-10V7a4 4 0 00-8 0v4h8z"
                      />
                    </svg>
                  </div>
                  <div>
                    <h3 class="text-lg font-semibold text-gray-800 dark:text-gray-100 mb-1">
                      Secure & Protected
                    </h3>
                    <p class="text-gray-600 dark:text-gray-400 text-sm">
                      Lock code protection ensures only authorized users can update match results
                      and modify tournament settings.
                    </p>
                  </div>
                </div>

                <div class="flex items-start gap-3">
                  <div
                    class="shrink-0 w-10 h-10 bg-orange-500/10 dark:bg-orange-500/20 rounded-lg flex items-center justify-center"
                  >
                    <svg
                      class="w-6 h-6 text-orange-600 dark:text-orange-400"
                      fill="none"
                      stroke="currentColor"
                      viewBox="0 0 24 24"
                    >
                      <path
                        stroke-linecap="round"
                        stroke-linejoin="round"
                        stroke-width="2"
                        d="M7 21a4 4 0 01-4-4V5a2 2 0 012-2h4a2 2 0 012 2v12a4 4 0 01-4 4zm0 0h12a2 2 0 002-2v-4a2 2 0 00-2-2h-2.343M11 7.343l1.657-1.657a2 2 0 012.828 0l2.829 2.829a2 2 0 010 2.828l-8.486 8.485M7 17h.01"
                      />
                    </svg>
                  </div>
                  <div>
                    <h3 class="text-lg font-semibold text-gray-800 dark:text-gray-100 mb-1">
                      Easy Player Management
                    </h3>
                    <p class="text-gray-600 dark:text-gray-400 text-sm">
                      Add, remove, and reorder players with intuitive drag-and-drop. Automatic
                      seeding and bye handling included.
                    </p>
                  </div>
                </div>
              </div>

              <!-- Closing Statement -->
              <div class="mt-8 pt-6 border-t border-gray-200 dark:border-gray-700">
                <p class="text-gray-600 dark:text-gray-400">
                  GameScrubs.com essentially provides organizers with everything they need to run
                  professional-looking tournament brackets without requiring any special software or
                  technical knowledge.
                </p>
              </div>
            </div>

            <!-- Recent Activity Section -->
            <div class="lg:col-span-6">
              <RecentActivity />
            </div>
          </div>
        </div>
      </main>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, type Ref } from 'vue';
import Sidebar from '@/partials/Sidebar.vue';
import Header from '@/partials/Header.vue';
import RecentActivity from '@/components/RecentActivity.vue';
import { SidebarVariant } from '@/models/SidebarVariant';
import { HeaderVariant } from '@/models/HeaderVariant';

const sidebarOpen: Ref<boolean> = ref(false);
</script>
