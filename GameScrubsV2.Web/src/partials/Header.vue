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
            <h1 v-if="bracketName" class="text-lg font-semibold text-gray-800 dark:text-gray-100">
              {{ bracketName }}
            </h1>
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
          <div>
            <button
              v-if="bracketId"
              class="w-8 h-8 flex items-center justify-center hover:bg-gray-100 lg:hover:bg-gray-200 dark:hover:bg-gray-700/50 dark:lg:hover:bg-gray-800 rounded-full ml-3"
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
          </div>
          <div>
            <button
              class="w-8 h-8 flex items-center justify-center hover:bg-gray-100 lg:hover:bg-gray-200 dark:hover:bg-gray-700/50 dark:lg:hover:bg-gray-800 rounded-full ml-3"
              :class="{ 'bg-gray-200 dark:bg-gray-800': searchModalOpen }"
              @click.stop="searchModalOpen = true"
              aria-controls="search-modal"
            >
              <span class="sr-only">Search</span>
              <IconSearch :size="20" :stroke-width="1.5" class="text-gray-500 dark:text-gray-400" />
            </button>
            <SearchModal
              id="search-modal"
              searchId="search"
              :modalOpen="searchModalOpen"
              @open-modal="searchModalOpen = true"
              @close-modal="searchModalOpen = false"
            />
          </div>
          <Notifications align="right" />
          <Help align="right" />
          <ThemeToggle />
          <!-- Divider -->
          <hr class="w-px h-6 bg-gray-200 dark:bg-gray-700/60 border-none" />
          <UserMenu align="right" />
        </div>
      </div>
    </div>
  </header>
</template>

<script setup lang="ts">
import { ref } from 'vue';
//import SearchModal from '../components/ModalSearch.vue'
//import Notifications from '../components/DropdownNotifications.vue'
//import Help from '../components/DropdownHelp.vue'
import ThemeToggle from '../components/ThemeToggle.vue';
import { IconScoreboard, IconSearch, IconLock, IconEye, IconEyeOff } from '@tabler/icons-vue';
//import UserMenu from '../components/DropdownProfile.vue'
import Tooltip from '@/components/Tooltip.vue';

interface Props {
  sidebarOpen: boolean;
  variant?: 'v1' | 'v2' | 'v3';
  bracketName?: string;
  game?: string;
  bracketId?: number;
  isLocked?: boolean;
}

defineProps<Props>();

interface Emits {
  (e: 'toggle-sidebar'): void;
  (e: 'show-scores'): void;
  (e: 'lock-code-change', lockCode: string): void;
}

const emit = defineEmits<Emits>();

const searchModalOpen = ref<boolean>(false);
const lockCodeInput = ref<string>('');
const lockCodeError = ref<boolean>(false);
const lockCodeInputRef = ref<HTMLInputElement>();
const showLockCode = ref<boolean>(false);

const showScores = () => {
  emit('show-scores');
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
