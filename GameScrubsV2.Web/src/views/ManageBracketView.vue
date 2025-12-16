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
        :variant="HeaderVariant.Bracket"
        :bracket-name="isEditMode ? 'Edit Bracket' : 'Create Bracket'"
        :bracket-id="isEditMode ? parseInt(route.params.id as string) : undefined"
        :is-locked="bracket?.isLocked"
        :show-scoreboard-button="false"
        @toggle-sidebar="sidebarOpen = !sidebarOpen"
      />

      <!-- Main content -->
      <main class="grow">
        <div class="px-4 sm:px-6 lg:px-8 py-8 w-full max-w-4xl mx-auto">
          <!-- Warning Alert for non-Setup status -->
          <div
            v-if="isEditMode && bracket?.status && bracket.status !== 'Setup'"
            class="mb-4 p-4 bg-yellow-100 dark:bg-yellow-900/30 border border-yellow-400 dark:border-yellow-700 text-yellow-700 dark:text-yellow-400 rounded-lg"
          >
            <div class="flex items-center justify-between gap-4">
              <div>
                <p class="font-medium">
                  This bracket cannot be edited because its status is "{{ bracket.status }}".
                </p>
                <p class="text-sm mt-1">Only brackets in "Setup" status can be modified.</p>
              </div>
              <button
                @click.stop="handleRevertToSetup"
                :disabled="isReverting"
                class="flex-shrink-0 px-4 py-2 bg-yellow-600 hover:bg-yellow-700 text-white rounded-lg transition-colors disabled:bg-gray-400 disabled:cursor-not-allowed"
              >
                {{ isReverting ? 'Reverting...' : 'Revert to Setup' }}
              </button>
            </div>
          </div>

          <!-- Form Card -->
          <div class="bg-white dark:bg-gray-800 shadow-lg rounded-lg p-6">
            <!-- Header with Manage Players button (only in edit mode) -->
            <div
              v-if="isEditMode"
              class="flex items-center justify-between mb-6 pb-4 border-b border-gray-200 dark:border-gray-700"
            >
              <h2 class="text-xl font-semibold text-gray-800 dark:text-gray-100">Edit Bracket</h2>
              <button
                type="button"
                @click="handleManagePlayers"
                class="flex items-center gap-2 px-4 py-2 bg-blue-600 hover:bg-blue-700 text-white rounded-lg transition-colors"
              >
                <IconUsers :size="20" :stroke-width="2" />
                Manage Players
              </button>
            </div>

            <form @submit.prevent="handleSubmit">
              <!-- Bracket Name -->
              <div class="mb-6">
                <label
                  for="name"
                  class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2"
                >
                  Bracket Name
                  <span class="text-red-500">*</span>
                </label>
                <input
                  id="name"
                  v-model="formData.name"
                  type="text"
                  required
                  minlength="5"
                  maxlength="100"
                  :disabled="isFormDisabled"
                  class="w-full px-4 py-2 border border-gray-300 dark:border-gray-600 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent dark:bg-gray-700 dark:text-gray-100 disabled:opacity-50 disabled:cursor-not-allowed"
                  placeholder="Enter bracket name"
                />
              </div>

              <!-- Game Name -->
              <div class="mb-6">
                <label
                  for="game"
                  class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2"
                >
                  Game
                  <span class="text-red-500">*</span>
                </label>
                <input
                  id="game"
                  v-model="formData.game"
                  type="text"
                  required
                  minlength="5"
                  maxlength="100"
                  :disabled="isFormDisabled"
                  class="w-full px-4 py-2 border border-gray-300 dark:border-gray-600 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent dark:bg-gray-700 dark:text-gray-100 disabled:opacity-50 disabled:cursor-not-allowed"
                  placeholder="Enter game name"
                />
              </div>

              <!-- Bracket Type -->
              <div class="mb-6">
                <label
                  for="type"
                  class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2"
                >
                  Bracket Type
                  <span class="text-red-500">*</span>
                </label>
                <select
                  id="type"
                  v-model="formData.type"
                  required
                  :disabled="isFormDisabled"
                  class="w-full px-4 py-2 border border-gray-300 dark:border-gray-600 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent dark:bg-gray-700 dark:text-gray-100 disabled:opacity-50 disabled:cursor-not-allowed"
                >
                  <option value="">Select bracket type</option>
                  <option v-for="type in bracketTypes" :key="type.value" :value="type.value">
                    {{ type.label }}
                  </option>
                </select>
              </div>

              <!-- Competition Type -->
              <div class="mb-6">
                <label
                  for="competition"
                  class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2"
                >
                  Competition Type
                  <span class="text-red-500">*</span>
                </label>
                <select
                  id="competition"
                  v-model="formData.competition"
                  required
                  :disabled="isFormDisabled"
                  class="w-full px-4 py-2 border border-gray-300 dark:border-gray-600 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent dark:bg-gray-700 dark:text-gray-100 disabled:opacity-50 disabled:cursor-not-allowed"
                >
                  <option value="">Select competition type</option>
                  <option v-for="comp in competitionTypes" :key="comp.value" :value="comp.value">
                    {{ comp.label }}
                  </option>
                </select>
              </div>

              <!-- Start Date -->
              <div class="mb-6">
                <label
                  for="startDate"
                  class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2"
                >
                  Start Date
                  <span class="text-red-500">*</span>
                </label>
                <input
                  id="startDate"
                  v-model="formData.startDate"
                  type="date"
                  required
                  :min="minDate"
                  :disabled="isFormDisabled"
                  class="w-full px-4 py-2 border border-gray-300 dark:border-gray-600 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent dark:bg-gray-700 dark:text-gray-100 disabled:opacity-50 disabled:cursor-not-allowed"
                />
              </div>

              <!-- Email (Optional) -->
              <div class="mb-6">
                <label
                  for="email"
                  class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2"
                >
                  Email
                </label>
                <input
                  id="email"
                  v-model="formData.email"
                  type="email"
                  :disabled="isFormDisabled"
                  class="w-full px-4 py-2 border border-gray-300 dark:border-gray-600 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent dark:bg-gray-700 dark:text-gray-100 disabled:opacity-50 disabled:cursor-not-allowed"
                  placeholder="your@email.com"
                />
              </div>

              <!-- URL (Optional) -->
              <div class="mb-6">
                <label
                  for="url"
                  class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2"
                >
                  URL
                </label>
                <input
                  id="url"
                  v-model="formData.url"
                  type="url"
                  :disabled="isFormDisabled"
                  class="w-full px-4 py-2 border border-gray-300 dark:border-gray-600 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent dark:bg-gray-700 dark:text-gray-100 disabled:opacity-50 disabled:cursor-not-allowed"
                  placeholder="https://example.com"
                />
              </div>

              <!-- Lock Code (Optional) -->
              <div class="mb-6">
                <label
                  for="lockCode"
                  class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2"
                >
                  Lock Code
                </label>
                <input
                  id="lockCode"
                  v-model="formData.lockCode"
                  type="text"
                  :disabled="isFormDisabled"
                  class="w-full px-4 py-2 border border-gray-300 dark:border-gray-600 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent dark:bg-gray-700 dark:text-gray-100 disabled:opacity-50 disabled:cursor-not-allowed"
                  placeholder="Optional lock code to protect bracket"
                />
                <p class="mt-1 text-sm text-gray-500 dark:text-gray-400">
                  If set, this code will be required to edit the bracket later
                </p>
              </div>

              <!-- Form Actions -->
              <div
                class="flex items-center justify-between pt-6 border-t border-gray-200 dark:border-gray-700"
              >
                <button
                  type="button"
                  @click="handleCancel"
                  class="px-6 py-2 border border-gray-300 dark:border-gray-600 rounded-lg text-gray-700 dark:text-gray-300 hover:bg-gray-50 dark:hover:bg-gray-700 transition-colors"
                >
                  Back
                </button>
                <button
                  type="submit"
                  :disabled="isSubmitting || isFormDisabled"
                  class="px-6 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 disabled:bg-gray-400 disabled:cursor-not-allowed transition-colors"
                >
                  {{
                    isSubmitting ? 'Saving...' : isEditMode ? 'Update Bracket' : 'Create Bracket'
                  }}
                </button>
              </div>
            </form>
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
</template>

<script setup lang="ts">
import { ref, computed, onMounted, inject, type Ref } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import { IconUsers } from '@tabler/icons-vue';
import Sidebar from '@/partials/Sidebar.vue';
import Header from '@/partials/Header.vue';
import ConfirmModal from '@/components/ConfirmModal.vue';
import { bracketService } from '@/services/bracketService';
import { BracketType } from '@/models/BracketType';
import { Competition } from '@/models/Competition';
import type { useNotification } from '@/composables/useNotification';
import { HeaderVariant } from '@/models/HeaderVariant';
import { useBracketStore } from '@/stores/bracket';

const notification = inject<ReturnType<typeof useNotification>>('notification');

const route = useRoute();
const router = useRouter();
const sidebarOpen: Ref<boolean> = ref(false);
const headerRef = ref<InstanceType<typeof Header>>();
const bracketStore = useBracketStore();

const bracket = ref<{ isLocked: boolean; status: string }>();

const isEditMode = computed(() => !!route.params.id);
const isSubmitting = ref(false);
const isReverting = ref(false);
const showRevertConfirm = ref(false);

const isFormDisabled = computed(() => {
  return isEditMode.value && bracket.value?.status !== 'Setup';
});

interface BracketFormData {
  name: string;
  game: string;
  type: BracketType | '';
  competition: Competition | '';
  startDate: string;
  email: string;
  url: string;
  lockCode: string;
}

const formData: Ref<BracketFormData> = ref({
  name: '',
  game: '',
  type: '',
  competition: '',
  startDate: '',
  email: '',
  url: '',
  lockCode: '',
});

const minDate = computed(() => {
  const today = new Date();
  return today.toISOString().split('T')[0];
});

const bracketTypes = [
  { value: BracketType.Single_8, label: 'Single Elimination - 8 Players' },
  { value: BracketType.Single_16, label: 'Single Elimination - 16 Players' },
  { value: BracketType.Single_32, label: 'Single Elimination - 32 Players' },
  { value: BracketType.Double_8, label: 'Double Elimination - 8 Players' },
  { value: BracketType.Double_16, label: 'Double Elimination - 16 Players' },
  { value: BracketType.Double_32, label: 'Double Elimination - 32 Players' },
];

const competitionTypes = [
  { value: Competition.VideoGames, label: 'Video Games' },
  { value: Competition.Sports, label: 'Sports' },
  { value: Competition.Esports, label: 'Esports' },
  { value: Competition.Other, label: 'Other' },
];

onMounted(async () => {
  if (isEditMode.value) {
    await loadBracket();
  }
});

async function loadBracket() {
  try {
    const bracketId = parseInt(route.params.id as string);
    const bracketData = await bracketService.getById(bracketId);

    bracket.value = {
      isLocked: bracketData.isLocked,
      status: bracketData.status,
    };

    formData.value = {
      name: bracketData.name,
      game: bracketData.game,
      type: bracketData.type,
      competition: bracketData.competition,
      startDate: bracketData.startDate?.split('T')[0] || '',
      email: '',
      url: bracketData.url || '',
      lockCode: '',
    };
  } catch (err) {
    const errorMessage = err instanceof Error ? err.message : 'Failed to load bracket';
    notification?.error(errorMessage);
  }
}

async function handleSubmit() {
  isSubmitting.value = true;

  try {
    if (isEditMode.value) {
      await updateBracket();
    } else {
      await createBracket();
    }
  } catch (err) {
    const errorMessage = err instanceof Error ? err.message : 'An error occurred';
    notification?.error(errorMessage);
  } finally {
    isSubmitting.value = false;
  }
}

async function createBracket() {
  if (!formData.value.type || !formData.value.competition) {
    throw new Error('Please select bracket type and competition type');
  }

  const payload = {
    name: formData.value.name,
    game: formData.value.game,
    type: formData.value.type,
    competition: formData.value.competition,
    startDate: formData.value.startDate,
    email: formData.value.email || undefined,
    url: formData.value.url || undefined,
    lockCode: formData.value.lockCode || undefined,
  };

  const result = await bracketService.create(payload);
  notification?.success('Bracket created successfully! Add players to get started.');

  setTimeout(() => {
    router.push({ name: 'bracket-manage-users', params: { id: result.id } });
  }, 1500);
}

async function updateBracket() {
  if (!formData.value.type || !formData.value.competition) {
    throw new Error('Please select bracket type and competition type');
  }

  const bracketId = parseInt(route.params.id as string);
  const payload = {
    id: bracketId,
    name: formData.value.name,
    game: formData.value.game,
    type: formData.value.type,
    competition: formData.value.competition,
    startDate: formData.value.startDate,
    email: formData.value.email || undefined,
    url: formData.value.url || undefined,
    lockCode: formData.value.lockCode || undefined,
  };

  try {
    await bracketService.update(payload, bracketStore.getLockCode(bracketId));
    notification?.success('Bracket updated successfully!');
    await loadBracket();
  } catch (err) {
    const errorMessage = err instanceof Error ? err.message : 'Failed to update bracket';
    if (errorMessage.includes('lock code')) {
      headerRef.value?.showLockCodeError();
    }
    throw err;
  }
}

function handleRevertToSetup() {
  showRevertConfirm.value = true;
}

async function confirmRevertToSetup() {
  if (!bracket.value || !isEditMode.value) return;

  showRevertConfirm.value = false;
  isReverting.value = true;

  try {
    const bracketId = parseInt(route.params.id as string);
    const updatedBracket = await bracketService.changeStatus(
      bracketId,
      'Setup',
      bracketStore.getLockCode(bracketId),
    );

    // Update local bracket state
    bracket.value = {
      isLocked: updatedBracket.isLocked,
      status: updatedBracket.status,
    };

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

function handleCancel() {
  router.back();
}

function handleManagePlayers() {
  router.push({ name: 'bracket-manage-users', params: { id: route.params.id } });
}
</script>
