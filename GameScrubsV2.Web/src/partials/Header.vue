<template>
  <header
    class="sticky top-0 before:absolute before:inset-0 before:backdrop-blur-md max-lg:before:bg-white/90 dark:max-lg:before:bg-gray-800/90 before:-z-10 z-30"
    :class="[
      variant === 'v2' || variant === 'v3'
        ? 'before:bg-white after:absolute after:h-px after:inset-x-0 after:top-full after:bg-gray-200 dark:after:bg-gray-700/60 after:-z-10'
        : 'max-lg:shadow-xs lg:before:bg-gray-100/90 dark:lg:before:bg-gray-900/90',
      variant === 'v2' ? 'dark:before:bg-gray-800' : '',
      variant === 'v3' ? 'dark:before:bg-gray-900' : '',
    ]"
  >
    <div class="px-4 sm:px-6 lg:px-8">
      <div
        class="flex items-center justify-between h-16"
        :class="
          variant === 'v2' || variant === 'v3'
            ? ''
            : 'lg:border-b border-gray-200 dark:border-gray-700/60'
        "
      >
        <!-- Header: Left side -->
        <div class="flex items-center gap-4">
          <!-- Hamburger button -->
          <button
            class="text-gray-500 hover:text-gray-600 dark:hover:text-gray-400 lg:hidden"
            @click.stop="$emit('toggle-sidebar')"
            aria-controls="sidebar"
            :aria-expanded="sidebarOpen"
          >
            <span class="sr-only">Open sidebar</span>
            <svg
              class="w-6 h-6 fill-current"
              viewBox="0 0 24 24"
              xmlns="http://www.w3.org/2000/svg"
            >
              <rect x="4" y="5" width="16" height="2" />
              <rect x="4" y="11" width="16" height="2" />
              <rect x="4" y="17" width="16" height="2" />
            </svg>
          </button>

          <!-- Bracket info -->
          <div v-if="bracketName || game" class="flex flex-col">
            <div class="flex items-center gap-2">
              <h1 v-if="bracketName" class="text-lg font-semibold text-gray-800 dark:text-gray-100">
                {{ bracketName }}
              </h1>
              <button
                v-if="bracketStatus && canChangeStatus"
                @click.stop="handleStatusClick"
                :class="[getStatusBadgeClass(bracketStatus), 'cursor-pointer hover:opacity-80 transition-opacity']"
              >
                {{ bracketStatus }}
              </button>
              <div v-else-if="bracketStatus" :class="getStatusBadgeClass(bracketStatus)">
                {{ bracketStatus }}
              </div>
            </div>
            <p v-if="game" class="text-sm text-gray-500 dark:text-gray-400">
              {{ game }}
            </p>
          </div>
        </div>
        <!-- Header: Right side -->
        <div class="flex items-center space-x-3">
          <div class="flex items-center gap-2" v-if="bracketId && isLocked">
            <Tooltip position="left">
              <template #trigger>
                <IconLock :size="20" :stroke-width="1.5" class="text-gray-500 dark:text-gray-400" />
              </template>
              <div class="text-xs whitespace-nowrap">
                This bracket has a lock code. The code is required for this bracket.
              </div>
            </Tooltip>
            <div class="relative">
              <input
                ref="lockCodeInputRef"
                :type="showLockCode ? 'text' : 'password'"
                name="lockCode"
                v-model="lockCodeInput"
                @input="handleLockCodeChange"
                placeholder="Lock Code"
                :class="[
                  'px-3 py-1 pr-8 text-sm border rounded bg-white dark:bg-gray-800 text-gray-900 dark:text-gray-100 transition-all duration-200',
                  lockCodeError
                    ? 'border-red-500 shadow-[0_0_0_3px_rgba(239,68,68,0.3)] shake'
                    : 'border-gray-300 dark:border-gray-600',
                ]"
              />
              <button
                type="button"
                @click="showLockCode = !showLockCode"
                class="absolute right-2 top-1/2 -translate-y-1/2 text-gray-500 hover:text-gray-700 dark:text-gray-400 dark:hover:text-gray-200"
              >
                <IconEye v-if="!showLockCode" :size="16" :stroke-width="1.5" />
                <IconEyeOff v-else :size="16" :stroke-width="1.5" />
              </button>
            </div>
          </div>
          <!-- Manage Players Button -->
          <button
            v-if="bracketId && showManagePlayersButton"
            class="w-8 h-8 flex items-center justify-center hover:bg-gray-100 lg:hover:bg-gray-200 dark:hover:bg-gray-700/50 dark:lg:hover:bg-gray-800 rounded-full"
            @click="managePlayers"
          >
            <Tooltip position="left">
              <template #trigger>
                <IconUsers
                  :size="20"
                  :stroke-width="1.5"
                  class="text-gray-500 dark:text-gray-400"
                />
              </template>
              <div class="text-xs whitespace-nowrap">Manage Players</div>
            </Tooltip>
          </button>
          <!-- Edit Bracket Button -->
          <button
            v-if="bracketId && showEditButton"
            class="w-8 h-8 flex items-center justify-center hover:bg-gray-100 lg:hover:bg-gray-200 dark:hover:bg-gray-700/50 dark:lg:hover:bg-gray-800 rounded-full"
            @click="editBracket"
          >
            <Tooltip position="left">
              <template #trigger>
                <IconSettings
                  :size="20"
                  :stroke-width="1.5"
                  class="text-gray-500 dark:text-gray-400"
                />
              </template>
              <div class="text-xs whitespace-nowrap">Edit Bracket</div>
            </Tooltip>
          </button>
          <!-- Scoreboard Button -->
          <button
            v-if="bracketId && showScoreboardButton"
            class="w-8 h-8 flex items-center justify-center hover:bg-gray-100 lg:hover:bg-gray-200 dark:hover:bg-gray-700/50 dark:lg:hover:bg-gray-800 rounded-full"
            @click="showScores"
          >
            <Tooltip position="left">
              <template #trigger>
                <IconScoreboard
                  :size="20"
                  :stroke-width="1.5"
                  class="text-gray-500 dark:text-gray-400"
                />
              </template>
              <div class="text-xs whitespace-nowrap">Bracket Scores</div>
            </Tooltip>
          </button>
          <ThemeToggle />
        </div>
      </div>
    </div>
  </header>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import ThemeToggle from '../components/ThemeToggle.vue';
import { IconScoreboard, IconSettings, IconUsers, IconLock, IconEye, IconEyeOff } from '@tabler/icons-vue';
import Tooltip from '@/components/Tooltip.vue';

interface Props {
  sidebarOpen: boolean;
  variant?: 'v1' | 'v2' | 'v3';
  bracketName?: string;
  game?: string;
  bracketId?: number;
  isLocked?: boolean;
  bracketStatus?: string;
  canChangeStatus?: boolean;
  hasChampion?: boolean;
  showScoreboardButton?: boolean;
  showEditButton?: boolean;
  showManagePlayersButton?: boolean;
}

const props = withDefaults(defineProps<Props>(), {
  showScoreboardButton: true,
  showEditButton: false,
  showManagePlayersButton: false,
  canChangeStatus: false,
  hasChampion: false,
});

interface Emits {
  (e: 'toggle-sidebar'): void;
  (e: 'show-scores'): void;
  (e: 'edit-bracket'): void;
  (e: 'manage-players'): void;
  (e: 'change-status'): void;
  (e: 'lock-code-change', lockCode: string): void;
}

const emit = defineEmits<Emits>();

const lockCodeInput = ref<string>('');
const lockCodeError = ref<boolean>(false);
const lockCodeInputRef = ref<HTMLInputElement>();
const showLockCode = ref<boolean>(false);

const showScores = () => {
  emit('show-scores');
};

const editBracket = () => {
  emit('edit-bracket');
};

const managePlayers = () => {
  emit('manage-players');
};

const handleStatusClick = () => {
  emit('change-status');
};

const handleLockCodeChange = () => {
  lockCodeError.value = false; // Clear error when user types
  emit('lock-code-change', lockCodeInput.value);
};

const showLockCodeError = () => {
  lockCodeError.value = true;
  // Focus the input
  lockCodeInputRef.value?.focus();
  // Reset after animation completes
  setTimeout(() => {
    lockCodeError.value = false;
  }, 500);
};

const getStatusBadgeClass = (status: string) => {
  const baseClasses = 'text-xs px-2.5 py-1 rounded-full shadow-none';

  switch (status) {
    case 'Setup':
      return `${baseClasses} bg-blue-500/20 text-blue-600 dark:text-blue-400`;
    case 'Started':
      return `${baseClasses} bg-green-500/20 text-green-600 dark:text-green-400`;
    case 'OnHold':
      return `${baseClasses} bg-yellow-500/20 text-yellow-600 dark:text-yellow-400`;
    case 'Completed':
      return `${baseClasses} bg-gray-500/20 text-gray-600 dark:text-gray-400`;
    default:
      return `${baseClasses} bg-gray-500/20 text-gray-600 dark:text-gray-400`;
  }
};

defineExpose({ showLockCodeError });
</script>

<style scoped>
@keyframes shake {
  0%,
  100% {
    transform: translateX(0);
  }
  10%,
  30%,
  50%,
  70%,
  90% {
    transform: translateX(-4px);
  }
  20%,
  40%,
  60%,
  80% {
    transform: translateX(4px);
  }
}

.shake {
  animation: shake 0.5s ease-in-out;
}
</style>
