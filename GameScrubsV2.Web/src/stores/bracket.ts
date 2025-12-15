import { ref } from 'vue';
import { defineStore } from 'pinia';

export const useBracketStore = defineStore('bracket', () => {
  // Store lock codes for brackets by bracket ID
  const lockCodes = ref<Map<number, string>>(new Map());

  function setLockCode(bracketId: number, lockCode: string) {
    lockCodes.value.set(bracketId, lockCode);
  }

  function getLockCode(bracketId: number): string | undefined {
    return lockCodes.value.get(bracketId);
  }

  function clearLockCode(bracketId: number) {
    lockCodes.value.delete(bracketId);
  }

  function clearAllLockCodes() {
    lockCodes.value.clear();
  }

  return {
    lockCodes,
    setLockCode,
    getLockCode,
    clearLockCode,
    clearAllLockCodes,
  };
});
