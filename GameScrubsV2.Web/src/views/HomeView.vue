<template>
  <div class="flex h-screen overflow-hidden">
    <!-- Sidebar -->
    <Sidebar :sidebarOpen="sidebarOpen" @close-sidebar="sidebarOpen = false" variant="v2" />

    <!-- Content area -->
    <div class="relative flex flex-col flex-1 overflow-y-auto overflow-x-hidden">
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
              <header class="mb-4">
                <!-- Title -->
                <h1 class="text-2xl md:text-3xl text-gray-800 dark:text-gray-100 font-bold">
                  Welcome!
                </h1>
              </header>

              <div>
                <h2 class="text-l leading-snug text-gray-800 dark:text-gray-100 font-bold mb-2">
                  Bacon ipsum
                </h2>
                <div class="space-y-6">
                  <p>
                    Bacon ipsum dolor amet fatback chuck biltong alcatra, pastrami shoulder short
                    ribs beef ribs strip steak sirloin brisket pork loin pancetta. Chislic rump
                    short loin pastrami strip steak cow pork ham sirloin ground round corned beef
                    frankfurter beef ribs shankle. Shoulder leberkas boudin, cupim ham chuck
                    porchetta pancetta tail pig filet mignon. Tenderloin ribeye corned beef pork
                    loin salami. Short loin pork loin strip steak, ground round meatball drumstick
                    andouille.
                  </p>

                  <p>
                    Hamburger cupim landjaeger corned beef, doner salami picanha alcatra chuck
                    tenderloin venison ham hock frankfurter tail. Shankle tri-tip tenderloin
                    bresaola, sausage turducken biltong pastrami cow kevin shoulder beef pancetta.
                    Spare ribs pork chop kielbasa brisket andouille chicken. Beef ribs shoulder
                    pancetta frankfurter biltong. Pork chop salami shoulder alcatra prosciutto
                    meatloaf. Frankfurter meatball sausage shank capicola salami turkey kevin.
                    Meatball pork belly strip steak chicken t-bone.
                  </p>
                </div>
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
import RecentActivity from '@/components/RecentActivity.vue';

const sidebarOpen: Ref<boolean> = ref(false);
</script>
