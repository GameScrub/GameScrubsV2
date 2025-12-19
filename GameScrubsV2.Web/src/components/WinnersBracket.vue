<template>
  <div class="winners-bracket">
    <div class="sm:flex sm:justify-between sm:items-center mb-8">
      <div class="mb-4 sm:mb-0">
        <h1 class="text-2xl md:text-3xl text-gray-800 dark:text-gray-100 font-bold">
          Winners Bracket
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

        <!-- Champion Header -->
        <div class="round-header">
          <h1 class="text-base md:text-base text-gray-800 dark:text-gray-100 font-bold mb-2">
            Champion
          </h1>
        </div>
      </div>

      <!-- Rounds with Matches -->
      <div class="rounds">
        <template v-for="(round, roundIndex) in rounds" :key="`winners-${roundIndex}`">
          <div
            class="round"
            :data-round="roundIndex + 1"
            :class="{
              'is-last-round': roundIndex === rounds.length - 1,
              'is-double-elim': isDoubleElimination,
            }"
            :style="getBracketMatchOffset(roundIndex)"
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
                  <!-- Position marker for first match in Round 4 (only for double elimination) -->
                  <PositionMarker
                    v-if="isDoubleElimination && roundIndex === 3 && matchIndex === 0"
                    :number="1"
                    position="left"
                    :vertical-position="75"
                    :connector-length="6"
                  />
                </div>
              </template>
            </div>
          </div>

          <!-- Horizontal Connectors between rounds -->
          <div
            v-if="roundIndex < rounds.length - 1"
            class="connector-space"
            :class="{ 'connector-space-final': roundIndex === rounds.length - 1 }"
          >
            <div
              v-for="(_, connectorIndex) in getConnectorCount(roundIndex)"
              :key="`connector-${roundIndex}-${connectorIndex}`"
              class="horizontal-connector"
              :style="getConnectorStyle(roundIndex, connectorIndex)"
            ></div>
          </div>
        </template>

        <!-- Champion Section -->
        <div class="round champion-round" :style="getChampionOffset()">
          <div class="matches">
            <div class="champion-container">
              <div class="">
                <div class="player champion">
                  <div class="text-sm">{{ champion?.playerName || PlayerPlaceholder.TBD }}</div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import BracketMatch from '@/components/BracketMatch.vue';
import PositionMarker from '@/components/PositionMarker.vue';
import type { BracketPlacement } from '@/models/BracketPlacement';
import { PlayerPlaceholder } from '@/models/PlayerPlaceholder';

interface MatchWithData {
  id: number;
  player1Data: BracketPlacement | null;
  player2Data: BracketPlacement | null;
  round: number;
}

interface Props {
  rounds: MatchWithData[][];
  champion: BracketPlacement | null;
  bracketStatus?: string;
  isDoubleElimination?: boolean;
}

const props = withDefaults(defineProps<Props>(), {
  isDoubleElimination: false,
});
const matchHeight = 6; // rem

// Calculate how many horizontal connectors are needed for this round
function getConnectorCount(roundIndex: number): number {
  // For the last round, we need exactly 1 connector to the champion
  if (roundIndex === props.rounds.length - 1) {
    return 1;
  }

  // For other rounds, return the number of matches in the next round
  if (roundIndex < props.rounds.length - 1) {
    const nextRound = props.rounds[roundIndex + 1];
    return nextRound ? nextRound.length : 0;
  }

  return 0;
}

function getConnectorStyle(roundIndex: number, connectorIndex: number) {
  // Special handling for double elimination: Round 3 to Round 4 connector
  if (props.isDoubleElimination && roundIndex === 2 && props.rounds.length - 1 === 3) {
    const round3Offset = calculateRoundOffset(2);
    const round3ConnectorPosition = round3Offset + matchHeight / 2;
    const verticalConnectorLength = 4;

    // Position horizontal connector to overlap with vertical line (no gap)
    // Fine-tuned to align perfectly
    return {
      top: `${round3ConnectorPosition + verticalConnectorLength - 0.125}rem`,
    };
  }

  // Special handling for last round to champion connector
  if (roundIndex === props.rounds.length - 1) {
    const lastRoundOffset = getConnectorOffset(roundIndex);

    // For a tournament with one final match, we just need its center
    if (connectorIndex === 0) {
      const matchTop = lastRoundOffset;
      const matchCenter = matchTop + matchHeight / 2;
      return {
        top: `${matchCenter}rem`,
      };
    }
  }

  const currentRoundOffset = getConnectorOffset(roundIndex);
  const matchSpacing = getMatchSpacing(roundIndex);

  // Get the two matches from the current round that feed into this connector
  const match1Index = connectorIndex * 2;
  const match2Index = match1Index + 1;

  // Calculate positions
  const match1Top = currentRoundOffset + match1Index * (matchHeight + matchSpacing);
  const match1Center = match1Top + matchHeight / 2;

  const match2Top = currentRoundOffset + match2Index * (matchHeight + matchSpacing);
  const match2Center = match2Top + matchHeight / 2;

  // Junction point is the midpoint between the two match centers
  const junctionPosition = (match1Center + match2Center) / 2;

  return {
    top: `${junctionPosition}rem`,
  };
}

function getMatchSpacing(roundIndex: number): number {
  if (roundIndex === 0) {
    return 2;
  }

  // Calculate spacing iteratively
  let spacing = 2;
  for (let i = 1; i <= roundIndex; i++) {
    spacing = 2 * (matchHeight + spacing) - matchHeight;
  }

  // Apply scaling for larger brackets to keep them manageable
  // Scale down progressively for rounds beyond the 3rd
  if (roundIndex > 3) {
    const scaleFactor = 0.7; // Reduce spacing for very large brackets
    return spacing * scaleFactor;
  }

  return spacing;
}

function calculateRoundOffset(roundIndex: number): number {
  if (roundIndex === 0) {
    return 0;
  }

  // Calculate all offsets iteratively from round 0 to roundIndex
  let currentOffset = 0;

  for (let i = 1; i <= roundIndex; i++) {
    const prevMatchSpacing = getMatchSpacing(i - 1);

    // Calculate the first two match centers from previous round
    const match1Top = currentOffset;
    const match1Center = match1Top + matchHeight / 2;

    const match2Top = currentOffset + (matchHeight + prevMatchSpacing);
    const match2Center = match2Top + matchHeight / 2;

    // The first connector midpoint is the average of these two centers
    const firstConnectorMidpoint = (match1Center + match2Center) / 2;

    // Position this round's first match so its center aligns with the connector midpoint
    currentOffset = firstConnectorMidpoint - matchHeight / 2;
  }

  return currentOffset;
}

function getBracketMatchOffset(roundIndex: number) {
  let offset = calculateRoundOffset(roundIndex);

  // For double elimination: align Round 4 with Round 3's connector position, then add padding
  if (props.isDoubleElimination && roundIndex === 3 && roundIndex === props.rounds.length - 1) {
    // Get Round 3's offset and calculate its center (where the connector is)
    const round3Offset = calculateRoundOffset(2);
    const round3ConnectorPosition = round3Offset + matchHeight / 2;

    // Position Round 4 accounting for:
    // - 4rem vertical connector from Round 3
    // - Align to player 1 position (25% from top of Round 4 match)
    // - Additional padding for position marker visibility
    const verticalConnectorLength = 4; // 4rem vertical line
    const paddingTop = 0; // No additional spacing needed with longer connector
    offset = round3ConnectorPosition + verticalConnectorLength - matchHeight * 0.25 + paddingTop;
  }

  return {
    '--round-offset': `${offset}rem`,
  };
}

function getConnectorOffset(roundIndex: number): number {
  let offset = calculateRoundOffset(roundIndex);

  // Apply the same double elimination adjustment as Round 4
  if (props.isDoubleElimination && roundIndex === 3 && roundIndex === props.rounds.length - 1) {
    const round3Offset = calculateRoundOffset(2);
    const round3ConnectorPosition = round3Offset + matchHeight / 2;
    const verticalConnectorLength = 4;
    const paddingTop = 0;
    offset = round3ConnectorPosition + verticalConnectorLength - matchHeight * 0.25 + paddingTop;
  }

  return offset;
}

function getChampionOffset() {
  const lastRoundIndex = props.rounds.length - 1;
  let lastRoundOffset = calculateRoundOffset(lastRoundIndex);

  // Apply the same double elimination adjustment as Round 4
  if (props.isDoubleElimination && lastRoundIndex === 3) {
    const round3Offset = calculateRoundOffset(2);
    const round3ConnectorPosition = round3Offset + matchHeight / 2;
    const verticalConnectorLength = 4;
    const paddingTop = 0;
    lastRoundOffset =
      round3ConnectorPosition + verticalConnectorLength - matchHeight * 0.25 + paddingTop;
  }

  const connectorPosition = lastRoundOffset + matchHeight / 2;
  const championContainerHeight = 8;
  const offset = connectorPosition - championContainerHeight / 2;

  return {
    '--round-offset': `${offset}rem`,
  };
}
</script>

<style scoped>
@import '../css/style.css' reference;

.winners-bracket {
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
  position: relative;
  padding-top: var(--round-offset, 0);
}

.connector-space {
  width: 4rem;
  position: relative;
  flex-shrink: 0;
}

.horizontal-connector {
  position: absolute;
  width: 2rem;
  height: 2px;
  left: 2rem;
  background: #51a2ff;
}

:global(.dark) .horizontal-connector {
  background: #818cf8;
}

.matches {
  display: flex;
  flex-direction: column;
  flex: 1;
  position: relative;
}

.match-wrapper {
  position: relative;
}

.round[data-round='1'] .matches .match-wrapper:not(:last-child) {
  margin-bottom: 2rem;
}

.round[data-round='2'] .matches .match-wrapper:not(:last-child) {
  margin-bottom: calc(v-bind('getMatchSpacing(1)') * 1rem);
}

.round[data-round='3'] .matches .match-wrapper:not(:last-child) {
  margin-bottom: calc(v-bind('getMatchSpacing(2)') * 1rem);
}

.round[data-round='4'] .matches .match-wrapper:not(:last-child) {
  margin-bottom: calc(v-bind('getMatchSpacing(3)') * 1rem);
}

.round[data-round='5'] .matches .match-wrapper:not(:last-child) {
  margin-bottom: calc(v-bind('getMatchSpacing(4)') * 1rem);
}

.round:last-child .matches :deep(.match::after) {
  display: none;
}

.champion-round {
  min-width: 250px;
  margin-left: 0;
  padding-top: var(--round-offset, 0);
}

.champion-container {
  display: flex;
  justify-content: center;
  align-items: center;
  min-height: 8rem;
}

.player.champion {
  @apply text-blue-400 bg-white dark:bg-gray-800 shadow-xs rounded-xl p-4 pr-10 pl-10;
  font-weight: 700;
}

.matches :deep(.match) {
  position: relative;
}

.matches :deep(.match::after) {
  content: '';
  position: absolute;
  right: -2rem;
  top: 50%;
  width: 2rem;
  height: 2px;
  background: #51a2ff;
  transform: translateY(-50%);
}

/* Vertical connectors for Round 1 */
.round[data-round='1'] .matches .match-wrapper:nth-child(odd) :deep(.match::before) {
  content: '';
  position: absolute;
  right: -2rem;
  top: 50%;
  width: 2px;
  height: calc(100% + 2rem);
  background: #51a2ff;
}

.round[data-round='1'] .matches .match-wrapper:nth-child(even) :deep(.match::before) {
  content: '';
  position: absolute;
  right: -2rem;
  bottom: 50%;
  width: 2px;
  height: calc(100% + 2rem);
  background: #51a2ff;
}

/* Vertical connectors for Round 2 */
.round[data-round='2'] .matches .match-wrapper:nth-child(odd) :deep(.match::before) {
  content: '';
  position: absolute;
  right: -2rem;
  top: 50%;
  width: 2px;
  height: calc(100% + v-bind('getMatchSpacing(1)') * 1rem);
  background: #51a2ff;
}

.round[data-round='2'] .matches .match-wrapper:nth-child(even) :deep(.match::before) {
  content: '';
  position: absolute;
  right: -2rem;
  bottom: 50%;
  width: 2px;
  height: calc(100% + v-bind('getMatchSpacing(1)') * 1rem);
  background: #51a2ff;
}

/* Vertical connectors for Round 3 */
.round[data-round='3'] .matches .match-wrapper:nth-child(odd) :deep(.match::before) {
  content: '';
  position: absolute;
  right: -2rem;
  top: 50%;
  width: 2px;
  height: calc(100% + v-bind('getMatchSpacing(2)') * 1rem);
  background: #51a2ff;
}

.round[data-round='3'] .matches .match-wrapper:nth-child(even) :deep(.match::before) {
  content: '';
  position: absolute;
  right: -2rem;
  bottom: 50%;
  width: 2px;
  height: calc(100% + v-bind('getMatchSpacing(2)') * 1rem);
  background: #51a2ff;
}

/* For double elimination: create vertical connector from Round 3 to Round 4 */
.round[data-round='3'].is-double-elim:has(~ .round[data-round='4'].is-last-round)
  .matches
  .match-wrapper:first-child
  :deep(.match::before) {
  height: 4rem !important; /* Vertical line extending to Round 4 player 1 area */
  width: 2px !important; /* Ensure width matches other connectors */
}

/* Hide other Round 3 connectors for double elimination finals */
.round[data-round='3'].is-double-elim:has(~ .round[data-round='4'].is-last-round)
  .matches
  .match-wrapper:nth-child(even)
  :deep(.match::before) {
  display: none !important;
}

/* Vertical connectors for Round 4 */
/* Skip the first match since it gets the position indicator instead */
.round[data-round='4']
  .matches
  .match-wrapper:nth-child(odd):not(:first-child)
  :deep(.match::before) {
  content: '';
  position: absolute;
  right: -2rem;
  top: 50%;
  width: 2px;
  height: calc(100% + v-bind('getMatchSpacing(3)') * 1rem);
  background: #51a2ff;
}

.round[data-round='4'] .matches .match-wrapper:nth-child(even) :deep(.match::before) {
  content: '';
  position: absolute;
  right: -2rem;
  bottom: 50%;
  width: 2px;
  height: calc(100% + v-bind('getMatchSpacing(3)') * 1rem);
  background: #51a2ff;
}

/* For double elimination, when Round 4 is the last round, hide vertical connectors entirely */
/* The last round only has one match, so no vertical connectors needed */
.round[data-round='4'].is-last-round
  .matches
  .match-wrapper:nth-child(odd):not(:first-child)
  :deep(.match::before) {
  display: none;
}

.round[data-round='4'].is-last-round .matches .match-wrapper:nth-child(even) :deep(.match::before) {
  display: none;
}

/* Vertical connectors for Round 5 */
.round[data-round='5'] .matches .match-wrapper:nth-child(odd) :deep(.match::before) {
  content: '';
  position: absolute;
  right: -2rem;
  top: 50%;
  width: 2px;
  height: calc(100% + v-bind('getMatchSpacing(4)') * 1rem);
  background: #51a2ff;
}

.round[data-round='5'] .matches .match-wrapper:nth-child(even) :deep(.match::before) {
  content: '';
  position: absolute;
  right: -2rem;
  bottom: 50%;
  width: 2px;
  height: calc(100% + v-bind('getMatchSpacing(4)') * 1rem);
  background: #51a2ff;
}
/* Prevent vertical connectors on the last round before champion */
.round.is-last-round .matches .match-wrapper :deep(.match::before) {
  display: none !important;
}

/* Extra specific rule for Round 4 when it's the last round */
.round[data-round='4'].is-last-round .matches .match-wrapper :deep(.match::before) {
  display: none !important;
}

/* Remove the ::after for all last rounds */
.round.is-last-round .matches :deep(.match::after) {
  content: '';
  position: absolute;
  right: -8rem;
  top: 50%;
  width: 8rem;
  height: 2px;
  background: #51a2ff;
  transform: translateY(-50%);
  display: block !important; /* Override the earlier display: none */
}

/* Dynamic spacing for all rounds - already handled above, removing duplicates */
.round[data-round='0'] .matches .match-wrapper:not(:last-child) {
  margin-bottom: 2rem;
}

/* Round 6 spacing */
.round[data-round='6'] .matches .match-wrapper:not(:last-child) {
  margin-bottom: calc(v-bind('getMatchSpacing(5)') * 1rem);
}

/* Round 7 spacing */
.round[data-round='7'] .matches .match-wrapper:not(:last-child) {
  margin-bottom: calc(v-bind('getMatchSpacing(6)') * 1rem);
}

/* Vertical connectors for Round 6 */
.round[data-round='6'] .matches .match-wrapper:nth-child(odd) :deep(.match::before) {
  content: '';
  position: absolute;
  right: -2rem;
  top: 50%;
  width: 2px;
  height: calc(100% + v-bind('getMatchSpacing(5)') * 1rem);
  background: #51a2ff;
}

.round[data-round='6'] .matches .match-wrapper:nth-child(even) :deep(.match::before) {
  content: '';
  position: absolute;
  right: -2rem;
  bottom: 50%;
  width: 2px;
  height: calc(100% + v-bind('getMatchSpacing(5)') * 1rem);
  background: #51a2ff;
}

/* Vertical connectors for Round 7 */
.round[data-round='7'] .matches .match-wrapper:nth-child(odd) :deep(.match::before) {
  content: '';
  position: absolute;
  right: -2rem;
  top: 50%;
  width: 2px;
  height: calc(100% + v-bind('getMatchSpacing(6)') * 1rem);
  background: #51a2ff;
}

.round[data-round='7'] .matches .match-wrapper:nth-child(even) :deep(.match::before) {
  content: '';
  position: absolute;
  right: -2rem;
  bottom: 50%;
  width: 2px;
  height: calc(100% + v-bind('getMatchSpacing(6)') * 1rem);
  background: #51a2ff;
}
</style>
