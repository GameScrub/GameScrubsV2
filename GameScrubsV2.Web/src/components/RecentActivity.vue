<template>
  <div class="col-span-full bg-white dark:bg-gray-800 shadow-xs rounded-xl">
    <header class="px-5 py-4 border-b border-gray-100 dark:border-gray-700/60">
      <h2 class="font-semibold text-gray-800 dark:text-gray-100">Recent Activity</h2>
    </header>
    <div class="p-3">
      <!-- Loading state -->
      <div v-if="loading" class="text-center py-8 text-gray-500 dark:text-gray-400">Loading...</div>

      <!-- Error state -->
      <div v-else-if="error" class="text-center py-8 text-red-500">
        {{ error }}
      </div>

      <!-- Empty state -->
      <div
        v-else-if="!activityGroups || activityGroups.length === 0"
        class="text-center py-8 text-gray-500 dark:text-gray-400"
      >
        No recent activity
      </div>

      <!-- Activity groups by status -->
      <div v-else>
        <div v-for="group in activityGroups" :key="group.key" class="mb-4">
          <header
            class="text-xs uppercase text-gray-400 dark:text-gray-500 bg-gray-50 dark:bg-gray-700/50 rounded-xs font-semibold p-2"
          >
            {{ formatStatusLabel(group.key) }}
          </header>
          <ul class="my-1">
            <li v-for="(bracket, index) in group.brackets" :key="bracket.id">
              <router-link
                :to="`/bracket/${bracket.id}`"
                class="flex px-2 hover:bg-gray-50 dark:hover:bg-gray-700/30 rounded-lg transition-colors cursor-pointer"
              >
                <div
                  :class="[
                    'w-10 h-10 rounded-full shrink-0 my-3 mr-3 flex items-center justify-center',
                    getStatusColor(group.key),
                  ]"
                >
                  <svg
                    v-if="group.key === BracketStatus.Setup"
                    class="w-6 h-6 stroke-current text-white"
                    fill="none"
                    viewBox="0 0 24 24"
                    stroke-width="2"
                  >
                    <path
                      stroke-linecap="round"
                      stroke-linejoin="round"
                      d="M10.343 3.94c.09-.542.56-.94 1.11-.94h1.093c.55 0 1.02.398 1.11.94l.149.894c.07.424.384.764.78.93.398.164.855.142 1.205-.108l.737-.527a1.125 1.125 0 011.45.12l.773.774c.39.389.44 1.002.12 1.45l-.527.737c-.25.35-.272.806-.107 1.204.165.397.505.71.93.78l.893.15c.543.09.94.56.94 1.109v1.094c0 .55-.397 1.02-.94 1.11l-.893.149c-.425.07-.765.383-.93.78-.165.398-.143.854.107 1.204l.527.738c.32.447.269 1.06-.12 1.45l-.774.773a1.125 1.125 0 01-1.449.12l-.738-.527c-.35-.25-.806-.272-1.203-.107-.397.165-.71.505-.781.929l-.149.894c-.09.542-.56.94-1.11.94h-1.094c-.55 0-1.019-.398-1.11-.94l-.148-.894c-.071-.424-.384-.764-.781-.93-.398-.164-.854-.142-1.204.108l-.738.527c-.447.32-1.06.269-1.45-.12l-.773-.774a1.125 1.125 0 01-.12-1.45l.527-.737c.25-.35.273-.806.108-1.204-.165-.397-.505-.71-.93-.78l-.894-.15c-.542-.09-.94-.56-.94-1.109v-1.094c0-.55.398-1.02.94-1.11l.894-.149c.424-.07.765-.383.93-.78.165-.398.143-.854-.107-1.204l-.527-.738a1.125 1.125 0 01.12-1.45l.773-.773a1.125 1.125 0 011.45-.12l.737.527c.35.25.807.272 1.204.107.397-.165.71-.505.78-.929l.15-.894z"
                    />
                    <path
                      stroke-linecap="round"
                      stroke-linejoin="round"
                      d="M15 12a3 3 0 11-6 0 3 3 0 016 0z"
                    />
                  </svg>
                  <svg
                    v-else-if="group.key === BracketStatus.Started"
                    class="w-6 h-6 stroke-current text-white"
                    fill="none"
                    viewBox="0 0 24 24"
                    stroke-width="2"
                  >
                    <path
                      stroke-linecap="round"
                      stroke-linejoin="round"
                      d="M5.25 5.653c0-.856.917-1.398 1.667-.986l11.54 6.348a1.125 1.125 0 010 1.971l-11.54 6.347a1.125 1.125 0 01-1.667-.985V5.653z"
                    />
                  </svg>
                  <svg
                    v-else-if="group.key === BracketStatus.OnHold"
                    class="w-6 h-6 stroke-current text-white"
                    fill="none"
                    viewBox="0 0 24 24"
                    stroke-width="2"
                  >
                    <path
                      stroke-linecap="round"
                      stroke-linejoin="round"
                      d="M15.75 5.25v13.5m-7.5-13.5v13.5"
                    />
                  </svg>
                  <svg
                    v-else-if="group.key === BracketStatus.Completed"
                    class="w-6 h-6 stroke-current text-white"
                    fill="none"
                    viewBox="0 0 24 24"
                    stroke-width="2"
                  >
                    <path
                      stroke-linecap="round"
                      stroke-linejoin="round"
                      d="M9 12.75L11.25 15 15 9.75M21 12a9 9 0 11-18 0 9 9 0 0118 0z"
                    />
                  </svg>
                </div>
                <div
                  :class="[
                    'grow flex py-3',
                    {
                      'border-b border-gray-100 dark:border-gray-700/60':
                        index < group.brackets.length - 1,
                    },
                  ]"
                >
                  <div class="grow flex flex-col gap-1.5">
                    <!-- Name row -->
                    <div class="flex items-center justify-between">
                      <span class="font-medium text-base text-gray-800 dark:text-gray-100">
                        {{ bracket.name || 'Untitled' }}
                      </span>
                      <IconRocket :size="30" :stroke-width="1.5" class="text-gray-600 dark:text-gray-400 ml-3" />
                    </div>
                    <!-- Game and badges row -->
                    <div class="flex flex-wrap items-center gap-2">
                      <span class="text-sm text-gray-600 dark:text-gray-400">{{ bracket.game }}</span>
                      <span :class="getTypeBadgeClass(bracket.type)">
                        {{ formatBracketType(bracket.type) }}
                      </span>
                    </div>
                  </div>
                </div>
              </router-link>
            </li>
          </ul>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { reportsService } from '@/services/reportsService';
import { type RecentActivityGroup } from '@/models/RecentActivity';
import { BracketStatus } from '@/models/BracketStatus';
import { IconRocket } from '@tabler/icons-vue';

const activityGroups = ref<RecentActivityGroup[]>([]);
const loading = ref(true);
const error = ref<string | null>(null);

const formatStatusLabel = (status: BracketStatus): string => {
  switch (status) {
    case BracketStatus.Setup:
      return 'Setup';
    case BracketStatus.Started:
      return 'In Progress';
    case BracketStatus.OnHold:
      return 'On Hold';
    case BracketStatus.Completed:
      return 'Completed';
    default:
      return status;
  }
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

const getStatusColor = (status: BracketStatus): string => {
  switch (status) {
    case BracketStatus.Setup:
      return 'bg-sky-500';
    case BracketStatus.Started:
      return 'bg-green-500';
    case BracketStatus.OnHold:
      return 'bg-yellow-500';
    case BracketStatus.Completed:
      return 'bg-violet-500';
    default:
      return 'bg-gray-500';
  }
};

const fetchRecentActivity = async () => {
  try {
    loading.value = true;
    error.value = null;
    activityGroups.value = await reportsService.getRecentActivity();
  } catch (err) {
    error.value = err instanceof Error ? err.message : 'Failed to load recent activity';
    console.error('Failed to fetch recent activity:', err);
  } finally {
    loading.value = false;
  }
};

onMounted(() => {
  fetchRecentActivity();
});
</script>
