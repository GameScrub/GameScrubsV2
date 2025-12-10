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
            :class="{ 'is-last-round': roundIndex === rounds.length - 1 }"
            :style="getBracketMatchOffset(roundIndex)"
          >
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
                  <div class="text-sm">{{ champion?.playerName || 'TBD' }}</div>
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
import type { BracketPlacement } from '@/models/BracketPlacement';

interface MatchWithData {
  id: number;
  player1Data: BracketPlacement | null;
  player2Data: BracketPlacement | null;
  round: number;
}

interface Props {
  rounds: MatchWithData[][];
  champion: BracketPlacement | null;
}

const props = defineProps<Props>();
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
  const offset = calculateRoundOffset(roundIndex);
  return {
    '--round-offset': `${offset}rem`,
  };
}

function getConnectorOffset(roundIndex: number): number {
  return calculateRoundOffset(roundIndex);
}

function getChampionOffset() {
  const lastRoundIndex = props.rounds.length - 1;
  const lastRoundOffset = calculateRoundOffset(lastRoundIndex);

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
  box-shadow: 0 2px 8px hwb(123 11% 33%);
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
  background: hwb(123 11% 33%);
}

.matches {
  display: flex;
  flex-direction: column;
  flex: 1;
  position: relative;
}

.round[data-round='1'] .matches :deep(.match:not(:last-child)) {
  margin-bottom: 2rem;
}

.round[data-round='2'] .matches :deep(.match:not(:last-child)) {
  margin-bottom: calc(v-bind('getMatchSpacing(1)') * 1rem);
}

.round[data-round='3'] .matches :deep(.match:not(:last-child)) {
  margin-bottom: calc(v-bind('getMatchSpacing(2)') * 1rem);
}

.round[data-round='4'] .matches :deep(.match:not(:last-child)) {
  margin-bottom: calc(v-bind('getMatchSpacing(3)') * 1rem);
}

.round[data-round='5'] .matches :deep(.match:not(:last-child)) {
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
  @apply text-green-500 bg-white dark:bg-gray-800 shadow-xs rounded-xl p-4 pr-10 pl-10;
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
  background: hwb(123 11% 33%);
  transform: translateY(-50%);
}

/* Vertical connectors for Round 1 */
.round[data-round='1'] .matches :deep(.match:nth-child(odd)::before) {
  content: '';
  position: absolute;
  right: -2rem;
  top: 50%;
  width: 2px;
  height: calc(100% + 2rem);
  background: hwb(123 11% 33%);
}

.round[data-round='1'] .matches :deep(.match:nth-child(even)::before) {
  content: '';
  position: absolute;
  right: -2rem;
  bottom: 50%;
  width: 2px;
  height: calc(100% + 2rem);
  background: hwb(123 11% 33%);
}

/* Vertical connectors for Round 2 */
.round[data-round='2'] .matches :deep(.match:nth-child(odd)::before) {
  content: '';
  position: absolute;
  right: -2rem;
  top: 50%;
  width: 2px;
  height: calc(100% + v-bind('getMatchSpacing(1)') * 1rem);
  background: hwb(123 11% 33%);
}

.round[data-round='2'] .matches :deep(.match:nth-child(even)::before) {
  content: '';
  position: absolute;
  right: -2rem;
  bottom: 50%;
  width: 2px;
  height: calc(100% + v-bind('getMatchSpacing(1)') * 1rem);
  background: hwb(123 11% 33%);
}

/* Vertical connectors for Round 3 */
.round[data-round='3'] .matches :deep(.match:nth-child(odd)::before) {
  content: '';
  position: absolute;
  right: -2rem;
  top: 50%;
  width: 2px;
  height: calc(100% + v-bind('getMatchSpacing(2)') * 1rem);
  background: hwb(123 11% 33%);
}

.round[data-round='3'] .matches :deep(.match:nth-child(even)::before) {
  content: '';
  position: absolute;
  right: -2rem;
  bottom: 50%;
  width: 2px;
  height: calc(100% + v-bind('getMatchSpacing(2)') * 1rem);
  background: hwb(123 11% 33%);
}

/* Vertical connectors for Round 4 */
.round[data-round='4'] .matches :deep(.match:nth-child(odd)::before) {
  content: '';
  position: absolute;
  right: -2rem;
  top: 50%;
  width: 2px;
  height: calc(100% + v-bind('getMatchSpacing(3)') * 1rem);
  background: hwb(123 11% 33%);
}

.round[data-round='4'] .matches :deep(.match:nth-child(even)::before) {
  content: '';
  position: absolute;
  right: -2rem;
  bottom: 50%;
  width: 2px;
  height: calc(100% + v-bind('getMatchSpacing(3)') * 1rem);
  background: hwb(123 11% 33%);
}

/* Vertical connectors for Round 5 */
.round[data-round='5'] .matches :deep(.match:nth-child(odd)::before) {
  content: '';
  position: absolute;
  right: -2rem;
  top: 50%;
  width: 2px;
  height: calc(100% + v-bind('getMatchSpacing(4)') * 1rem);
  background: hwb(123 11% 33%);
}

.round[data-round='5'] .matches :deep(.match:nth-child(even)::before) {
  content: '';
  position: absolute;
  right: -2rem;
  bottom: 50%;
  width: 2px;
  height: calc(100% + v-bind('getMatchSpacing(4)') * 1rem);
  background: hwb(123 11% 33%);
}
/* Prevent vertical connectors on the last round before champion */
.round.is-last-round .matches :deep(.match::before) {
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
  background: hwb(123 11% 33%);
  transform: translateY(-50%);
  display: block !important; /* Override the earlier display: none */
}

/* Dynamic spacing for all rounds */
.round[data-round='1'] .matches :deep(.match:not(:last-child)) {
  margin-bottom: 2rem;
}

.round[data-round='2'] .matches :deep(.match:not(:last-child)) {
  margin-bottom: calc(v-bind('getMatchSpacing(1)') * 1rem);
}

.round[data-round='3'] .matches :deep(.match:not(:last-child)) {
  margin-bottom: calc(v-bind('getMatchSpacing(2)') * 1rem);
}

.round[data-round='4'] .matches :deep(.match:not(:last-child)) {
  margin-bottom: calc(v-bind('getMatchSpacing(3)') * 1rem);
}

.round[data-round='5'] .matches :deep(.match:not(:last-child)) {
  margin-bottom: calc(v-bind('getMatchSpacing(4)') * 1rem);
}

.round[data-round='0'] .matches :deep(.match:not(:last-child)) {
  margin-bottom: 2rem;
}

/* Round 6 spacing */
.round[data-round='6'] .matches :deep(.match:not(:last-child)) {
  margin-bottom: calc(v-bind('getMatchSpacing(5)') * 1rem);
}

/* Vertical connectors for Round 6 */
.round[data-round='6'] .matches :deep(.match:nth-child(odd)::before) {
  content: '';
  position: absolute;
  right: -2rem;
  top: 50%;
  width: 2px;
  height: calc(100% + v-bind('getMatchSpacing(5)') * 1rem);
  background: hwb(123 11% 33%);
}

.round[data-round='6'] .matches :deep(.match:nth-child(even)::before) {
  content: '';
  position: absolute;
  right: -2rem;
  bottom: 50%;
  width: 2px;
  height: calc(100% + v-bind('getMatchSpacing(5)') * 1rem);
  background: hwb(123 11% 33%);
}

/* Round 7 spacing (for future 64-player brackets) */
.round[data-round='7'] .matches :deep(.match:not(:last-child)) {
  margin-bottom: calc(v-bind('getMatchSpacing(6)') * 1rem);
}

/* Vertical connectors for Round 7 */
.round[data-round='7'] .matches :deep(.match:nth-child(odd)::before) {
  content: '';
  position: absolute;
  right: -2rem;
  top: 50%;
  width: 2px;
  height: calc(100% + v-bind('getMatchSpacing(6)') * 1rem);
  background: hwb(123 11% 33%);
}

.round[data-round='7'] .matches :deep(.match:nth-child(even)::before) {
  content: '';
  position: absolute;
  right: -2rem;
  bottom: 50%;
  width: 2px;
  height: calc(100% + v-bind('getMatchSpacing(6)') * 1rem);
  background: hwb(123 11% 33%);
}
</style>
