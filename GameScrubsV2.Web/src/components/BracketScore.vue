<template>
  <Teleport to="body">
    <ModalEmpty id="bracket-score-modal" :modal-open="isModalOpen" @close-modal="closeModal">
      <div class="p-6">
        <!-- Modal header -->
        <div class="flex items-center justify-between mb-4">
          <h2 class="text-xl font-semibold text-gray-900 dark:text-gray-100">Scores</h2>
          <button
            @click="closeModal"
            class="text-gray-400 hover:text-gray-600 dark:hover:text-gray-200"
          >
            <svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path
                stroke-linecap="round"
                stroke-linejoin="round"
                stroke-width="2"
                d="M6 18L18 6M6 6l12 12"
              />
            </svg>
          </button>
        </div>

        <!-- Modal body -->
        <div class="mb-6 flex flex-col gap-3">
          <div v-for="score in scores" :key="score.position" class="flex gap-3">
            <span class="font-semibold">{{ score.position }}.</span>
            <span>{{ score.name }}</span>
          </div>
        </div>
      </div>
    </ModalEmpty>
  </Teleport>
</template>

<script setup lang="ts">
import { ref, inject } from 'vue';
import { bracketPlacementService } from '@/services/bracketPlacementService';
import type { useNotification } from '@/composables/useNotification';
import type { Score } from '@/models/Score';
import ModalEmpty from '@/components/ModalEmpty.vue';

const notification = inject<ReturnType<typeof useNotification>>('notification');
const scores = ref<Score[]>([]);

interface Props {
  bracketId?: number;
}

const props = withDefaults(defineProps<Props>(), {
  bracketId: undefined,
});

const isModalOpen = ref(false);

const openModal = () => {
  isModalOpen.value = true;
};

const closeModal = () => {
  isModalOpen.value = false;
};

const showScores = async () => {
  if (!props.bracketId) {
    notification?.error('Bracket id required!');
    return null;
  }

  try {
    const response = await bracketPlacementService.getBrackeScores(props.bracketId);

    // Transform the response from { "PlayerName": position } to [{ name: "PlayerName", position: position }]
    scores.value = Object.entries(response).map(([name, position]) => ({
      name,
      position,
    }));

    openModal();
  } catch (error) {
    const errorMessage = error instanceof Error ? error.message : 'Failed to update bracket';
    notification?.error(errorMessage);
  }
};

defineExpose({
  showScores,
});
</script>
