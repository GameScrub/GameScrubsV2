import { API_BASE_URL } from '@/api.config';
import type { BracketPlacement } from '@/models/BracketPlacement';
import type { BracketPosition } from '../models/BracketPosition';
import { getErrorMessage } from './apiErrorHandler';

export const bracketPlacementService = {
  // GET Bracket Placements
  async getBracketPlacements(bracketId: number): Promise<BracketPlacement[]> {
    const response = await fetch(`${API_BASE_URL}/brackets/${bracketId}/placements`);

    if (!response.ok) {
      const errorMessage = await getErrorMessage(response);
      throw new Error(errorMessage);
    }
    return response.json();
  },
  // Get Bracket Positions
  async getBrackePositions(bracketId: number): Promise<BracketPosition[]> {
    const response = await fetch(`${API_BASE_URL}/brackets/${bracketId}/placements/positions/`);

    if (!response.ok) {
      const errorMessage = await getErrorMessage(response);
      throw new Error(errorMessage);
    }

    return response.json();
  },
  // Get Bracket Scores
  async getBrackeScores(bracketId: number): Promise<Record<string, number>> {
    const response = await fetch(`${API_BASE_URL}/brackets/${bracketId}/placements/score`);

    if (!response.ok) {
      const errorMessage = await getErrorMessage(response);
      throw new Error(errorMessage);
    }

    return response.json();
  },
  // Set Bracket Placement
  async setBracketPlacement(
    bracketId: number,
    placementId: number,
    lockCode?: string,
  ): Promise<void> {
    const url = lockCode
      ? `${API_BASE_URL}/brackets/${bracketId}/placements/score/${placementId}/${lockCode}`
      : `${API_BASE_URL}/brackets/${bracketId}/placements/score/${placementId}`;

    const response = await fetch(url, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
    });

    if (!response.ok) {
      const errorMessage = await getErrorMessage(response);
      throw new Error(errorMessage);
    }
  },
};
