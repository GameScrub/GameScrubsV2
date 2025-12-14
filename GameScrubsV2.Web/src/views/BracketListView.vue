<template>
  <div class="flex h-[100dvh] overflow-hidden">
    <!-- Sidebar -->
    <Sidebar :sidebarOpen="sidebarOpen" @close-sidebar="sidebarOpen = false" />

    <!-- Content area -->
    <div class="relative flex flex-col flex-1 overflow-y-auto overflow-x-hidden">
      <!-- Site header -->
      <Header :sidebarOpen="sidebarOpen" @toggle-sidebar="sidebarOpen = !sidebarOpen" />

      <main class="grow">
        <div class="px-4 sm:px-6 lg:px-8 py-8 w-full mx-auto">
          <!-- Page header -->
          <div class="sm:flex sm:justify-between sm:items-center mb-8">
            <!-- Left: Title -->
            <div class="mb-4 sm:mb-0">
              <h1 class="text-2xl md:text-3xl text-gray-800 dark:text-gray-100 font-bold">
                Brackets
              </h1>
            </div>

            <!-- Right: Actions  -->
            <div
              class="grid grid-flow-col sm:auto-cols-max justify-start sm:justify-end gap-2"
            ></div>
          </div>

          <!-- Table -->
          <div v-if="loading">Loading...</div>
          <div v-if="error" class="error">{{ error }}</div>

          <div class="bg-white dark:bg-gray-800 shadow-xs rounded-xl relative">
            <header class="px-5 py-4">
              <h2 class="font-semibold text-gray-800 dark:text-gray-100">
                Found
                <span class="text-gray-400 dark:text-gray-500 font-medium">{{ items.length }}</span>
              </h2>
            </header>

            <div class="overflow-x-auto">
              <table
                class="table-auto w-full dark:text-gray-300 divide-y divide-gray-100 dark:divide-gray-700/60"
              >
                <!-- Table header -->
                <thead
                  class="text-xs uppercase text-gray-500 dark:text-gray-400 bg-gray-50 dark:bg-gray-900/20 border-t border-gray-100 dark:border-gray-700/60"
                >
                  <tr>
                    <th class="px-2 first:pl-5 last:pr-5 py-3 whitespace-nowrap">
                      <div class="font-semibold text-left">Id</div>
                    </th>
                    <th class="px-2 first:pl-5 last:pr-5 py-3 whitespace-nowrap">
                      <div class="font-semibold text-left">Name</div>
                    </th>
                    <th class="px-2 first:pl-5 last:pr-5 py-3 whitespace-nowrap">
                      <div class="font-semibold text-left">Game</div>
                    </th>
                    <th class="px-2 first:pl-5 last:pr-5 py-3 whitespace-nowrap">
                      <div class="font-semibold text-left">Url</div>
                    </th>
                    <th class="px-2 first:pl-5 last:pr-5 py-3 whitespace-nowrap">
                      <div class="font-semibold text-left">IsLocked</div>
                    </th>
                    <th class="px-2 first:pl-5 last:pr-5 py-3 whitespace-nowrap">
                      <div class="font-semibold">Type</div>
                    </th>
                    <th class="px-2 first:pl-5 last:pr-5 py-3 whitespace-nowrap">
                      <div class="font-semibold text-left">Staus</div>
                    </th>
                    <th class="px-2 first:pl-5 last:pr-5 py-3 whitespace-nowrap">
                      <div class="font-semibold text-left">Competition</div>
                    </th>
                    <th class="px-2 first:pl-5 last:pr-5 py-3 whitespace-nowrap">
                      <div class="font-semibold text-left">Start Date</div>
                    </th>
                    <th class="px-2 first:pl-5 last:pr-5 py-3 whitespace-nowrap">
                      <div class="font-semibold text-left">Actions</div>
                    </th>
                  </tr>
                </thead>
                <tbody
                  v-if="items.length"
                  class="text-sm divide-y divide-gray-100 dark:divide-gray-700/60"
                >
                  <tr v-for="item in items" :key="item.id">
                    <td class="px-2 first:pl-5 last:pr-5 py-3 whitespace-nowrap w-px">
                      {{ item.id }}
                    </td>
                    <td class="px-2 first:pl-5 last:pr-5 py-3 whitespace-nowrap">
                      <router-link
                        :to="{ name: 'bracket', params: { id: item.id } }"
                        class="text-blue-600 hover:text-blue-800 dark:text-blue-400 dark:hover:text-blue-300 cursor-pointer"
                      >
                        {{ item.name }}
                      </router-link>
                    </td>
                    <td class="px-2 first:pl-5 last:pr-5 py-3 whitespace-nowrap">
                      {{ item.game }}
                    </td>
                    <td class="px-2 first:pl-5 last:pr-5 py-3 whitespace-nowrap">
                      {{ item.url }}
                    </td>
                    <td class="px-2 first:pl-5 last:pr-5 py-3 whitespace-nowrap">
                      {{ item.isLocked }}
                    </td>
                    <td class="px-2 first:pl-5 last:pr-5 py-3 whitespace-nowrap">
                      {{ item.type }}
                    </td>
                    <td class="px-2 first:pl-5 last:pr-5 py-3 whitespace-nowrap">
                      {{ item.status }}
                    </td>
                    <td class="px-2 first:pl-5 last:pr-5 py-3 whitespace-nowrap">
                      {{ item.competition }}
                    </td>
                    <td class="px-2 first:pl-5 last:pr-5 py-3 whitespace-nowrap">
                      {{ item.startDate }}
                    </td>
                    <td class="px-2 first:pl-5 last:pr-5 py-3 whitespace-nowrap">
                      <div class="flex items-center gap-3">
                        <router-link
                          :to="{ name: 'bracket-edit', params: { id: item.id } }"
                          class="text-gray-600 hover:text-blue-600 dark:text-gray-400 dark:hover:text-blue-400 transition-colors"
                          title="Edit Bracket"
                        >
                          <IconSettings :size="20" :stroke-width="1.5" />
                        </router-link>
                        <router-link
                          :to="{ name: 'bracket-manage-users', params: { id: item.id } }"
                          class="text-gray-600 hover:text-blue-600 dark:text-gray-400 dark:hover:text-blue-400 transition-colors"
                          title="Manage Players"
                        >
                          <IconUsers :size="20" :stroke-width="1.5" />
                        </router-link>
                      </div>
                    </td>
                  </tr>
                </tbody>
              </table>
            </div>
          </div>
        </div>
      </main>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { bracketService } from '@/services/bracketService';
import Sidebar from '../partials/Sidebar.vue';
import Header from '../partials/Header.vue';
import { IconUsers, IconSettings } from '@tabler/icons-vue';
import type { Bracket } from '@/models/Bracket';

const items = ref<Bracket[]>([]);
const loading = ref(false);
const error = ref<string | null>(null);

const sidebarOpen = ref(false);

const loadItems = async () => {
  loading.value = true;
  error.value = null;
  try {
    items.value = await bracketService.getAll();
  } catch (err) {
    error.value = err instanceof Error ? err.message : 'An error occurred';
  } finally {
    loading.value = false;
  }
};

onMounted(() => {
  loadItems();
});
</script>
