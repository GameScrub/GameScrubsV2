import { ref } from 'vue';

export type NotificationType = 'success' | 'error' | 'warning' | 'info';

export interface NotificationItem {
  id: number;
  message: string;
  type: NotificationType;
  open: boolean;
}

let notificationId = 0;

export function useNotification() {
  const notifications = ref<NotificationItem[]>([]);

  const addNotification = (message: string, type: NotificationType = 'info', duration = 3000) => {
    const id = notificationId++;
    const notification: NotificationItem = {
      id,
      message,
      type,
      open: true,
    };

    notifications.value.push(notification);

    if (duration > 0) {
      setTimeout(() => {
        removeNotification(id);
      }, duration);
    }
  };

  const removeNotification = (id: number) => {
    const index = notifications.value.findIndex((n) => n.id === id);

    if (index > -1) {
      notifications.value[index]!.open = false;
      // Remove from array after animation
      setTimeout(() => {
        notifications.value = notifications.value.filter((n) => n.id !== id);
      }, 200);
    }
  };

  const success = (message: string, duration = 3000) =>
    addNotification(message, 'success', duration);
  const error = (message: string, duration = 3000) => addNotification(message, 'error', duration);
  const warning = (message: string, duration = 3000) =>
    addNotification(message, 'warning', duration);
  const info = (message: string, duration = 3000) => addNotification(message, 'info', duration);

  return {
    notifications,
    removeNotification,
    success,
    error,
    warning,
    info,
  };
}
