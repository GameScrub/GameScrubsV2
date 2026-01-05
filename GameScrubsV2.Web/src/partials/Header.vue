<!-- eslint-disable vue/multi-word-component-names -->
<template>
  <header
    class="sticky top-0 before:absolute before:inset-0 before:backdrop-blur-md max-lg:before:bg-white/90 dark:max-lg:before:bg-gray-800/90 before:-z-10 z-30"
    :class="[
      variant === HeaderVariant.ManagePlayer
        ? 'before:bg-white after:absolute after:h-px after:inset-x-0 after:top-full after:bg-gray-200 dark:after:bg-gray-700/60 after:-z-10 dark:before:bg-gray-800'
        : 'max-lg:shadow-xs lg:before:bg-gray-100/90 dark:lg:before:bg-gray-900/90',
    ]"
  >
    <div class="px-4 sm:px-6 lg:px-8">
      <div
        class="flex items-center justify-between h-16"
        :class="
          variant === HeaderVariant.ManagePlayer
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
              <h1 v-if="bracketName" class="text-xl font-semibold text-gray-800 dark:text-gray-100">
                {{ bracketName }}
              </h1>
              <button
                v-if="bracketStatus && canChangeStatus"
                @click.stop="handleStatusClick"
                :class="[
                  getStatusBadgeClass(bracketStatus),
                  'cursor-pointer hover:opacity-80 transition-opacity',
                ]"
              >
                {{ bracketStatus }}
              </button>
              <div v-else-if="bracketStatus" :class="getStatusBadgeClass(bracketStatus)">
                {{ bracketStatus }}
              </div>
            </div>
            <p v-if="game" class="text-m font-medium text-gray-700 dark:text-gray-300">
              {{ game }}
            </p>
          </div>
        </div>
        <!-- Header: Right side -->
        <div class="flex items-center space-x-3">
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

          <ThemeToggle />

          <!-- Live Indicator -->
          <div v-if="bracketId && isSignalRConnected !== undefined" class="relative group">
            <div
              class="flex items-center gap-2 px-3 py-1.5 rounded-lg transition-all"
              :class="
                isSignalRConnected
                  ? 'bg-green-500/10 dark:bg-green-500/20 border border-green-500/30'
                  : 'bg-gray-500/10 dark:bg-gray-500/20 border border-gray-500/30'
              "
            >
              <div class="relative">
                <div
                  class="w-2 h-2 rounded-full"
                  :class="isSignalRConnected ? 'bg-green-500 animate-pulse' : 'bg-gray-400'"
                ></div>
                <div
                  v-if="isSignalRConnected"
                  class="absolute inset-0 w-2 h-2 bg-green-500 rounded-full animate-ping"
                ></div>
              </div>
              <span
                class="text-xs font-medium"
                :class="
                  isSignalRConnected
                    ? 'text-green-700 dark:text-green-400'
                    : 'text-gray-700 dark:text-gray-400'
                "
              >
                {{ isSignalRConnected ? 'Connected' : 'Offline' }}
              </span>
            </div>
            <!-- Tooltip -->
            <div
              class="absolute right-0 top-full mt-2 w-64 p-3 bg-gray-900 text-white text-xs rounded-lg shadow-lg opacity-0 invisible group-hover:opacity-100 group-hover:visible transition-all duration-200 z-50 pointer-events-none"
            >
              <p class="font-semibold mb-1">
                {{ isSignalRConnected ? 'Auto-Refresh Active' : 'Connection Lost' }}
              </p>
              <p class="text-gray-300">
                {{
                  isSignalRConnected
                    ? 'This bracket automatically updates when the tournament manager makes changes. No need to refresh your browser!'
                    : 'Real-time updates are currently unavailable. Please refresh the page to reconnect.'
                }}
              </p>
              <div class="absolute -top-1 right-4 w-2 h-2 bg-gray-900 transform rotate-45"></div>
            </div>
          </div>
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
        </div>
      </div>
    </div>
  </header>
</template>

<script setup lang="ts">
import { ref, watch } from 'vue';
import { useRouter } from 'vue-router';
import ThemeToggle from '../components/ThemeToggle.vue';
import {
  IconScoreboard,
  IconSettings,
  IconUsers,
  IconLock,
  IconEye,
  IconEyeOff,
} from '@tabler/icons-vue';
import Tooltip from '@/components/Tooltip.vue';
import { useBracketStore } from '@/stores/bracket';
import { HeaderVariant } from '@/models/HeaderVariant';
import { BracketStatus } from '@/models/BracketStatus';

interface Props {
  sidebarOpen: boolean;
  variant?: HeaderVariant;
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
  isSignalRConnected?: boolean;
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
  (e: 'change-status'): void;
}

const emit = defineEmits<Emits>();
const router = useRouter();
const bracketStore = useBracketStore();

const lockCodeInput = ref<string>('');
const lockCodeError = ref<boolean>(false);
const lockCodeInputRef = ref<HTMLInputElement>();
const showLockCode = ref<boolean>(false);

// Restore lock code from store whenever bracketId changes
watch(
  () => props.bracketId,
  (newBracketId) => {
    if (newBracketId) {
      const storedLockCode = bracketStore.getLockCode(newBracketId);
      if (storedLockCode) {
        lockCodeInput.value = storedLockCode;
      } else {
        lockCodeInput.value = '';
      }
    } else {
      lockCodeInput.value = '';
    }
  },
  { immediate: true },
);

const showScores = () => {
  emit('show-scores');
};

const editBracket = () => {
  if (props.bracketId) {
    router.push({ name: 'bracket-edit', params: { id: props.bracketId } });
  }
};

const managePlayers = () => {
  if (props.bracketId) {
    router.push({ name: 'bracket-manage-players', params: { id: props.bracketId } });
  }
};

const handleStatusClick = () => {
  emit('change-status');
};

const handleLockCodeChange = () => {
  lockCodeError.value = false;

  if (props.bracketId && lockCodeInput.value) {
    bracketStore.setLockCode(props.bracketId, lockCodeInput.value);
  }
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
    case BracketStatus.Setup:
      return `${baseClasses} bg-blue-500/20 text-blue-600 dark:text-blue-400`;
    case BracketStatus.Started:
      return `${baseClasses} bg-green-500/20 text-green-600 dark:text-green-400`;
    case BracketStatus.OnHold:
      return `${baseClasses} bg-yellow-500/20 text-yellow-600 dark:text-yellow-400`;
    case BracketStatus.Completed:
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
