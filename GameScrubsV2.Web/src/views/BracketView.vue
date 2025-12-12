<template>
  <div class="flex">
    <Sidebar :sidebarOpen="sidebarOpen" @close-sidebar="sidebarOpen = false" />

    <div class="relative flex flex-col flex-1 overflow-y-auto overflow-x-hidden">
      <Header
        ref="headerRef"
        :sidebarOpen="sidebarOpen"
        :bracket-name="bracket?.name"
        :game="bracket?.game"
        :bracket-id="bracket?.id"
        :is-locked="bracket?.isLocked"
        @toggle-sidebar="sidebarOpen = !sidebarOpen"
        @show-scores="handleShowScores"
        @lock-code-change="handleLockCodeChange"
      />
      <main class="p-4">
        <div v-if="loading && placements.length === 0">Loading...</div>
        <div v-if="error" class="error">{{ error }}</div>

        <div v-if="!loading || placements.length > 0" class="inline-block">
          <div class="tournament-bracket">
            <div class="bracket-wrapper">
              <WinnersBracket :rounds="winnersRounds" :champion="champion" :lock-code="lockCode" />
              <LosersBracket :rounds="losersRounds" :lock-code="lockCode" />
            </div>
          </div>
        </div>
      </main>
    </div>

    <BracketScore ref="bracketScoreRef" :bracket-id="bracket?.id" />
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, computed, provide } from 'vue';
import { useRoute } from 'vue-router';
import { bracketPlacementService } from '@/services/bracketPlacementService';
import { bracketService } from '@/services/bracketService';

import Sidebar from '@/partials/Sidebar.vue';
import Header from '@/partials/Header.vue';
import WinnersBracket from '@/components/WinnersBracket.vue';
import LosersBracket from '@/components/LosersBracket.vue';
import BracketScore from '@/components/BracketScore.vue';

import { type BracketPlacement } from '@/models/BracketPlacement';
import { type BracketPosition } from '@/models/BracketPosition';
import { type Bracket } from '@/models/Bracket';

const route = useRoute();
const bracket = ref<Bracket>();
const placements = ref<BracketPlacement[]>([]);
const positions = ref<BracketPosition[]>([]);
const error = ref<string | null>(null);
const loading = ref(false);
const sidebarOpen = ref(false);
const isRefreshing = ref(false);
const bracketScoreRef = ref<InstanceType<typeof BracketScore>>();
const headerRef = ref<InstanceType<typeof Header>>();
const lockCode = ref<string>();

interface MatchWithData extends BracketPosition {
  player1Data: BracketPlacement | null;
  player2Data: BracketPlacement | null;
  round: number;
}

const winnersMatches = computed(() =>
  positions.value.filter(
    (match) => match.player1?.startsWith('w') && match.player2?.startsWith('w'),
  ),
);

const losersMatches = computed(() =>
  positions.value.filter(
    (match) => match.player1?.startsWith('l') || match.player2?.startsWith('l'),
  ),
);

const champion = computed(() => {
  const allRounds = buildRounds(winnersMatches.value);

  if (allRounds.length === 0) return null;

  const finalsMatch = allRounds[allRounds.length - 1]?.[0];

  if (!finalsMatch) return null;

  // Find the winner by looking at the winLocation
  const championPlace = finalsMatch.winLocation;
  if (!championPlace) return null;

  const winner = placements.value.find((p) => p.bracketPlace === championPlace) || null;

  return winner;
});

const winnersRounds = computed(() => {
  return buildRounds(winnersMatches.value);
});

const losersRounds = computed(() => {
  return buildRounds(losersMatches.value);
});

function buildRounds(matches: BracketPosition[]): MatchWithData[][] {
  if (matches.length === 0) return [];

  const rounds: Map<number, MatchWithData[]> = new Map();
  const matchByWinLocation = new Map<string | null, BracketPosition>();

  matches.forEach((match) => {
    matchByWinLocation.set(match.winLocation, match);
  });

  const assignRound = (match: BracketPosition, visited = new Set<number>()): number => {
    if (visited.has(match.id)) {
      return 0;
    }

    visited.add(match.id);

    const player1Match = matchByWinLocation.get(match.player1);
    const player2Match = matchByWinLocation.get(match.player2);

    if (!player1Match && !player2Match) {
      return 1;
    }

    const player1Round = player1Match ? assignRound(player1Match, visited) : 0;
    const player2Round = player2Match ? assignRound(player2Match, visited) : 0;

    return Math.max(player1Round, player2Round) + 1;
  };

  matches.forEach((match) => {
    const round = assignRound(match);

    const matchWithData: MatchWithData = {
      ...match,
      player1Data: placements.value.find((p) => p.bracketPlace === match.player1) || null,
      player2Data: placements.value.find((p) => p.bracketPlace === match.player2) || null,
      round,
    };

    if (!rounds.has(round)) {
      rounds.set(round, []);
    }
    rounds.get(round)!.push(matchWithData);
  });

  return Array.from(rounds.entries())
    .sort(([a], [b]) => a - b)
    .map(([_, matches]) => matches);
}

const loadData = async () => {
  loading.value = true;
  error.value = null;
  isRefreshing.value = true;
  try {
    const bracketId = Number(route.params.id);

    bracket.value = await bracketService.getById(bracketId);

    [placements.value, positions.value] = await Promise.all([
      bracketPlacementService.getBracketPlacements(bracketId),
      bracketPlacementService.getBrackePositions(bracketId),
    ]);
  } catch (err) {
    error.value = err instanceof Error ? err.message : 'An error occurred';
  } finally {
    loading.value = false;
    isRefreshing.value = false;
  }
};

const refreshData = async () => {
  isRefreshing.value = true;
  try {
    const bracketId = Number(route.params.id);

    bracket.value = await bracketService.getById(bracketId);

    [placements.value, positions.value] = await Promise.all([
      bracketPlacementService.getBracketPlacements(bracketId),
      bracketPlacementService.getBrackePositions(bracketId),
    ]);
  } catch (err) {
    error.value = err instanceof Error ? err.message : 'An error occurred';
  } finally {
    isRefreshing.value = false;
  }
};

const showLockCodeError = () => {
  headerRef.value?.showLockCodeError();
};

provide('refreshBracket', refreshData);
provide('showLockCodeError', showLockCodeError);

const handleShowScores = () => {
  bracketScoreRef.value?.showScores();
};

const handleLockCodeChange = (code: string) => {
  lockCode.value = code;
};

onMounted(() => {
  loadData();
});
</script>

<style scoped>
@import '../css/style.css' reference;

.tournament-bracket {
  padding: 2rem;
  overflow-x: auto;
}

.bracket-wrapper {
  display: inline-flex; /* Change to inline-flex */
  flex-direction: column;
  gap: 3rem;
  min-width: max-content;
}

.error {
  color: #ef4444;
  padding: 1rem;
  background: #fef2f2;
  border-radius: 4px;
  margin: 1rem 0;
}
</style>
