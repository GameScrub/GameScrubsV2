<template>
  <div class="position-marker" :style="markerStyle">
    <div class="connector-line" :style="connectorStyle"></div>
    <div class="marker-badge" :style="badgeStyle">
      {{ number }}
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue';

interface Props {
  number: number | string;
  position: 'left' | 'right';
  verticalPosition: number; // percentage from top (0-100)
  connectorLength?: number; // in rem, default 4
  color?: string; // border and connector color
}

const props = withDefaults(defineProps<Props>(), {
  connectorLength: 4,
  color: '#6b7280',
});

const markerStyle = computed(() => ({
  top: `${props.verticalPosition}%`,
  [props.position]: '0',
}));

const connectorStyle = computed(() => {
  const direction = props.position === 'left' ? 'right' : 'left';
  return {
    [direction]: '100%',
    width: `${props.connectorLength}rem`,
    background: props.position === 'left' ? '#51a2ff' : '#dc2626',
  };
});

const badgeStyle = computed(() => {
  const direction = props.position === 'left' ? 'right' : 'left';
  return {
    [direction]: `${props.connectorLength}rem`,
    borderColor: props.color,
  };
});
</script>

<style scoped>
.position-marker {
  position: absolute;
  transform: translateY(-50%);
  z-index: 10;
}

.connector-line {
  position: absolute;
  top: 50%;
  height: 2px;
  transform: translateY(-50%);
}

.marker-badge {
  position: absolute;
  top: 50%;
  transform: translateY(-50%);
  width: 2.5rem;
  height: 2.5rem;
  line-height: 2.5rem;
  text-align: center;
  background: white;
  border: 2px solid;
  border-radius: 50%;
  font-size: 1rem;
  font-weight: bold;
  color: #374151;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.2);
}

:global(.dark) .marker-badge {
  background: #1f2937;
  color: #e5e7eb;
}
</style>
