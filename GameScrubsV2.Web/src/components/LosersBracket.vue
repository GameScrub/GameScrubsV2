<template>
  <div class="losers-bracket" v-if="rounds.length">
    <div class="sm:flex sm:justify-between sm:items-center mb-8">
      <div class="mb-4 sm:mb-0">
        <h1 class="text-2xl md:text-3xl text-gray-800 dark:text-gray-100 font-bold">
          Losers Bracket
        </h1>
      </div>
    </div>

    <div class="rounds">
      <div
        v-for="(round, roundIndex) in rounds"
        :key="`losers-${roundIndex}`"
        class="round"
        :data-round="roundIndex + 1"
      >
        <h1 class="text-base md:text-base text-gray-800 dark:text-gray-100 font-bold mb-2">
          Round {{ roundIndex + 1 }}
        </h1>
        <div class="matches">
          <BracketMatch
            v-for="match in round"
            :key="match.id"
            :player1="match.player1Data"
            :player2="match.player2Data"
            :show-scores="false"
          />
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import BracketMatch from '@/components/BracketMatch.vue';
import type { BracketPlacement } from '@/models/BracketPlacement';

interface MatchWithData {
  id: number;
  player1Data: BracketPlacement | null;
  player2Data: BracketPlacement | null;
  round: number;
}

interface Props {
  rounds: MatchWithData[][];
}

defineProps<Props>();
</script>

<style scoped>
@import '../css/style.css' reference;

.losers-bracket {
  border-radius: 8px;
  padding: 2rem;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
}

.rounds {
  display: flex;
  gap: 4rem;
  position: relative;
  align-items: center;
}

.round {
  display: flex;
  flex-direction: column;
  gap: 1rem;
  min-width: 200px;
}

.matches {
  display: flex;
  flex-direction: column;
  justify-content: center;
  gap: 2rem;
  flex: 1;
}

.matches :deep(.match) {
  position: relative;
}

/* Customize losers bracket connectors differently */
.matches :deep(.match::after) {
  content: '';
  position: absolute;
  right: -2rem;
  top: 50%;
  width: 2rem;
  height: 2px;
  background: #dc2626; /* Different color for losers bracket */
  transform: translateY(-50%);
}

/* You can add different spacing/layout logic here for losers bracket */
</style>
