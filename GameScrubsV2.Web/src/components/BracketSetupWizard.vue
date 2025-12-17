<template>
  <div class="bracket-setup-container">
    <div class="setup-steps">
      <!-- Step 1: Manage Bracket -->
      <div class="setup-step">
        <div class="step-number">1</div>
        <div class="step-content">
          <h2 class="step-title">Manage Bracket</h2>
          <p class="step-description">
            Configure your bracket details including the tournament name, game type, format, and other
            settings. Make sure everything is set up the way you want before adding players.
          </p>
          <button @click="handleEditBracket" class="step-button">
            <IconSettings :size="20" :stroke-width="1.5" />
            Edit Bracket
          </button>
        </div>
      </div>

      <!-- Step 2: Manage Players -->
      <div class="setup-step">
        <div class="step-number">2</div>
        <div class="step-content">
          <h2 class="step-title">Manage Players</h2>
          <p class="step-description">
            Add, remove, sort, and order players for your tournament. You can organize the seeding and
            ensure all participants are properly configured before starting the bracket.
          </p>
          <button @click="handleManagePlayers" class="step-button">
            <IconUsers :size="20" :stroke-width="1.5" />
            Manage Players
          </button>
        </div>
      </div>

      <!-- Step 3: Begin Tournament -->
      <div class="setup-step">
        <div class="step-number">3</div>
        <div class="step-content">
          <h2 class="step-title">Begin Tournament</h2>
          <p class="step-description">
            Once you've configured your bracket and added all players, start the tournament! Note that
            once started, the bracket structure cannot be modified.
          </p>
          <button @click.stop="handleBeginTournament" class="step-button step-button-primary">
            <IconPlayerPlay :size="20" :stroke-width="1.5" />
            Start Tournament
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { useRouter } from 'vue-router';
import { IconSettings, IconUsers, IconPlayerPlay } from '@tabler/icons-vue';

interface Props {
  bracketId: number;
}

interface Emits {
  (e: 'begin-tournament'): void;
}

const props = defineProps<Props>();
const emit = defineEmits<Emits>();
const router = useRouter();

const handleEditBracket = () => {
  router.push({ name: 'bracket-edit', params: { id: props.bracketId } });
};

const handleManagePlayers = () => {
  router.push({ name: 'bracket-manage-users', params: { id: props.bracketId } });
};

const handleBeginTournament = () => {
  emit('begin-tournament');
};
</script>

<style scoped>
.bracket-setup-container {
  display: flex;
  justify-content: center;
  align-items: center;
  min-height: calc(100vh - 16rem);
  padding: 2rem;
}

.setup-steps {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(280px, 1fr));
  gap: 2rem;
  max-width: 1200px;
  width: 100%;
}

@media (min-width: 768px) {
  .setup-steps {
    grid-template-columns: repeat(3, 1fr);
  }
}

.setup-step {
  display: flex;
  flex-direction: column;
  align-items: center;
  padding: 2rem;
  background: linear-gradient(135deg, #ffffff 0%, #f9fafb 100%);
  border-radius: 12px;
  box-shadow: 0 4px 6px -1px rgba(0, 0, 0, 0.1), 0 2px 4px -1px rgba(0, 0, 0, 0.06);
  transition: all 0.3s ease;
}

.setup-step:hover {
  transform: translateY(-4px);
  box-shadow: 0 10px 15px -3px rgba(0, 0, 0, 0.1), 0 4px 6px -2px rgba(0, 0, 0, 0.05);
  background: linear-gradient(135deg, #f3f4f6 0%, #e5e7eb 100%);
}

:global(.dark) .setup-step {
  background: linear-gradient(135deg, #1f2937 0%, #2d3748 100%);
  box-shadow: 0 4px 6px -1px rgba(0, 0, 0, 0.3), 0 2px 4px -1px rgba(0, 0, 0, 0.2);
}

:global(.dark) .setup-step:hover {
  box-shadow: 0 10px 15px -3px rgba(0, 0, 0, 0.4), 0 4px 6px -2px rgba(0, 0, 0, 0.3);
  background: linear-gradient(135deg, #374151 0%, #4b5563 100%);
}

.step-number {
  width: 3rem;
  height: 3rem;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 1.5rem;
  font-weight: bold;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
  border-radius: 50%;
  margin-bottom: 1.5rem;
  box-shadow: 0 4px 6px -1px rgba(102, 126, 234, 0.4);
}

.step-content {
  display: flex;
  flex-direction: column;
  align-items: center;
  text-align: center;
  flex: 1;
}

.step-title {
  font-size: 1.25rem;
  font-weight: 600;
  color: #1f2937;
  margin-bottom: 0.75rem;
}

:global(.dark) .step-title {
  color: #f9fafb;
}

.step-description {
  font-size: 0.875rem;
  line-height: 1.5;
  color: #6b7280;
  margin-bottom: 1.5rem;
  flex: 1;
}

:global(.dark) .step-description {
  color: #9ca3af;
}

.step-button {
  display: inline-flex;
  align-items: center;
  gap: 0.5rem;
  padding: 0.625rem 1.25rem;
  font-size: 0.875rem;
  font-weight: 500;
  color: #4f46e5;
  background: white;
  border: 1px solid #e5e7eb;
  border-radius: 6px;
  cursor: pointer;
  transition: all 0.2s ease;
}

.step-button:hover {
  background: #f9fafb;
  border-color: #4f46e5;
  transform: translateY(-1px);
}

:global(.dark) .step-button {
  color: #818cf8;
  background: #374151;
  border-color: #4b5563;
}

:global(.dark) .step-button:hover {
  background: #4b5563;
  border-color: #818cf8;
}

.step-button-primary {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
  border: none;
  box-shadow: 0 4px 6px -1px rgba(102, 126, 234, 0.4);
}

.step-button-primary:hover {
  background: linear-gradient(135deg, #5568d3 0%, #6a3f91 100%);
  box-shadow: 0 10px 15px -3px rgba(102, 126, 234, 0.5);
}

:global(.dark) .step-button-primary {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
}

:global(.dark) .step-button-primary:hover {
  background: linear-gradient(135deg, #5568d3 0%, #6a3f91 100%);
}
</style>
