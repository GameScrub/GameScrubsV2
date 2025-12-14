import { API_BASE_URL } from '@/api.config';
import { type Player } from '@/models/Player';
import { getErrorMessage } from './apiErrorHandler';

export const playerService = {
  // GET all players for a bracket
  async getAll(bracketId: number): Promise<Player[]> {
    const response = await fetch(`${API_BASE_URL}/players/${bracketId}`);
    if (!response.ok) {
      const errorMessage = await getErrorMessage(response);
      throw new Error(errorMessage);
    }
    return response.json();
  },

  // POST add new player
  async add(
    bracketId: number,
    playerName: string,
    seed: number,
    lockCode?: string,
  ): Promise<void> {
    const url = lockCode
      ? `${API_BASE_URL}/players/${lockCode}`
      : `${API_BASE_URL}/players`;

    const response = await fetch(url, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({
        bracketId,
        playerName,
        seed,
      }),
    });
    if (!response.ok) {
      const errorMessage = await getErrorMessage(response);
      throw new Error(errorMessage);
    }
  },

  // DELETE remove player
  async remove(bracketId: number, playerId: number, lockCode?: string): Promise<void> {
    const url = lockCode
      ? `${API_BASE_URL}/players/${lockCode}`
      : `${API_BASE_URL}/players`;

    const response = await fetch(url, {
      method: 'DELETE',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({
        bracketId,
        playerId,
      }),
    });
    if (!response.ok) {
      const errorMessage = await getErrorMessage(response);
      throw new Error(errorMessage);
    }
  },

  // POST reorder players
  async reorder(bracketId: number, playerIds: number[], lockCode?: string): Promise<void> {
    const url = lockCode
      ? `${API_BASE_URL}/players/reorder/${lockCode}`
      : `${API_BASE_URL}/players/reorder`;

    const response = await fetch(url, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({
        bracketId,
        playerIds,
      }),
    });
    if (!response.ok) {
      const errorMessage = await getErrorMessage(response);
      throw new Error(errorMessage);
    }
  },
};
