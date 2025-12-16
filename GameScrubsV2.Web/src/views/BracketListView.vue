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

          <!-- Filters -->
          <div class="bg-white dark:bg-gray-800 shadow-xs rounded-xl p-3 mb-4">
            <div class="flex flex-wrap items-end gap-3">
              <!-- Name Filter -->
              <div class="flex-1 min-w-[150px]">
                <label class="block text-xs font-medium text-gray-700 dark:text-gray-300 mb-1">
                  Name
                </label>
                <input
                  v-model="filters.name"
                  type="text"
                  placeholder="Filter..."
                  class="w-full px-2 py-1.5 text-sm border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-gray-100"
                  @input="handleTextFilterChange"
                />
              </div>

              <!-- Game Filter -->
              <div class="flex-1 min-w-[150px]">
                <label class="block text-xs font-medium text-gray-700 dark:text-gray-300 mb-1">
                  Game
                </label>
                <input
                  v-model="filters.game"
                  type="text"
                  placeholder="Filter..."
                  class="w-full px-2 py-1.5 text-sm border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-gray-100"
                  @input="handleTextFilterChange"
                />
              </div>

              <!-- Status Filter -->
              <div class="min-w-[120px]">
                <label class="block text-xs font-medium text-gray-700 dark:text-gray-300 mb-1">
                  Status
                </label>
                <select
                  v-model="filters.status"
                  class="w-full px-2 py-1.5 text-sm border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-gray-100"
                  @change="loadItems"
                >
                  <option value="">All</option>
                  <option value="Setup">Setup</option>
                  <option value="Started">Started</option>
                  <option value="OnHold">On Hold</option>
                  <option value="Completed">Completed</option>
                </select>
              </div>

              <!-- Type Filter -->
              <div class="min-w-[140px]">
                <label class="block text-xs font-medium text-gray-700 dark:text-gray-300 mb-1">
                  Type
                </label>
                <select
                  v-model="filters.type"
                  class="w-full px-2 py-1.5 text-sm border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-gray-100"
                  @change="loadItems"
                >
                  <option value="">All</option>
                  <option value="Single_8">8 Single</option>
                  <option value="Single_16">16 Single</option>
                  <option value="Single_32">32 Single</option>
                  <option value="Double_8">8 Double</option>
                  <option value="Double_16">16 Double</option>
                  <option value="Double_32">32 Double</option>
                </select>
              </div>

              <!-- Clear Filters Button -->
              <button
                v-if="hasActiveFilters"
                @click="clearFilters"
                class="px-3 py-1.5 text-xs bg-gray-200 hover:bg-gray-300 dark:bg-gray-700 dark:hover:bg-gray-600 text-gray-800 dark:text-gray-200 rounded-lg transition-colors"
              >
                Clear
              </button>
            </div>
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
                    <th class="px-2 first:pl-5 last:pr-5 py-3 whitespace-nowrap w-px">
                      <div class="font-semibold text-left">#</div>
                    </th>
                    <th class="px-2 first:pl-5 last:pr-5 py-3 whitespace-nowrap">
                      <div class="font-semibold text-left">Name</div>
                    </th>
                    <th class="px-2 first:pl-5 last:pr-5 py-3 whitespace-nowrap">
                      <div class="font-semibold text-left">Game</div>
                    </th>
                    <th class="px-2 first:pl-5 last:pr-5 py-3 whitespace-nowrap">
                      <div class="font-semibold text-left">Type</div>
                    </th>
                    <th class="px-2 first:pl-5 last:pr-5 py-3 whitespace-nowrap">
                      <div class="font-semibold text-left">Status</div>
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
                  <tr v-for="(item, index) in items" :key="item.id">
                    <td class="px-2 first:pl-5 last:pr-5 py-3 whitespace-nowrap w-px">
                      <div class="text-gray-500 dark:text-gray-400">{{ index + 1 }}</div>
                    </td>
                    <td class="px-2 first:pl-5 last:pr-5 py-3 whitespace-nowrap">
                      <div class="flex items-center gap-2">
                        <router-link
                          :to="{ name: 'bracket', params: { id: item.id } }"
                          class="font-medium text-blue-600 hover:text-blue-800 dark:text-blue-400 dark:hover:text-blue-300 cursor-pointer"
                        >
                          {{ item.name }}
                        </router-link>
                        <IconLock
                          v-if="item.isLocked"
                          :size="16"
                          :stroke-width="2"
                          class="text-gray-500 dark:text-gray-400 shrink-0"
                          title="This bracket is locked"
                        />
                      </div>
                    </td>
                    <td class="px-2 first:pl-5 last:pr-5 py-3 whitespace-nowrap">
                      <div class="text-gray-900 dark:text-gray-100">{{ item.game }}</div>
                    </td>
                    <td class="px-2 first:pl-5 last:pr-5 py-3 whitespace-nowrap">
                      <span :class="getTypeBadgeClass(item.type)">
                        {{ formatBracketType(item.type) }}
                      </span>
                    </td>
                    <td class="px-2 first:pl-5 last:pr-5 py-3 whitespace-nowrap">
                      <span :class="getStatusBadgeClass(item.status)">
                        {{ item.status }}
                      </span>
                    </td>
                    <td class="px-2 first:pl-5 last:pr-5 py-3 whitespace-nowrap">
                      <span :class="getCompetitionBadgeClass()">
                        {{ item.competition }}
                      </span>
                    </td>
                    <td class="px-2 first:pl-5 last:pr-5 py-3 whitespace-nowrap">
                      <div class="text-gray-700 dark:text-gray-300">
                        {{ formatDate(item.startDate) }}
                      </div>
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
import { ref, onMounted, computed } from 'vue';
import { bracketService } from '@/services/bracketService';
import Sidebar from '../partials/Sidebar.vue';
import Header from '../partials/Header.vue';
import { IconUsers, IconSettings, IconLock } from '@tabler/icons-vue';
import type { Bracket } from '@/models/Bracket';

const items = ref<Bracket[]>([]);
const loading = ref(false);
const error = ref<string | null>(null);

const sidebarOpen = ref(false);

// Filter state
const filters = ref({
  name: '',
  game: '',
  status: '',
  type: '',
});

// Debounce timer for text inputs
let filterDebounceTimer: ReturnType<typeof setTimeout> | null = null;

// Check if any filters are active
const hasActiveFilters = computed(() => {
  return (
    filters.value.name !== '' ||
    filters.value.game !== '' ||
    filters.value.status !== '' ||
    filters.value.type !== ''
  );
});

const loadItems = async () => {
  loading.value = true;
  error.value = null;
  try {
    const params: any = {};

    if (filters.value.name) params.name = filters.value.name;
    if (filters.value.game) params.game = filters.value.game;
    if (filters.value.status) params.status = filters.value.status;
    if (filters.value.type) params.type = filters.value.type;

    items.value = await bracketService.search(Object.keys(params).length > 0 ? params : undefined);
  } catch (err) {
    error.value = err instanceof Error ? err.message : 'An error occurred';
  } finally {
    loading.value = false;
  }
};

// Handle filter changes with debounce for text inputs
const handleTextFilterChange = () => {
  if (filterDebounceTimer) {
    clearTimeout(filterDebounceTimer);
  }

  filterDebounceTimer = setTimeout(() => {
    loadItems();
  }, 500);
};

// Clear all filters
const clearFilters = () => {
  filters.value = {
    name: '',
    game: '',
    status: '',
    type: '',
  };
  loadItems();
};

// Format bracket type from "Double_16" to "16 Double Elimination"
const formatBracketType = (type: string): string => {
  const match = type.match(/^(Single|Double)_(\d+)$/);
  if (match) {
    const [, eliminationType, playerCount] = match;
    return `${playerCount} ${eliminationType} Elimination`;
  }
  return type;
};

// Format date to just show MM/DD/YYYY
const formatDate = (dateString: string | null): string => {
  if (!dateString) return '';
  const date = new Date(dateString);
  return date.toLocaleDateString('en-US', {
    month: '2-digit',
    day: '2-digit',
    year: 'numeric',
  });
};

// Get badge class for status
const getStatusBadgeClass = (status: string): string => {
  const baseClasses = 'inline-flex items-center px-2.5 py-1 rounded-full text-xs font-medium';

  switch (status) {
    case 'Setup':
      return `${baseClasses} bg-blue-100 text-blue-700 dark:bg-blue-900/30 dark:text-blue-400`;
    case 'Started':
      return `${baseClasses} bg-green-100 text-green-700 dark:bg-green-900/30 dark:text-green-400`;
    case 'OnHold':
      return `${baseClasses} bg-yellow-100 text-yellow-700 dark:bg-yellow-900/30 dark:text-yellow-400`;
    case 'Completed':
      return `${baseClasses} bg-gray-100 text-gray-700 dark:bg-gray-900/30 dark:text-gray-400`;
    default:
      return `${baseClasses} bg-gray-100 text-gray-700 dark:bg-gray-900/30 dark:text-gray-400`;
  }
};

// Get badge class for competition type
const getCompetitionBadgeClass = (): string => {
  return 'inline-flex items-center px-2.5 py-1 rounded-full text-xs font-medium bg-purple-100 text-purple-700 dark:bg-purple-900/30 dark:text-purple-400';
};

// Get badge class for bracket type based on player count and elimination type
const getTypeBadgeClass = (type: string): string => {
  const baseClasses = 'inline-flex items-center px-2.5 py-1 rounded-full text-xs font-medium';

  // Determine if it's Single or Double elimination
  const isSingle = type.startsWith('Single');

  // Extract player count
  const match = type.match(/_(\d+)$/);
  const playerCount = match ? match[1] : '';

  // Different colors based on elimination type and player count
  if (isSingle) {
    switch (playerCount) {
      case '8':
        return `${baseClasses} bg-blue-100 text-blue-700 dark:bg-blue-900/30 dark:text-blue-400`;
      case '16':
        return `${baseClasses} bg-cyan-100 text-cyan-700 dark:bg-cyan-900/30 dark:text-cyan-400`;
      case '32':
        return `${baseClasses} bg-sky-100 text-sky-700 dark:bg-sky-900/30 dark:text-sky-400`;
      default:
        return `${baseClasses} bg-blue-100 text-blue-700 dark:bg-blue-900/30 dark:text-blue-400`;
    }
  } else {
    // Double elimination
    switch (playerCount) {
      case '8':
        return `${baseClasses} bg-amber-100 text-amber-700 dark:bg-amber-900/30 dark:text-amber-400`;
      case '16':
        return `${baseClasses} bg-orange-100 text-orange-700 dark:bg-orange-900/30 dark:text-orange-400`;
      case '32':
        return `${baseClasses} bg-red-100 text-red-700 dark:bg-red-900/30 dark:text-red-400`;
      default:
        return `${baseClasses} bg-amber-100 text-amber-700 dark:bg-amber-900/30 dark:text-amber-400`;
    }
  }
};

onMounted(() => {
  loadItems();
});
</script>
