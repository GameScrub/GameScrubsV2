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
          <div
            v-for="score in scores"
            :key="score.position"
            class="flex items-center gap-3 p-3"
          >
            <div class="flex items-center gap-2 min-w-[40px]">
              <IconCrown
                v-if="score.position === 1"
                :size="24"
                :stroke-width="1.5"
                class="text-yellow-500"
              />
              <IconMedal2
                v-else-if="score.position === 2"
                :size="24"
                :stroke-width="1.5"
                class="text-gray-400"
              />
              <IconMedal
                v-else-if="score.position === 3"
                :size="24"
                :stroke-width="1.5"
                class="text-amber-600"
              />
              <span v-else class="font-semibold">{{ score.position }}.</span>
            </div>
            <span :class="getNameClass(score.position)">{{ score.name }}</span>
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
import { IconCrown, IconMedal, IconMedal2 } from '@tabler/icons-vue';

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

function getNameClass(position: number) {
  if (position === 1) {
    return 'font-bold text-lg text-yellow-600 dark:text-yellow-400';
  } else if (position === 2) {
    return 'font-semibold text-lg text-slate-600 dark:text-slate-300';
  } else if (position === 3) {
    return 'font-semibold text-lg text-amber-700 dark:text-amber-400';
  }
  return 'text-gray-700 dark:text-gray-300';
}

defineExpose({
  showScores,
});
</script>
