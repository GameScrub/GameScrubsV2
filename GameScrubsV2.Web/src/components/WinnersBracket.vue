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
              'is-semi-finals': isDoubleElimination && roundIndex === rounds.length - 2,
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
                    :position-marker="getPositionMarker(match)"
                    :marker-icon="showTrophyIcon(match, roundIndex, matchIndex) ? IconTrophyFilled : undefined"
                    :is-winners-bracket="true"
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
import type { BracketPlacement } from '@/models/BracketPlacement';
import { PlayerPlaceholder } from '@/models/PlayerPlaceholder';
import { IconTrophyFilled } from '@tabler/icons-vue';

interface MatchWithData {
  id: number;
  player1Data: BracketPlacement | null;
  player2Data: BracketPlacement | null;
  round: number;
  loseLocation?: string | null;
  markerPosition?: number | null;
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

// Helper function to get position marker or icon for a match
function getPositionMarker(match: MatchWithData): number | undefined {
  if (!props.isDoubleElimination) return undefined;

  // If markerPosition is -1, show trophy icon (handled separately in template)
  // If markerPosition is null or undefined, show no marker
  // Otherwise, show the marker number
  if (match.markerPosition === null || match.markerPosition === undefined || match.markerPosition === -1) {
    return undefined;
  }

  return match.markerPosition;
}

function showTrophyIcon(match: MatchWithData, roundIndex: number, matchIndex: number): boolean {
  if (!props.isDoubleElimination) return false;

  // Show trophy if markerPosition is -1 (Grand Finals from winners)
  if (match.markerPosition === -1 && roundIndex === props.rounds.length - 1 && matchIndex === 0) {
    return true;
  }

  return false;
}

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
  // Special handling for double elimination: connector from semi-finals to finals
  const finalsRoundIndex = props.rounds.length - 1;
  const semiFinalsRoundIndex = finalsRoundIndex - 1;

  if (props.isDoubleElimination && roundIndex === semiFinalsRoundIndex) {
    const semiFinalsOffset = calculateRoundOffset(semiFinalsRoundIndex);
    const semiFinalsConnectorPosition = semiFinalsOffset + matchHeight / 2;
    const verticalConnectorLength = 4;

    // Position horizontal connector to overlap with vertical line (no gap)
    // Fine-tuned to align perfectly
    return {
      top: `${semiFinalsConnectorPosition + verticalConnectorLength - 0.125}rem`,
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

  // For double elimination: align finals with semi-finals connector position
  const finalsRoundIndex = props.rounds.length - 1;
  const semiFinalsRoundIndex = finalsRoundIndex - 1;

  if (props.isDoubleElimination && roundIndex === finalsRoundIndex) {
    // Get semi-finals offset and calculate its center (where the connector is)
    const semiFinalsOffset = calculateRoundOffset(semiFinalsRoundIndex);
    const semiFinalsConnectorPosition = semiFinalsOffset + matchHeight / 2;

    // Position finals accounting for:
    // - 4rem vertical connector from semi-finals
    // - Align to player 1 position (25% from top of finals match)
    const verticalConnectorLength = 4;
    const paddingTop = 0;
    offset = semiFinalsConnectorPosition + verticalConnectorLength - matchHeight * 0.25 + paddingTop;
  }

  return {
    '--round-offset': `${offset}rem`,
  };
}

function getConnectorOffset(roundIndex: number): number {
  let offset = calculateRoundOffset(roundIndex);

  // Apply the same double elimination adjustment for finals
  const finalsRoundIndex = props.rounds.length - 1;
  const semiFinalsRoundIndex = finalsRoundIndex - 1;

  if (props.isDoubleElimination && roundIndex === finalsRoundIndex) {
    const semiFinalsOffset = calculateRoundOffset(semiFinalsRoundIndex);
    const semiFinalsConnectorPosition = semiFinalsOffset + matchHeight / 2;
    const verticalConnectorLength = 4;
    const paddingTop = 0;
    offset = semiFinalsConnectorPosition + verticalConnectorLength - matchHeight * 0.25 + paddingTop;
  }

  return offset;
}

function getChampionOffset() {
  const finalsRoundIndex = props.rounds.length - 1;
  const semiFinalsRoundIndex = finalsRoundIndex - 1;
  let lastRoundOffset = calculateRoundOffset(finalsRoundIndex);

  // Apply the same double elimination adjustment for finals
  if (props.isDoubleElimination) {
    const semiFinalsOffset = calculateRoundOffset(semiFinalsRoundIndex);
    const semiFinalsConnectorPosition = semiFinalsOffset + matchHeight / 2;
    const verticalConnectorLength = 4;
    const paddingTop = 0;
    lastRoundOffset =
      semiFinalsConnectorPosition + verticalConnectorLength - matchHeight * 0.25 + paddingTop;
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
  overflow: visible; /* Allow position markers to extend outside */
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

/* For double elimination: create vertical connector from semi-finals to finals */
.round.is-semi-finals .matches .match-wrapper:first-child :deep(.match::before) {
  content: '' !important; /* Ensure the pseudo-element exists */
  display: block !important;
  position: absolute !important;
  right: -2rem !important;
  top: 50% !important;
  height: 4rem !important; /* Vertical line extending to finals player 1 area */
  width: 2px !important; /* Ensure width matches other connectors */
  background: #51a2ff !important;
}

/* Hide other semi-finals connectors for double elimination finals */
.round.is-semi-finals .matches .match-wrapper:nth-child(even) :deep(.match::before) {
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

/* Dynamic spacing for all rounds */
.round[data-round='0'] .matches .match-wrapper:not(:last-child) {
  margin-bottom: 2rem;
}

.round[data-round='6'] .matches .match-wrapper:not(:last-child) {
  margin-bottom: calc(v-bind('getMatchSpacing(5)') * 1rem);
}

.round[data-round='7'] .matches .match-wrapper:not(:last-child) {
  margin-bottom: calc(v-bind('getMatchSpacing(6)') * 1rem);
}

.round[data-round='8'] .matches .match-wrapper:not(:last-child) {
  margin-bottom: calc(v-bind('getMatchSpacing(7)') * 1rem);
}

.round[data-round='9'] .matches .match-wrapper:not(:last-child) {
  margin-bottom: calc(v-bind('getMatchSpacing(8)') * 1rem);
}

.round[data-round='10'] .matches .match-wrapper:not(:last-child) {
  margin-bottom: calc(v-bind('getMatchSpacing(9)') * 1rem);
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

/* Vertical connectors for Round 8 */
.round[data-round='8'] .matches .match-wrapper:nth-child(odd) :deep(.match::before) {
  content: '';
  position: absolute;
  right: -2rem;
  top: 50%;
  width: 2px;
  height: calc(100% + v-bind('getMatchSpacing(7)') * 1rem);
  background: #51a2ff;
}

.round[data-round='8'] .matches .match-wrapper:nth-child(even) :deep(.match::before) {
  content: '';
  position: absolute;
  right: -2rem;
  bottom: 50%;
  width: 2px;
  height: calc(100% + v-bind('getMatchSpacing(7)') * 1rem);
  background: #51a2ff;
}

/* Vertical connectors for Round 9 */
.round[data-round='9'] .matches .match-wrapper:nth-child(odd) :deep(.match::before) {
  content: '';
  position: absolute;
  right: -2rem;
  top: 50%;
  width: 2px;
  height: calc(100% + v-bind('getMatchSpacing(8)') * 1rem);
  background: #51a2ff;
}

.round[data-round='9'] .matches .match-wrapper:nth-child(even) :deep(.match::before) {
  content: '';
  position: absolute;
  right: -2rem;
  bottom: 50%;
  width: 2px;
  height: calc(100% + v-bind('getMatchSpacing(8)') * 1rem);
  background: #51a2ff;
}

/* Vertical connectors for Round 10 */
.round[data-round='10'] .matches .match-wrapper:nth-child(odd) :deep(.match::before) {
  content: '';
  position: absolute;
  right: -2rem;
  top: 50%;
  width: 2px;
  height: calc(100% + v-bind('getMatchSpacing(9)') * 1rem);
  background: #51a2ff;
}

.round[data-round='10'] .matches .match-wrapper:nth-child(even) :deep(.match::before) {
  content: '';
  position: absolute;
  right: -2rem;
  bottom: 50%;
  width: 2px;
  height: calc(100% + v-bind('getMatchSpacing(9)') * 1rem);
  background: #51a2ff;
}
</style>
