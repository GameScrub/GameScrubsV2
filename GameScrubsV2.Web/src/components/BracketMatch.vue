<template>
  <div class="match bg-white dark:bg-gray-800 shadow-xs rounded-xl pl-2 pr-2" :class="{ 'completed': bracketStatus === 'Completed' }">
    <!-- Body -->
    <div class="m-0" @click.stop="openModal">
      <!-- Content -->
      <div class="text-sm divide-y divide-gray-200 dark:divide-gray-700/60">
        <div v-if="player1" class="player" :class="getPlayerClass(player1)">
          <span class="player-name">{{ player1.playerName }}</span>
          <span v-if="showScores" class="score">{{ player1.score }}</span>
        </div>
        <div v-else class="player empty">
          <span class="player-name">TBD</span>
        </div>

        <div v-if="player2" class="player" :class="getPlayerClass(player2)">
          <span class="player-name">{{ player2.playerName }}</span>
          <span v-if="showScores" class="score">{{ player2.score }}</span>
        </div>
        <div v-else class="player empty">
          <span class="player-name">TBD</span>
        </div>
      </div>
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
        <div class="flex items-center justify-between mb-4">
          <h2 class="text-xl font-semibold text-gray-900 dark:text-gray-100">Who won?</h2>
          <button
            @click="closeModal"
            class="text-gray-400 hover:text-gray-600 dark:hover:text-gray-200"
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
        <div class="mb-6 flex gap-3">
          <button
            @click="setWinner(player1)"
            class="px-4 py-2 bg-blue-600 text-white rounded hover:bg-blue-700"
          >
            {{ player1.playerName }}
          </button>
          <button
            @click="setWinner(player2)"
            class="px-4 py-2 bg-blue-600 text-white rounded hover:bg-blue-700"
          >
            {{ player2.playerName }}
          </button>
        </div>

        <!-- Modal footer -->
        <div class="flex justify-end gap-3">
          <button
            @click="closeModal"
            class="px-4 py-2 bg-gray-200 text-gray-800 rounded hover:bg-gray-300"
          >
            Cancel
          </button>
        </div>
      </div>
    </ModalEmpty>
  </Teleport>
</template>

<script setup lang="ts">
import { ref, inject } from 'vue';
import { bracketPlacementService } from '@/services/bracketPlacementService';
import { type BracketPlacement } from '@/models/BracketPlacement';
import { PlacementStatus } from '@/models/PlacementStatus';
import ModalEmpty from '@/components/ModalEmpty.vue';
import type { useNotification } from '@/composables/useNotification';

interface Props {
  player1: BracketPlacement | null;
  player2: BracketPlacement | null;
  showScores?: boolean;
  lockCode?: string;
  bracketStatus?: string;
}

const notification = inject<ReturnType<typeof useNotification>>('notification');
const refreshBracket = inject<() => Promise<void>>('refreshBracket');
const showLockCodeError = inject<() => void>('showLockCodeError');

const props = withDefaults(defineProps<Props>(), {
  showScores: false,
});

const isModalOpen = ref(false);

const openModal = () => {
  // Don't open modal if bracket is completed
  if (props.bracketStatus === 'Completed') {
    return;
  }
  isModalOpen.value = true;
};

const closeModal = () => {
  isModalOpen.value = false;
};

const setWinner = async (player: BracketPlacement) => {
  try {
    await bracketPlacementService.setBracketPlacement(player.bracketId, player.id, props.lockCode);
    notification?.success('Bracket scores updated');
    closeModal();

    if (refreshBracket) {
      await refreshBracket();
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
    empty: player.playerName === '--',
  };
}
</script>

<style scoped>
@import '../css/style.css' reference;

.match {
  cursor: pointer;
  transition: background-color 0.2s ease;
}

.match:hover {
  box-shadow: 0 2px 8px hwb(123 11% 33%);
}

.match.completed {
  cursor: default;
}

.match.completed:hover {
  box-shadow: 0 1px 3px 0 rgba(0, 0, 0, 0.1), 0 1px 2px 0 rgba(0, 0, 0, 0.06);
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
  @apply text-sm text-green-500;
}

.player.loser {
  @apply text-sm text-red-500;
}

.score {
  font-weight: 600;
  color: #6b7280;
  margin-left: 1rem;
  min-width: 2rem;
  text-align: right;
}

.winner .score {
  color: #22c55e;
}

.loser .score {
  color: #ef4444;
}
</style>
