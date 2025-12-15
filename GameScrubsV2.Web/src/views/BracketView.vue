<template>
  <div class="flex">
    <Sidebar :sidebarOpen="sidebarOpen" @close-sidebar="sidebarOpen = false" />

    <div class="relative flex flex-col flex-1 overflow-y-auto overflow-x-hidden">
      <Header
        ref="headerRef"
        :sidebarOpen="sidebarOpen"
        :variant="HeaderVariant.Bracket"
        :bracket-name="bracket?.name"
        :game="bracket?.game"
        :bracket-id="bracket?.id"
        :is-locked="bracket?.isLocked"
        :bracket-status="bracket?.status"
        :can-change-status="canChangeStatus"
        :has-champion="!!champion"
        :show-manage-players-button="true"
        :show-edit-button="true"
        @toggle-sidebar="sidebarOpen = !sidebarOpen"
        @show-scores="handleShowScores"
        @change-status="handleChangeStatus"
      />
      <main class="p-4">
        <div v-if="loading && placements.length === 0">Loading...</div>
        <div v-if="error" class="error">{{ error }}</div>

        <div v-if="!loading || placements.length > 0" class="inline-block">
          <div class="tournament-bracket">
            <div class="bracket-wrapper">
              <WinnersBracket :rounds="winnersRounds" :champion="champion" :bracket-status="bracket?.status" />
              <LosersBracket :rounds="losersRounds" :bracket-status="bracket?.status" />
            </div>
          </div>
        </div>
      </main>
    </div>

    <BracketScore ref="bracketScoreRef" :bracket-id="bracket?.id" />

    <!-- Status Change Confirmation Modal -->
    <Teleport to="body">
      <ConfirmModal
        id="change-status-modal"
        :modal-open="showStatusConfirm"
        :title="statusChangeModalTitle"
        :message="statusChangeModalMessage"
        :confirm-text="statusChangeConfirmText"
        cancel-text="Cancel"
        @confirm="confirmStatusChange"
        @cancel="showStatusConfirm = false"
      />
    </Teleport>

    <!-- No Champion Error Modal -->
    <Teleport to="body">
      <ConfirmModal
        id="no-champion-error-modal"
        :modal-open="showNoChampionError"
        title="Cannot Complete Bracket"
        message="Cannot complete the bracket yet. A champion must be determined first."
        confirm-text="OK"
        :show-cancel="false"
        @confirm="showNoChampionError = false"
        @cancel="showNoChampionError = false"
      />
    </Teleport>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, computed, provide, inject } from 'vue';
import { useRoute } from 'vue-router';
import { bracketPlacementService } from '@/services/bracketPlacementService';
import { bracketService } from '@/services/bracketService';

import Sidebar from '@/partials/Sidebar.vue';
import Header from '@/partials/Header.vue';
import WinnersBracket from '@/components/WinnersBracket.vue';
import LosersBracket from '@/components/LosersBracket.vue';
import BracketScore from '@/components/BracketScore.vue';
import ConfirmModal from '@/components/ConfirmModal.vue';

import { type BracketPlacement } from '@/models/BracketPlacement';
import { type BracketPosition } from '@/models/BracketPosition';
import { type Bracket } from '@/models/Bracket';
import { HeaderVariant } from '@/models/HeaderVariant';
import type { useNotification } from '@/composables/useNotification';
import { useBracketStore } from '@/stores/bracket';

const route = useRoute();
const notification = inject<ReturnType<typeof useNotification>>('notification');
const bracketStore = useBracketStore();

const bracket = ref<Bracket>();
const placements = ref<BracketPlacement[]>([]);
const positions = ref<BracketPosition[]>([]);
const error = ref<string | null>(null);
const loading = ref(false);
const sidebarOpen = ref(false);
const isRefreshing = ref(false);
const bracketScoreRef = ref<InstanceType<typeof BracketScore>>();
const headerRef = ref<InstanceType<typeof Header>>();
const showStatusConfirm = ref(false);
const showNoChampionError = ref(false);
const nextStatus = ref<string>('');

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

const canChangeStatus = computed(() => {
  const status = bracket.value?.status;
  return status === 'Setup' || status === 'Started';
});

const statusChangeModalTitle = computed(() => {
  if (bracket.value?.status === 'Setup') {
    return 'Start Bracket';
  } else if (bracket.value?.status === 'Started') {
    return 'Complete Bracket';
  }
  return 'Change Status';
});

const statusChangeModalMessage = computed(() => {
  if (bracket.value?.status === 'Setup') {
    return 'Are you sure you want to start this bracket? Once started, the bracket structure cannot be modified.';
  } else if (bracket.value?.status === 'Started') {
    if (!champion.value) {
      return 'Cannot complete the bracket yet. A champion must be determined first.';
    }
    return `Are you sure you want to complete this bracket? The winner is ${champion.value.playerName}.`;
  }
  return '';
});

const statusChangeConfirmText = computed(() => {
  if (bracket.value?.status === 'Setup') {
    return 'Start Bracket';
  } else if (bracket.value?.status === 'Started') {
    return 'Complete Bracket';
  }
  return 'Confirm';
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

const handleChangeStatus = () => {
  if (!bracket.value) return;

  // Determine next status
  if (bracket.value.status === 'Setup') {
    nextStatus.value = 'Started';
    showStatusConfirm.value = true;
  } else if (bracket.value.status === 'Started') {
    if (!champion.value) {
      showNoChampionError.value = true;
      return;
    }
    nextStatus.value = 'Completed';
    showStatusConfirm.value = true;
  }
};

const confirmStatusChange = async () => {
  if (!bracket.value) return;

  showStatusConfirm.value = false;

  try {
    const bracketId = bracket.value.id;
    await bracketService.changeStatus(bracketId, nextStatus.value, bracketStore.getLockCode(bracketId));

    await loadData();

    notification?.success('Bracket status updated successfully!');
  } catch (err) {
    const errorMessage = err instanceof Error ? err.message : 'Failed to change bracket status';
    if (errorMessage.includes('lock code')) {
      headerRef.value?.showLockCodeError();
    }
    notification?.error(errorMessage);
  }
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
