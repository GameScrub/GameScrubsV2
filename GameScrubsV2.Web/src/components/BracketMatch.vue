<template>
  <div class="match bg-white dark:bg-gray-800 shadow-xs rounded-xl pl-2 pr-2">
    <!-- Body -->
    <div class="m-0">
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
</template>

<script setup lang="ts">
import { type BracketPlacement } from '@/models/BracketPlacement';
import { PlacementStatus } from '@/models/PlacementStatus';

interface Props {
  player1: BracketPlacement | null;
  player2: BracketPlacement | null;
  showScores?: boolean;
}

const props = withDefaults(defineProps<Props>(), {
  showScores: false,
});

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
  /* background: #f0fdf4;
  border-color: #22c55e;
  font-weight: 600;*/

  @apply text-sm text-green-500;
}

.player.loser {
  /* background: #fef2f2;
  border-color: #ef4444;
  opacity: 0.7; */
  @apply text-sm text-red-500;
}

.player.empty {
  /* background: #f9fafb;
  border-style: dashed;
  opacity: 0.5; */
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
