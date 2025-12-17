<template>
  <div class="flex h-screen overflow-hidden">
    <!-- Sidebar -->
    <Sidebar :sidebarOpen="sidebarOpen" @close-sidebar="sidebarOpen = false" variant="v2" />

    <!-- Content area -->
    <div class="relative flex flex-col flex-1 overflow-y-auto overflow-x-hidden">
      <!-- Header -->
      <Header
        ref="headerRef"
        :sidebarOpen="sidebarOpen"
        :variant="HeaderVariant.ManagePlayer"
        :bracket-name="bracket?.name"
        :game="bracket?.game"
        :bracket-id="bracket?.id"
        :is-locked="bracket?.isLocked"
        :show-scoreboard-button="false"
        @toggle-sidebar="sidebarOpen = !sidebarOpen"
      />

      <!-- Main content -->
      <main class="grow">
        <div class="px-4 sm:px-6 lg:px-8 py-8 w-full max-w-4xl mx-auto">
          <!-- Warning Alert for non-Setup status -->
          <div
            v-if="bracket?.status && bracket.status !== 'Setup'"
            class="mb-4 p-4 bg-yellow-100 dark:bg-yellow-900/30 border border-yellow-400 dark:border-yellow-700 text-yellow-700 dark:text-yellow-400 rounded-lg"
          >
            <div class="flex items-center justify-between gap-4">
              <div>
                <p class="font-medium">
                  Players cannot be added or reordered because the bracket status is "{{
                    bracket.status
                  }}".
                </p>
                <p class="text-sm mt-1">Only brackets in "Setup" status can be modified.</p>
              </div>
              <button
                @click.stop="handleRevertToSetup"
                :disabled="isReverting"
                class="shrink-0 px-4 py-2 bg-yellow-600 hover:bg-yellow-700 text-white rounded-lg transition-colors disabled:bg-gray-400 disabled:cursor-not-allowed"
              >
                {{ isReverting ? 'Reverting...' : 'Revert to Setup' }}
              </button>
            </div>
          </div>

          <!-- Page Title -->
          <div class="mb-6">
            <div class="flex items-center justify-between">
              <div>
                <h1 class="text-2xl md:text-3xl text-gray-800 dark:text-gray-100 font-bold">
                  Manage Players
                </h1>
                <p class="text-gray-600 dark:text-gray-400 mt-1">
                  Add players and arrange their seeding order by dragging them
                </p>
              </div>
              <div class="flex items-center gap-2">
                <button
                  type="button"
                  @click.stop="handleViewBracket"
                  class="flex items-center gap-2 px-4 py-2 bg-blue-600 hover:bg-blue-700 text-white rounded-lg transition-colors"
                >
                  <IconTournament :size="20" :stroke-width="2" />
                  View Bracket
                </button>
                <button
                  type="button"
                  @click.stop="handleEditBracket"
                  class="flex items-center gap-2 px-4 py-2 bg-gray-600 hover:bg-gray-700 dark:bg-gray-700 dark:hover:bg-gray-600 text-white rounded-lg transition-colors"
                >
                  <IconSettings :size="20" :stroke-width="2" />
                  Edit Bracket
                </button>
              </div>
            </div>
          </div>

          <!-- Add Player Form -->
          <div class="bg-white dark:bg-gray-800 shadow-lg rounded-lg p-6 mb-6">
            <h2 class="text-lg font-semibold text-gray-800 dark:text-gray-100 mb-4">
              Add New Player
            </h2>
            <form @submit.prevent="handleAddPlayer" class="flex gap-3">
              <input
                v-model="newPlayerName"
                type="text"
                maxlength="25"
                minlength="1"
                required
                :disabled="isFormDisabled"
                placeholder="Enter player name"
                class="flex-1 px-4 py-2 border border-gray-300 dark:border-gray-600 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent dark:bg-gray-700 dark:text-gray-100 disabled:opacity-50 disabled:cursor-not-allowed"
              />
              <button
                type="submit"
                :disabled="isSubmitting || !newPlayerName.trim() || isFormDisabled"
                class="px-6 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 disabled:bg-gray-400 disabled:cursor-not-allowed transition-colors"
              >
                Add Player
              </button>
            </form>
            <p class="text-sm text-gray-500 dark:text-gray-400 mt-2">
              Maximum {{ maxPlayers }} players for this bracket type. Current:
              {{ players.length }} /
              {{ maxPlayers }}
            </p>
          </div>

          <!-- Players List -->
          <div class="bg-white dark:bg-gray-800 shadow-lg rounded-lg p-6">
            <div class="flex items-center justify-between mb-4">
              <h2 class="text-lg font-semibold text-gray-800 dark:text-gray-100">
                Player Seeding (Drag to Reorder)
              </h2>
              <button
                v-if="displayPlayers.length > 0"
                type="button"
                @click="handleRandomize"
                :disabled="isSubmitting || isFormDisabled"
                class="flex items-center gap-2 px-3 py-2 text-sm bg-purple-600 hover:bg-purple-700 text-white rounded-lg transition-colors disabled:bg-gray-400 disabled:cursor-not-allowed"
                title="Randomize player order"
              >
                <IconReload :size="16" :stroke-width="2" />
                Randomize
              </button>
            </div>

            <div
              v-if="displayPlayers.length === 0"
              class="text-center py-8 text-gray-500 dark:text-gray-400"
            >
              No players added yet. Add your first player above.
            </div>

            <draggable
              v-else
              v-model="displayPlayers"
              @end="handleDragEnd"
              item-key="seed"
              handle=".drag-handle"
              :disabled="isFormDisabled"
              class="space-y-2"
            >
              <template #item="{ element, index }">
                <div
                  :class="[
                    'flex items-center gap-3 p-4 rounded-lg border transition-colors',
                    index % 2 === 0
                      ? 'bg-blue-50 dark:bg-blue-900/20 border-blue-300 dark:border-blue-700 hover:border-blue-400 dark:hover:border-blue-500'
                      : 'bg-gray-50 dark:bg-gray-700/50 border-gray-200 dark:border-gray-600 hover:border-blue-400 dark:hover:border-blue-500',
                    element.id === 0 ? 'opacity-60' : '',
                    index % 2 === 1 ? 'mb-4' : '',
                  ]"
                >
                  <!-- Drag Handle -->
                  <div
                    :class="[
                      'drag-handle',
                      isFormDisabled ? 'cursor-not-allowed opacity-50' : 'cursor-move',
                      'text-gray-400 hover:text-gray-600 dark:hover:text-gray-300',
                    ]"
                  >
                    <svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path
                        stroke-linecap="round"
                        stroke-linejoin="round"
                        stroke-width="2"
                        d="M4 8h16M4 16h16"
                      />
                    </svg>
                  </div>

                  <!-- Seed Number -->
                  <div
                    :class="[
                      'shrink-0 w-10 h-10 flex items-center justify-center text-white rounded-full font-bold',
                      element.id === 0
                        ? 'bg-gray-500'
                        : index % 2 === 0
                          ? 'bg-blue-600'
                          : 'bg-gray-600',
                    ]"
                  >
                    {{ index + 1 }}
                  </div>

                  <!-- Player Name -->
                  <div class="flex-1 font-medium text-gray-800 dark:text-gray-100">
                    {{ element.playerName || 'Unnamed Player' }}
                  </div>

                  <!-- Delete Button (only for real players, not byes) -->
                  <button
                    v-if="element.id !== 0"
                    @click.stop="handleRemovePlayer(element.id)"
                    :disabled="isSubmitting || isFormDisabled"
                    class="shrink-0 p-2 text-red-600 hover:bg-red-50 dark:hover:bg-red-900/30 rounded-lg transition-colors disabled:opacity-50 disabled:cursor-not-allowed"
                    title="Remove player"
                  >
                    <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path
                        stroke-linecap="round"
                        stroke-linejoin="round"
                        stroke-width="2"
                        d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16"
                      />
                    </svg>
                  </button>
                  <!-- Placeholder space for byes to maintain alignment -->
                  <div v-else class="shrink-0 w-10"></div>
                </div>
              </template>
            </draggable>
          </div>

          <!-- Action Buttons -->
          <div class="flex items-center justify-between mt-6">
            <button
              type="button"
              @click="handleBack"
              class="px-6 py-2 border border-gray-300 dark:border-gray-600 rounded-lg text-gray-700 dark:text-gray-300 hover:bg-gray-50 dark:hover:bg-gray-700 transition-colors"
            >
              Back to Bracket
            </button>
            <button
              @click="handleSaveOrder"
              :disabled="isSubmitting || !hasChanges || isFormDisabled"
              class="px-6 py-2 bg-green-600 text-white rounded-lg hover:bg-green-700 disabled:bg-gray-400 disabled:cursor-not-allowed transition-colors"
            >
              {{ isSubmitting ? 'Saving...' : 'Save Order' }}
            </button>
          </div>
        </div>
      </main>
    </div>
  </div>

  <!-- Confirm Modal for Reverting to Setup -->
  <Teleport to="body">
    <ConfirmModal
      id="revert-to-setup-modal"
      :modal-open="showRevertConfirm"
      title="Revert to Setup Status"
      message="Are you sure you want to revert this bracket to Setup status? This will clear all match results."
      confirm-text="Revert to Setup"
      cancel-text="Cancel"
      :danger="true"
      @confirm="confirmRevertToSetup"
      @cancel="showRevertConfirm = false"
    />
  </Teleport>

  <!-- Confirm Modal for Removing Player -->
  <Teleport to="body">
    <ConfirmModal
      id="remove-player-modal"
      :modal-open="showRemovePlayerConfirm"
      title="Remove Player"
      message="Are you sure you want to remove this player?"
      confirm-text="Remove"
      cancel-text="Cancel"
      :danger="true"
      @confirm="confirmRemovePlayer"
      @cancel="showRemovePlayerConfirm = false"
    />
  </Teleport>

  <!-- Confirm Modal for Unsaved Changes -->
  <Teleport to="body">
    <ConfirmModal
      id="unsaved-changes-modal"
      :modal-open="showUnsavedChangesConfirm"
      title="Unsaved Changes"
      message="You have unsaved changes. Are you sure you want to leave without saving?"
      confirm-text="Leave Without Saving"
      cancel-text="Stay"
      :danger="true"
      @confirm="confirmLeaveWithoutSaving"
      @cancel="cancelLeave"
    />
  </Teleport>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, inject, nextTick, watch, type Ref } from 'vue';
import { useRoute, useRouter, onBeforeRouteLeave } from 'vue-router';
import draggable from 'vuedraggable';
import { IconReload, IconTournament, IconSettings } from '@tabler/icons-vue';
import Sidebar from '@/partials/Sidebar.vue';
import Header from '@/partials/Header.vue';
import ConfirmModal from '@/components/ConfirmModal.vue';
import { bracketService } from '@/services/bracketService';
import { playerService } from '@/services/playerService';
import { type Bracket } from '@/models/Bracket';
import { type Player } from '@/models/Player';
import { HeaderVariant } from '@/models/HeaderVariant';
import type { useNotification } from '@/composables/useNotification';
import { useBracketStore } from '@/stores/bracket';

const route = useRoute();
const router = useRouter();
const notification = inject<ReturnType<typeof useNotification>>('notification');
const sidebarOpen: Ref<boolean> = ref(false);
const headerRef = ref<InstanceType<typeof Header>>();
const bracketStore = useBracketStore();

const bracket = ref<Bracket>();
const players = ref<Player[]>([]);
const displayPlayers = ref<Player[]>([]);
const originalPlayerOrder = ref<number[]>([]);
const newPlayerName = ref('');

const isSubmitting = ref(false);
const isReverting = ref(false);
const showRevertConfirm = ref(false);
const showRemovePlayerConfirm = ref(false);
const playerToRemove = ref<number | null>(null);
const isDirty = ref(false);
const showUnsavedChangesConfirm = ref(false);
const pendingNavigation = ref<(() => void) | null>(null);
const isLoadingData = ref(false);
const isInitialLoad = ref(true);

const maxPlayers = computed(() => {
  if (!bracket.value) return 0;
  const match = bracket.value.type.match(/_(\d+)$/);
  return match && match[1] ? parseInt(match[1]) : 0;
});

// Computed property that combines real players with placeholder bye players
const playersWithByes = computed(() => {
  const result = [...players.value];
  const byesNeeded = maxPlayers.value - players.value.length;

  // Add placeholder bye players

  const missingSeedIds: number[] = [];

  for (let i = 0; i < maxPlayers.value; i++) {
    if (!players.value.some((x) => x.seed == i)) {
      missingSeedIds.push(i);
      // result.push({
      //   id: 0,
      //   bracketId: bracket.value?.id || 0,
      //   playerName: '--',
      //   seed: i,
      //   score: 0,
      // });
      //result.sort((a, b) => a.seed - b.seed);
    }
  }

  console.log(missingSeedIds);

  for (let i = 0; i < byesNeeded; i++) {
    const byeSeed = missingSeedIds.shift() ?? players.value.length + i;

    result.push({
      id: 0,
      bracketId: bracket.value?.id || 0,
      playerName: '--',
      seed: byeSeed, //players.value.length + i,
      score: 0,
    });

    missingSeedIds.pop();
  }

  result.sort((a, b) => a.seed - b.seed);

  return result;
});

const hasChanges = computed(() => {
  const currentOrder = displayPlayers.value.map((p) => p.id);
  return JSON.stringify(currentOrder) !== JSON.stringify(originalPlayerOrder.value);
});

const isFormDisabled = computed(() => {
  return bracket.value?.status !== 'Setup';
});

// Watch for player order changes to set dirty flag
watch(
  displayPlayers,
  () => {
    // Don't set dirty flag if we're loading data or during initial load
    if (!isLoadingData.value && !isInitialLoad.value) {
      const currentOrder = displayPlayers.value.map((p) => p.id);
      const hasOrderChanged =
        JSON.stringify(currentOrder) !== JSON.stringify(originalPlayerOrder.value);
      if (hasOrderChanged) {
        isDirty.value = true;
      }
    }
  },
  { deep: true },
);

// Navigation guard to prevent accidental navigation with unsaved changes
onBeforeRouteLeave((to, from, next) => {
  if (isDirty.value && !isSubmitting.value) {
    showUnsavedChangesConfirm.value = true;
    pendingNavigation.value = () => next();
    return false;
  } else {
    next();
  }
});

onMounted(async () => {
  await loadData();
});

async function loadData() {
  isLoadingData.value = true;
  try {
    const bracketId = Number(route.params.id);

    bracket.value = await bracketService.getById(bracketId);
    players.value = await playerService.getAll(bracketId);

    // Populate displayPlayers with real players + bye placeholders
    displayPlayers.value = playersWithByes.value;
    originalPlayerOrder.value = playersWithByes.value.map((p) => p.id);
  } catch (err) {
    const errorMessage = err instanceof Error ? err.message : 'Failed to load bracket data';
    notification?.error(errorMessage);
  } finally {
    isLoadingData.value = false;
    isInitialLoad.value = false;
  }
}

async function handleAddPlayer() {
  if (!newPlayerName.value.trim() || !bracket.value) return;

  isSubmitting.value = true;

  try {
    if (players.value.length >= maxPlayers.value) {
      throw new Error('Bracket is full');
    }

    await playerService.add(
      bracket.value.id,
      newPlayerName.value.trim(),
      players.value.length,
      bracketStore.getLockCode(bracket.value.id),
    );

    notification?.success('Player added successfully!');
    newPlayerName.value = '';
    await loadData();
    isDirty.value = false;
  } catch (err) {
    const errorMessage = err instanceof Error ? err.message : 'Failed to add player';
    if (errorMessage.includes('lock code')) {
      headerRef.value?.showLockCodeError();
    }
    notification?.error(errorMessage);
  } finally {
    isSubmitting.value = false;
  }
}

function handleRemovePlayer(playerId: number) {
  playerToRemove.value = playerId;
  showRemovePlayerConfirm.value = true;
}

async function confirmRemovePlayer() {
  if (!bracket.value || playerToRemove.value === null) {
    showRemovePlayerConfirm.value = false;
    return;
  }

  const playerId = playerToRemove.value;
  showRemovePlayerConfirm.value = false;
  isSubmitting.value = true;

  try {
    await playerService.remove(
      bracket.value.id,
      playerId,
      bracketStore.getLockCode(bracket.value.id),
    );

    notification?.success('Player removed successfully!');
    await loadData();
    isDirty.value = false;
  } catch (err) {
    const errorMessage = err instanceof Error ? err.message : 'Failed to remove player';
    if (errorMessage.includes('lock code')) {
      headerRef.value?.showLockCodeError();
    }
    notification?.error(errorMessage);
  } finally {
    isSubmitting.value = false;
    playerToRemove.value = null;
  }
}

async function handleDragEnd() {
  // The players array is automatically updated by vuedraggable
  // We just need to show that there are unsaved changes
}

async function handleSaveOrder() {
  if (!bracket.value || !hasChanges.value) return;

  isSubmitting.value = true;

  try {
    // Map displayPlayers to IDs, where 0 represents a bye
    const playerIds = displayPlayers.value.map((p) => p.id);
    await playerService.reorder(
      bracket.value.id,
      playerIds,
      bracketStore.getLockCode(bracket.value.id),
    );

    notification?.success('Player order saved successfully!');
    originalPlayerOrder.value = playerIds;
    isDirty.value = false;
  } catch (err) {
    const errorMessage = err instanceof Error ? err.message : 'Failed to save player order';
    if (errorMessage.includes('lock code')) {
      headerRef.value?.showLockCodeError();
    }
    notification?.error(errorMessage);
  } finally {
    isSubmitting.value = false;
  }
}

async function handleRandomize() {
  // Save current scroll position
  const scrollY = window.scrollY;
  const scrollX = window.scrollX;

  // Clear the array first to prevent any reactivity issues
  displayPlayers.value = [];

  // Wait for Vue to process the clear
  await nextTick();

  // Get a fresh snapshot from playersWithByes computed and create deep copies
  const freshSnapshot = playersWithByes.value.map((p) => ({ ...p }));

  // Fisher-Yates shuffle algorithm
  for (let i = freshSnapshot.length - 1; i > 0; i--) {
    const j = Math.floor(Math.random() * (i + 1));
    const temp = freshSnapshot[i];
    if (temp && freshSnapshot[j]) {
      freshSnapshot[i] = freshSnapshot[j];
      freshSnapshot[j] = temp;
    }
  }

  // Set the shuffled array
  displayPlayers.value = freshSnapshot;

  // Restore scroll position
  await nextTick();
  window.scrollTo(scrollX, scrollY);

  notification?.success('Player order randomized!');
}

function handleRevertToSetup() {
  showRevertConfirm.value = true;
}

async function confirmRevertToSetup() {
  if (!bracket.value) return;

  showRevertConfirm.value = false;
  isReverting.value = true;

  try {
    const updatedBracket = await bracketService.changeStatus(
      bracket.value.id,
      'Setup',
      bracketStore.getLockCode(bracket.value.id),
    );

    // Update local bracket state
    bracket.value = updatedBracket;

    notification?.success('Bracket status reverted to Setup successfully!');
  } catch (err) {
    const errorMessage = err instanceof Error ? err.message : 'Failed to revert bracket status';
    if (errorMessage.includes('lock code')) {
      headerRef.value?.showLockCodeError();
    }
    notification?.error(errorMessage);
  } finally {
    isReverting.value = false;
  }
}

function handleBack() {
  router.push({ name: 'bracket', params: { id: bracket.value?.id } });
}

function handleViewBracket() {
  router.push({ name: 'bracket', params: { id: bracket.value?.id } });
}

function handleEditBracket() {
  router.push({ name: 'bracket-edit', params: { id: bracket.value?.id } });
}

function confirmLeaveWithoutSaving() {
  showUnsavedChangesConfirm.value = false;
  isDirty.value = false;
  if (pendingNavigation.value) {
    pendingNavigation.value();
    pendingNavigation.value = null;
  }
}

function cancelLeave() {
  showUnsavedChangesConfirm.value = false;
  pendingNavigation.value = null;
}
</script>
