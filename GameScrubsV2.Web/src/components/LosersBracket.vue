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
          <div
            class="round"
            :data-round="roundIndex + 1"
            :style="getRoundOffset(roundIndex)"
          >
            <div class="matches">
              <template v-for="(match, matchIndex) in round" :key="match.id">
                <div class="match-wrapper">
                  <BracketMatch
                    :player1="match.player1Data"
                    :player2="match.player2Data"
                    :show-scores="false"
                    :bracket-status="bracketStatus"
                  />
                  <!-- Position marker for last match in Round 4 (only for double elimination) -->
                  <PositionMarker
                    v-if="isDoubleElimination && roundIndex === 3 && matchIndex === round.length - 1"
                    :number="1"
                    position="right"
                    :vertical-position="50"
                    :connector-length="4"
                  />
                </div>
              </template>
            </div>
          </div>

          <!-- Connector space between rounds -->
          <div
            v-if="roundIndex < rounds.length - 1"
            class="connector-space"
          >
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

function getRoundOffset(roundIndex: number) {
  // Round 1 stays at 0
  if (roundIndex === 0) {
    return { '--round-offset': '0rem' };
  }
  // Round 2 gets offset down by 1.5rem so its player1 position aligns with Round 1 match centers
  if (roundIndex === 1) {
    return { '--round-offset': '1.5rem' };
  }
  // Round 3 needs to align with Round 2's vertical connectors
  if (roundIndex === 2) {
    const round2Offset = 1.5; // rem
    const matchSpacing = 2; // gap between matches in rem

    // Position of first match in Round 2
    const match1Top = round2Offset;
    const match1Center = match1Top + matchHeight / 2;

    // Position of second match in Round 2
    const match2Top = round2Offset + matchHeight + matchSpacing;
    const match2Center = match2Top + matchHeight / 2;

    // Junction point is midpoint between the two match centers
    const junctionPosition = (match1Center + match2Center) / 2;

    // Round 3 first match's player 1 position should align with the junction
    // Player 1 is at matchTop + 1.5rem, so matchTop = junctionPosition - 1.5rem
    const round3Offset = junctionPosition - 1.5;

    return { '--round-offset': `${round3Offset}rem` };
  }

  // Round 4+ handling
  if (roundIndex === 3) {
    // Check if Round 3 matches feed into Round 4 with 2-to-1 pattern
    const round3MatchCount = props.rounds[2]?.length || 0;
    const round4MatchCount = props.rounds[3]?.length || 0;

    // If Round 3 has 2x matches as Round 4, use junction pattern (like Round 2→3)
    if (round3MatchCount === round4MatchCount * 2) {
      const round3Offset = calculateRound3Offset();
      const matchSpacing = 2; // gap between matches in rem

      // Position of first match in Round 3
      const match1Top = round3Offset;
      const match1Center = match1Top + matchHeight / 2;

      // Position of second match in Round 3
      const match2Top = round3Offset + matchHeight + matchSpacing;
      const match2Center = match2Top + matchHeight / 2;

      // Junction point is midpoint between the two match centers
      const junctionPosition = (match1Center + match2Center) / 2;

      // Round 4 first match's player 1 position should align with the junction
      const round4Offset = junctionPosition - 1.5;

      return { '--round-offset': `${round4Offset}rem` };
    } else {
      // Direct 1-to-1 connection (like Round 1→2), use same offset as Round 3
      return { '--round-offset': `${calculateRound3Offset() + 1.5}rem` };
    }
  }

  return { '--round-offset': '1.5rem' };
}

function calculateRound3Offset(): number {
  const round2Offset = 1.5;
  const matchSpacing = 2;

  const match1Top = round2Offset;
  const match1Center = match1Top + matchHeight / 2;

  const match2Top = round2Offset + matchHeight + matchSpacing;
  const match2Center = match2Top + matchHeight / 2;

  const junctionPosition = (match1Center + match2Center) / 2;
  return junctionPosition - 1.5;
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
  // Round 2 to Round 3 connectors
  if (roundIndex === 1) {
    const round2Offset = 1.5; // rem - Round 2 starts at 1.5rem
    const matchSpacing = 2; // gap between matches in rem

    // Get the two matches from Round 2 that feed into this connector
    const match1Index = connectorIndex * 2;
    const match2Index = match1Index + 1;

    // Calculate absolute positions including Round 2's offset
    const match1Top = round2Offset + match1Index * (matchHeight + matchSpacing);
    const match1Center = match1Top + matchHeight / 2;

    const match2Top = round2Offset + match2Index * (matchHeight + matchSpacing);
    const match2Center = match2Top + matchHeight / 2;

    // Junction point is the midpoint between the two match centers
    const junctionPosition = (match1Center + match2Center) / 2;

    return {
      top: `${junctionPosition}rem`,
    };
  }

  // Round 3 to Round 4 connectors
  if (roundIndex === 2) {
    const round3Offset = calculateRound3Offset();
    const matchSpacing = 2; // gap between matches in rem

    // Get the two matches from Round 3 that feed into this connector
    const match1Index = connectorIndex * 2;
    const match2Index = match1Index + 1;

    // Calculate absolute positions including Round 3's offset
    const match1Top = round3Offset + match1Index * (matchHeight + matchSpacing);
    const match1Center = match1Top + matchHeight / 2;

    const match2Top = round3Offset + match2Index * (matchHeight + matchSpacing);
    const match2Center = match2Top + matchHeight / 2;

    // Junction point is the midpoint between the two match centers
    const junctionPosition = (match1Center + match2Center) / 2;

    return {
      top: `${junctionPosition}rem`,
    };
  }

  return { top: '0rem' };
}
</script>

<style scoped>
@import '../css/style.css' reference;

.losers-bracket {
  border-radius: 8px;
  padding: 2rem;
  box-shadow: 0 2px 8px rgb(90, 120, 250);
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
  gap: 2rem;
  flex: 1;
}

.match-wrapper {
  position: relative;
}

.matches :deep(.match) {
  position: relative;
}

/* Horizontal connectors - each Round 1 match connects to corresponding Round 2 match */
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

/* Hide horizontal connectors on Round 2 matches (they use the connector-space instead) */
.round[data-round='2'] .matches :deep(.match::after) {
  width: 2rem;
  right: -2rem;
}

/* Vertical connectors for Round 2 - standard bracket pattern */
.round[data-round='2'] .matches .match-wrapper:nth-child(odd) :deep(.match::before) {
  content: '';
  position: absolute;
  right: -2rem;
  top: 50%;
  width: 2px;
  height: calc(100% + 2rem);
  background: #dc2626;
}

.round[data-round='2'] .matches .match-wrapper:nth-child(even) :deep(.match::before) {
  content: '';
  position: absolute;
  right: -2rem;
  bottom: 50%;
  width: 2px;
  height: calc(100% + 2rem);
  background: #dc2626;
}

/* Adjust Round 3 horizontal connectors when there's a 2-to-1 pattern */
.round[data-round='3']:has(~ .connector-space .horizontal-connector) .matches :deep(.match::after) {
  width: 2rem;
  right: -2rem;
}

/* Vertical connectors for Round 3 - only when there's a 2-to-1 pattern */
.round[data-round='3']:has(~ .connector-space .horizontal-connector) .matches .match-wrapper:nth-child(odd) :deep(.match::before) {
  content: '';
  position: absolute;
  right: -2rem;
  top: 50%;
  width: 2px;
  height: calc(100% + 2rem);
  background: #dc2626;
}

.round[data-round='3']:has(~ .connector-space .horizontal-connector) .matches .match-wrapper:nth-child(even) :deep(.match::before) {
  content: '';
  position: absolute;
  right: -2rem;
  bottom: 50%;
  width: 2px;
  height: calc(100% + 2rem);
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
