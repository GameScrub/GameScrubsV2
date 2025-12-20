<template>
  <div class="losers-bracket" v-if="rounds.length">
    <div class="sm:flex sm:justify-between sm:items-center mb-8">
      <div class="mb-4 sm:mb-0">
        <h1 class="text-2xl md:text-3xl text-gray-800 dark:text-gray-100 font-bold">
          Losers Bracket
        </h1>
      </div>
    </div>

    <div class="divide-y divide-gray-200 dark:divide-gray-700/60">
      <!-- Round Headers -->
      <div class="round-headers">
        <div
          v-for="(round, roundIndex) in rounds"
          :key="`header-${roundIndex}`"
          class="round-header"
        >
          <h1 class="text-base md:text-base text-gray-800 dark:text-gray-100 font-bold mb-2">
            Round {{ roundIndex + 1 }}
          </h1>
        </div>
      </div>

      <!-- Rounds with Matches -->
      <div class="rounds">
        <template v-for="(round, roundIndex) in rounds" :key="`losers-${roundIndex}`">
          <div class="round" :data-round="roundIndex + 1" :style="getRoundOffset(roundIndex)">
            <div class="matches">
              <template v-for="(match, matchIndex) in round" :key="match.id">
                <div class="match-wrapper">
                  <BracketMatch
                    :player1="match.player1Data"
                    :player2="match.player2Data"
                    :show-scores="false"
                    :bracket-status="bracketStatus"
                  />
                  <!-- Position marker for last match in losers finals (only for double elimination) -->
                  <PositionMarker
                    v-if="
                      isDoubleElimination &&
                      roundIndex === rounds.length - 1 &&
                      matchIndex === round.length - 1
                    "
                    :number="1"
                    position="right"
                    :vertical-position="50"
                    :connector-length="2"
                  />
                </div>
              </template>
            </div>
          </div>

          <!-- Connector space between rounds -->
          <div v-if="roundIndex < rounds.length - 1" class="connector-space">
            <div
              v-for="(_, connectorIndex) in getConnectorCount(roundIndex)"
              :key="`connector-${roundIndex}-${connectorIndex}`"
              class="horizontal-connector"
              :style="getConnectorStyle(roundIndex, connectorIndex)"
            ></div>
          </div>
        </template>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import BracketMatch from '@/components/BracketMatch.vue';
import PositionMarker from '@/components/PositionMarker.vue';
import type { BracketPlacement } from '@/models/BracketPlacement';

interface MatchWithData {
  id: number;
  player1Data: BracketPlacement | null;
  player2Data: BracketPlacement | null;
  round: number;
}

interface Props {
  rounds: MatchWithData[][];
  bracketStatus?: string;
  isDoubleElimination?: boolean;
}

const props = withDefaults(defineProps<Props>(), {
  isDoubleElimination: false,
});

const matchHeight = 6; // rem - half of match height is 3rem

// Calculate the gap between matches for a given round
function getMatchGap(roundIndex: number): number {
  const round = props.rounds[roundIndex];
  if (!round) return 2;

  // Round 0 always has 2rem gap
  if (roundIndex === 0) return 2;

  // For losers bracket, we need to calculate spacing iteratively
  // The pattern alternates: 2-to-1, 1-to-1, 2-to-1, 1-to-1, etc.
  let spacing = 2;

  for (let i = 1; i <= roundIndex; i++) {
    const currentRound = props.rounds[i];
    const prevRound = props.rounds[i - 1];

    if (!currentRound || !prevRound) continue;

    // Check if it's a 2-to-1 pattern
    if (prevRound.length === currentRound.length * 2) {
      // 2-to-1: matches spread out to align with junction points
      spacing = 2 * (matchHeight + spacing) - matchHeight;
    } else {
      // 1-to-1: spacing stays the same
      // No change to spacing
    }
  }

  return spacing;
}

function getRoundOffset(roundIndex: number) {
  // Round 0 (first round) stays at 0
  if (roundIndex === 0) {
    return { '--round-offset': '0rem', '--match-gap': '2rem' };
  }

  // Calculate offset dynamically based on previous round
  const offset = calculateDynamicRoundOffset(roundIndex);
  const gap = getMatchGap(roundIndex);
  return { '--round-offset': `${offset}rem`, '--match-gap': `${gap}rem` };
}

function calculateDynamicRoundOffset(roundIndex: number): number {
  if (roundIndex === 0) {
    return 0;
  }

  const prevRoundIndex = roundIndex - 1;
  const prevRound = props.rounds[prevRoundIndex];
  const currentRound = props.rounds[roundIndex];

  if (!prevRound || !currentRound) {
    return 1.5;
  }

  const prevRoundOffset = calculateDynamicRoundOffset(prevRoundIndex);
  const prevMatchGap = getMatchGap(prevRoundIndex); // Use dynamic gap calculation

  // Check if it's a 2-to-1 pattern (junction connector)
  if (prevRound.length === currentRound.length * 2) {
    // For larger brackets, offset to align with middle matches
    // baseMatchIndex determines which pair of matches from the previous round to use as reference
    let baseMatchIndex = 0;

    // Only offset for the very first 2-to-1 transition (Round 1→2)
    if (prevRoundIndex === 0) {
      if (prevRound.length >= 16) {
        // For 32-man brackets (16→8), skip first 2 matches
        baseMatchIndex = 2;
      } else if (prevRound.length >= 8) {
        // For 16-man brackets (8→4), skip first 1 match
        baseMatchIndex = 1;
      }
    } else {
      // For subsequent 2-to-1 transitions (e.g., Round 2→3, Round 4→5)
      // We need to offset by the match index that corresponds to this current match
      // For Round 3 Match 0: align with Round 2 Match 0
      // For Round 3 Match 1: align with Round 2 Match 2
      // This means baseMatchIndex should remain 0 for Match 0, but we need to handle each match separately
      // However, calculateDynamicRoundOffset calculates the FIRST match position
      // So for Round 3, we align the first match (Match 0) with Round 2's Match 0
      baseMatchIndex = 0;
    }

    const match1Top = prevRoundOffset + baseMatchIndex * (matchHeight + prevMatchGap);
    const match1Center = match1Top + matchHeight / 2;

    const match2Top = prevRoundOffset + (baseMatchIndex + 1) * (matchHeight + prevMatchGap);
    const match2Center = match2Top + matchHeight / 2;

    const junctionPosition = (match1Center + match2Center) / 2;

    // Align current round's first match player 1 position with junction
    return junctionPosition - 1.5;
  } else {
    // Direct 1-to-1 connection - align this round's player 1 position with previous round's match center
    // Previous round's first match center is at prevRoundOffset + matchHeight/2
    // This round's player 1 is at offset + 1.5
    // So: offset + 1.5 = prevRoundOffset + matchHeight/2
    // offset = prevRoundOffset + 3 - 1.5 = prevRoundOffset + 1.5
    return prevRoundOffset + 1.5;
  }
}

// Calculate how many horizontal connectors are needed for the connector-space
function getConnectorCount(roundIndex: number): number {
  // No connectors after the last round
  if (roundIndex >= props.rounds.length - 1) {
    return 0;
  }

  const currentRound = props.rounds[roundIndex];
  const nextRound = props.rounds[roundIndex + 1];

  if (!currentRound || !nextRound) {
    return 0;
  }

  // Check if it's a 2-to-1 pattern (needs junction connectors)
  // If current round has 2x matches as next round, we need junction connectors
  if (currentRound.length === nextRound.length * 2) {
    return nextRound.length; // One connector per next round match
  }

  // Otherwise it's a 1-to-1 direct connection (no junction connectors needed)
  return 0;
}

function getConnectorStyle(roundIndex: number, connectorIndex: number) {
  // Generic connector positioning for any round with 2-to-1 pattern
  const currentRoundOffset = calculateDynamicRoundOffset(roundIndex);
  const currentMatchGap = getMatchGap(roundIndex); // Use dynamic gap calculation

  // Get the two matches from current round that feed into this connector
  const match1Index = connectorIndex * 2;
  const match2Index = match1Index + 1;

  // Calculate absolute positions including current round's offset
  const match1Top = currentRoundOffset + match1Index * (matchHeight + currentMatchGap);
  const match1Center = match1Top + matchHeight / 2;

  const match2Top = currentRoundOffset + match2Index * (matchHeight + currentMatchGap);
  const match2Center = match2Top + matchHeight / 2;

  // Junction point is the midpoint between the two match centers
  const junctionPosition = (match1Center + match2Center) / 2;

  return {
    top: `${junctionPosition}rem`,
  };
}
</script>

<style scoped>
@import '../css/style.css' reference;

.losers-bracket {
  border-radius: 8px;
  padding: 2rem;
  padding-right: 4rem; /* Extra padding to contain position marker */
  box-shadow: 0 2px 8px rgb(90, 120, 250);
  overflow: visible;
}

.round-headers {
  display: flex;
  gap: 4rem;
  margin-bottom: 1rem;
}

.round-header {
  min-width: 200px;
}

.rounds {
  display: flex;
  gap: 0;
  position: relative;
  align-items: flex-start;
  overflow: visible; /* Ensure position markers don't cause scroll */
}

.round {
  display: flex;
  flex-direction: column;
  gap: 1rem;
  min-width: 200px;
  padding-top: var(--round-offset, 0);
}

.matches {
  display: flex;
  flex-direction: column;
  justify-content: center;
  gap: var(--match-gap, 2rem);
  flex: 1;
}

.match-wrapper {
  position: relative;
  overflow: visible; /* Allow position markers to extend outside */
}

.matches :deep(.match) {
  position: relative;
}

/* Horizontal connectors - default for 1-to-1 connections */
.matches :deep(.match::after) {
  content: '';
  position: absolute;
  right: -4rem;
  top: 50%;
  width: 4rem;
  height: 2px;
  background: #dc2626; /* Different color for losers bracket */
  transform: translateY(-50%);
}

/* Hide connector on last round */
.round:last-of-type .matches :deep(.match::after) {
  display: none;
}

/* Vertical connectors for Round 2 - standard bracket pattern */
.round[data-round='2'] .matches .match-wrapper:nth-child(odd) :deep(.match::before) {
  content: '';
  position: absolute;
  right: -2rem;
  top: 50%;
  width: 2px;
  height: calc(100% + var(--match-gap, 2rem));
  background: #dc2626;
}

.round[data-round='2'] .matches .match-wrapper:nth-child(even) :deep(.match::before) {
  content: '';
  position: absolute;
  right: -2rem;
  bottom: 50%;
  width: 2px;
  height: calc(100% + var(--match-gap, 2rem));
  background: #dc2626;
}

/* Round 2: Shorten horizontal connectors (2-to-1 pattern with junction) */
.round[data-round='2'] .matches :deep(.match::after) {
  width: 2rem;
  right: -2rem;
}

/* For any other round with junction connectors, shorten the horizontal connectors */
/* This uses :has(+ .connector-space .horizontal-connector) to check ONLY the immediate next connector-space */
.round[data-round='3']:has(+ .connector-space .horizontal-connector) .matches :deep(.match::after),
.round[data-round='4']:has(+ .connector-space .horizontal-connector) .matches :deep(.match::after),
.round[data-round='5']:has(+ .connector-space .horizontal-connector) .matches :deep(.match::after),
.round[data-round='6']:has(+ .connector-space .horizontal-connector) .matches :deep(.match::after) {
  width: 2rem;
  right: -2rem;
}

/* Vertical connectors for rounds with 2-to-1 pattern */
/* Only add vertical connectors if the immediate next connector-space has junction connectors */
.round[data-round='3']:has(+ .connector-space .horizontal-connector)
  .matches
  .match-wrapper:nth-child(odd)
  :deep(.match::before),
.round[data-round='4']:has(+ .connector-space .horizontal-connector)
  .matches
  .match-wrapper:nth-child(odd)
  :deep(.match::before),
.round[data-round='5']:has(+ .connector-space .horizontal-connector)
  .matches
  .match-wrapper:nth-child(odd)
  :deep(.match::before),
.round[data-round='6']:has(+ .connector-space .horizontal-connector)
  .matches
  .match-wrapper:nth-child(odd)
  :deep(.match::before) {
  content: '';
  position: absolute;
  right: -2rem;
  top: 50%;
  width: 2px;
  height: calc(100% + var(--match-gap, 2rem));
  background: #dc2626;
}

.round[data-round='3']:has(+ .connector-space .horizontal-connector)
  .matches
  .match-wrapper:nth-child(even)
  :deep(.match::before),
.round[data-round='4']:has(+ .connector-space .horizontal-connector)
  .matches
  .match-wrapper:nth-child(even)
  :deep(.match::before),
.round[data-round='5']:has(+ .connector-space .horizontal-connector)
  .matches
  .match-wrapper:nth-child(even)
  :deep(.match::before),
.round[data-round='6']:has(+ .connector-space .horizontal-connector)
  .matches
  .match-wrapper:nth-child(even)
  :deep(.match::before) {
  content: '';
  position: absolute;
  right: -2rem;
  bottom: 50%;
  width: 2px;
  height: calc(100% + var(--match-gap, 2rem));
  background: #dc2626;
}

/* Connector space between rounds */
.connector-space {
  width: 4rem;
  position: relative;
  flex-shrink: 0;
}

/* Horizontal connectors at junction points */
.horizontal-connector {
  position: absolute;
  width: 2rem;
  height: 2px;
  left: 2rem;
  background: #dc2626;
}
</style>
