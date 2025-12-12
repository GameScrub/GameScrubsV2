<script setup lang="ts">
import { RouterView } from 'vue-router';
import { provide } from 'vue';
import { useNotification } from '@/composables/useNotification';
import Notification from '@/components/Notification.vue';

const notificationSystem = useNotification();
const { notifications, removeNotification } = notificationSystem;

provide('notification', notificationSystem);
</script>

<template>
  <RouterView />

  <div class="fixed bottom-4 right-4 z-50 space-y-2">
    <TransitionGroup name="notification" tag="div" class="space-y-2">
      <div v-for="notification in notifications" :key="`notif-${notification.id}`">
        <Notification
          :open="notification.open"
          :type="notification.type"
          @close="removeNotification(notification.id)"
        >
          {{ notification.message }}
        </Notification>
      </div>
    </TransitionGroup>
  </div>
</template>

<style scoped>
.notification-enter-active,
.notification-leave-active {
  transition: all 0.3s ease;
}

.notification-enter-from {
  opacity: 0;
  transform: translateX(100px);
}

.notification-leave-to {
  opacity: 0;
  transform: translateX(100px);
}
</style>
