<template>
  <div
    class="match bg-white dark:bg-gray-800 shadow-xs rounded-xl pl-2 pr-2"
    :class="{ completed: bracketStatus === BracketStatus.Completed }"
  >
    <!-- Body -->
    <div class="m-0" @click.stop="openModal">
      <!-- Content -->
      <div class="text-sm divide-y divide-gray-200 dark:divide-gray-700/60">
        <div v-if="player1" class="player" :class="getPlayerClass(player1)">
          <span class="player-name">{{ player1.playerName }}</span>
          <span v-if="showScores" class="score">{{ player1.score }}</span>
        </div>
        <div v-else class="player empty">
          <span class="player-name">{{ PlayerPlaceholder.TBD }}</span>
        </div>

        <div v-if="player2" class="player" :class="getPlayerClass(player2)">
          <span class="player-name">{{ player2.playerName }}</span>
          <span v-if="showScores" class="score">{{ player2.score }}</span>
        </div>
        <div v-else class="player empty">
          <span class="player-name">{{ PlayerPlaceholder.TBD }}</span>
        </div>
      </div>
    </div>

    <!-- Position Marker -->
    <div v-if="positionMarker || markerIcon" class="position-marker">
      <component v-if="markerIcon" :is="markerIcon" :size="16" :stroke-width="2" />
      <span v-else>{{ toRoman(positionMarker!) }}</span>
    </div>
  </div>

  <!-- Options Modal -->
  <Teleport to="body">
    <ModalEmpty
      v-if="player1 && player2"
      id="bracket-match-options-modal"
      :modal-open="isModalOpen"
      @close-modal="closeModal"
    >
      <div class="p-6">
        <!-- Modal header -->
        <div class="flex items-center justify-between mb-6">
          <h2 class="text-2xl font-bold text-gray-900 dark:text-gray-100">Select Winner</h2>
          <button
            @click="closeModal"
            class="text-gray-400 hover:text-gray-600 dark:hover:text-gray-200 transition-colors"
          >
            <svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path
                stroke-linecap="round"
                stroke-linejoin="round"
                stroke-width="2"
                d="M6 18L18 6M6 6l12 12"
              />
            </svg>
          </button>
        </div>

        <!-- Modal body -->
        <div class="mb-6 flex items-center gap-4">
          <button
            @click="setWinner(player1)"
            class="flex-1 px-6 py-4 bg-gradient-to-r from-blue-500 to-blue-600 hover:from-blue-600 hover:to-blue-700 text-white rounded-lg font-semibold text-lg shadow-md hover:shadow-lg transition-all duration-200 transform hover:scale-[1.02] flex items-center justify-center group"
          >
            <span class="truncate">{{ player1.playerName }}</span>
            <svg
              class="w-6 h-6 ml-2 opacity-0 group-hover:opacity-100 transition-opacity flex-shrink-0"
              fill="none"
              stroke="currentColor"
              viewBox="0 0 24 24"
            >
              <path
                stroke-linecap="round"
                stroke-linejoin="round"
                stroke-width="2"
                d="M5 13l4 4L19 7"
              />
            </svg>
          </button>

          <!-- VS Icon/Text -->
          <div class="flex-shrink-0 text-gray-400 dark:text-gray-500 font-bold text-xl">VS</div>

          <button
            @click="setWinner(player2)"
            class="flex-1 px-6 py-4 bg-gradient-to-r from-blue-500 to-blue-600 hover:from-blue-600 hover:to-blue-700 text-white rounded-lg font-semibold text-lg shadow-md hover:shadow-lg transition-all duration-200 transform hover:scale-[1.02] flex items-center justify-center group"
          >
            <span class="truncate">{{ player2.playerName }}</span>
            <svg
              class="w-6 h-6 ml-2 opacity-0 group-hover:opacity-100 transition-opacity flex-shrink-0"
              fill="none"
              stroke="currentColor"
              viewBox="0 0 24 24"
            >
              <path
                stroke-linecap="round"
                stroke-linejoin="round"
                stroke-width="2"
                d="M5 13l4 4L19 7"
              />
            </svg>
          </button>
        </div>

        <!-- Modal footer -->
        <div class="flex justify-center">
          <button
            @click="closeModal"
            class="px-6 py-2 bg-gray-200 dark:bg-gray-700 text-gray-800 dark:text-gray-200 rounded-lg hover:bg-gray-300 dark:hover:bg-gray-600 transition-colors font-medium"
          >
            Cancel
          </button>
        </div>
      </div>
    </ModalEmpty>
  </Teleport>
</template>

<script setup lang="ts">
import { ref, inject, computed } from 'vue';
import { bracketPlacementService } from '@/services/bracketPlacementService';
import { useBracketStore } from '@/stores/bracket';
import { type BracketPlacement } from '@/models/BracketPlacement';
import { PlacementStatus } from '@/models/PlacementStatus';
import { BracketStatus } from '@/models/BracketStatus';
import { PlayerPlaceholder } from '@/models/PlayerPlaceholder';
import ModalEmpty from '@/components/ModalEmpty.vue';
import type { useNotification } from '@/composables/useNotification';
import type { Component } from 'vue';

interface Props {
  player1: BracketPlacement | null;
  player2: BracketPlacement | null;
  showScores?: boolean;
  bracketStatus?: string;
  positionMarker?: number;
  isWinnersBracket?: boolean;
  markerIcon?: Component;
}

const notification = inject<ReturnType<typeof useNotification>>('notification');
const refreshBracket = inject<() => Promise<void>>('refreshBracket');
const showLockCodeError = inject<() => void>('showLockCodeError');
const checkAndPromptCompleteStatus = inject<() => void>('checkAndPromptCompleteStatus');
const bracketStore = useBracketStore();

const props = withDefaults(defineProps<Props>(), {
  showScores: false,
});

// Get lock code from store based on bracket ID
const lockCode = computed(() => {
  const bracketId = props.player1?.bracketId || props.player2?.bracketId;
  return bracketId ? bracketStore.getLockCode(bracketId) : undefined;
});

// Convert number to Roman numerals
function toRoman(num: number): string {
  if (num === 0) return '';

  const romanNumerals = [
    { value: 50, numeral: 'L' },
    { value: 40, numeral: 'XL' },
    { value: 10, numeral: 'X' },
    { value: 9, numeral: 'IX' },
    { value: 5, numeral: 'V' },
    { value: 4, numeral: 'IV' },
    { value: 1, numeral: 'I' },
  ];

  let result = '';
  for (const { value, numeral } of romanNumerals) {
    while (num >= value) {
      result += numeral;
      num -= value;
    }
  }
  return result;
}

const isModalOpen = ref(false);

const openModal = () => {
  // Don't open modal if bracket is in Setup status
  if (props.bracketStatus === BracketStatus.Setup) {
    notification?.error('Bracket status must be set to "Started" before selecting winners.');
    return;
  }

  // Don't open modal if bracket is completed
  if (props.bracketStatus === BracketStatus.Completed) {
    return;
  }

  // Don't open modal if one or both players are missing
  if (!props.player1 || !props.player2) {
    notification?.error('Previous match must be completed before selecting a winner.');
    return;
  }

  // Auto-select winner if one or both players are byes
  const player1IsBye = props.player1.playerName === PlayerPlaceholder.BYE;
  const player2IsBye = props.player2.playerName === PlayerPlaceholder.BYE;

  if (player1IsBye || player2IsBye) {
    // If both are byes, select player1
    if (player1IsBye && player2IsBye) {
      setWinner(props.player1);
      return;
    }

    // If only player2 is bye, select player1
    if (player2IsBye) {
      setWinner(props.player1);
      return;
    }

    // If only player1 is bye, select player2
    if (player1IsBye) {
      setWinner(props.player2);
      return;
    }
  }

  isModalOpen.value = true;
};

const closeModal = () => {
  isModalOpen.value = false;
};

const setWinner = async (player: BracketPlacement) => {
  try {
    await bracketPlacementService.setBracketPlacement(player.bracketId, player.id, lockCode.value);
    notification?.success('Bracket scores updated');
    closeModal();

    if (refreshBracket) {
      await refreshBracket();

      // Check if this resulted in a champion being determined
      if (checkAndPromptCompleteStatus) {
        checkAndPromptCompleteStatus();
      }
    }
  } catch (error) {
    const errorMessage = error instanceof Error ? error.message : 'Failed to update bracket';

    // Check if it's a lock code error
    if (errorMessage.toLowerCase().includes('lock code')) {
      closeModal();
      showLockCodeError?.();
    }

    notification?.error(errorMessage);
  }
};

function getPlayerClass(player: BracketPlacement) {
  const isWinner = player.status === PlacementStatus.Winner;
  const isLoser = player.status === PlacementStatus.Loser;

  return {
    winner: isWinner,
    loser: isLoser,
    empty: player.playerName === PlayerPlaceholder.BYE,
  };
}
</script>

<style scoped>
@import '../css/style.css' reference;

.match {
  position: relative;
  cursor: pointer;
  transition: background-color 0.2s ease;
}

.match:hover {
  box-shadow: 0 2px 8px #51a2ff;
}

.match.completed {
  cursor: default;
}

.match.completed:hover {
  box-shadow:
    0 1px 3px 0 rgba(0, 0, 0, 0.1),
    0 1px 2px 0 rgba(0, 0, 0, 0.06);
}

.player {
  display: flex;
  justify-content: space-between;
  align-items: center;
  transition: all 0.2s;
  min-height: 3rem;
}

.player-name {
  flex: 1;
  @apply text-sm;
}

.player:hover:not(.empty) {
  cursor: pointer;
}

.player.winner {
  @apply text-sm text-blue-300;
}

.player.loser {
  @apply text-sm text-shadow-amber-50;
  text-decoration: line-through;
}

.score {
  font-weight: 600;
  color: #6b7280;
  margin-left: 1rem;
  min-width: 2rem;
  text-align: right;
}

.winner .score {
  color: #51a2ff;
}

.loser .score {
  color: #ef4444;
}

.position-marker {
  position: absolute;
  right: 0.25rem;
  top: 50%;
  transform: translateY(-50%);
  min-width: 1.25rem;
  height: 1.25rem;
  padding: 0 0.25rem;
  display: flex;
  align-items: center;
  justify-content: center;
  background: #e5e7eb;
  border: 1px solid #d1d5db;
  border-radius: 0.625rem;
  font-size: 0.625rem;
  font-weight: 500;
  color: #6b7280;
  transition: all 0.2s ease;
  z-index: 5;
}

.match:hover .position-marker {
  background: #d1d5db;
  border-color: #9ca3af;
  color: #4b5563;
}

:global(.dark) .position-marker {
  background: #374151;
  border-color: #4b5563;
  color: #9ca3af;
}

:global(.dark) .match:hover .position-marker {
  background: #4b5563;
  border-color: #6b7280;
  color: #d1d5db;
}
</style>
