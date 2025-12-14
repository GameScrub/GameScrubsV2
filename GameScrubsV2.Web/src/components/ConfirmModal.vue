<template>
  <ModalEmpty :id="id" :modal-open="modalOpen" @close-modal="handleCancel">
    <div class="p-6">
      <!-- Modal header -->
      <div class="mb-4">
        <h2 class="text-xl font-semibold text-gray-900 dark:text-gray-100">
          {{ title }}
        </h2>
      </div>

      <!-- Modal body -->
      <div class="mb-6 text-gray-600 dark:text-gray-400">
        <p>{{ message }}</p>
      </div>

      <!-- Modal footer -->
      <div class="flex justify-end gap-3">
        <button
          v-if="showCancel"
          @click.stop="handleCancel"
          class="px-4 py-2 bg-gray-200 dark:bg-gray-700 text-gray-800 dark:text-gray-200 rounded hover:bg-gray-300 dark:hover:bg-gray-600 transition-colors"
        >
          {{ cancelText }}
        </button>
        <button
          @click.stop="handleConfirm"
          :class="[
            'px-4 py-2 text-white rounded transition-colors',
            danger
              ? 'bg-red-600 hover:bg-red-700'
              : 'bg-blue-600 hover:bg-blue-700'
          ]"
        >
          {{ confirmText }}
        </button>
      </div>
    </div>
  </ModalEmpty>
</template>

<script setup lang="ts">
import ModalEmpty from './ModalEmpty.vue';

interface Props {
  id: string;
  modalOpen: boolean;
  title: string;
  message: string;
  confirmText?: string;
  cancelText?: string;
  danger?: boolean;
  showCancel?: boolean;
}

withDefaults(defineProps<Props>(), {
  confirmText: 'Confirm',
  cancelText: 'Cancel',
  danger: false,
  showCancel: true,
});

const emit = defineEmits<{
  confirm: [];
  cancel: [];
}>();

function handleConfirm() {
  emit('confirm');
}

function handleCancel() {
  emit('cancel');
}
</script>
