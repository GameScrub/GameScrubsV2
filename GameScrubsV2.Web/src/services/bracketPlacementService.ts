import { API_BASE_URL } from '@/api.config';
import type { BracketPlacement } from '@/models/BracketPlacement';
import type { BracketPosition } from '../models/BracketPosition';

export const bracketPlacementService = {
  // GET Bracket Placements
  async getBracketPlacements(bracketId: number): Promise<BracketPlacement[]> {
    const response = await fetch(`${API_BASE_URL}/brackets/${bracketId}/placements`);

    if (!response.ok) {
      throw new Error('Failed to fetch bracket placements');
    }
    return response.json();
  },
  // Get Bracket Positions
  async getBrackePositions(bracketId: number): Promise<BracketPosition[]> {
    const response = await fetch(`${API_BASE_URL}/brackets/${bracketId}/placements/positions/`);

    if (!response.ok) {
      throw new Error('Failed to fetch bracket positions');
    }

    return response.json();
  },
  // Set Bracket Placement
  async setBracketPlacement(bracketId: number, placementId: number): Promise<void> {
    const response = await fetch(
      `${API_BASE_URL}/brackets/${bracketId}/placements/score/${placementId}`,
      {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
      },
    );

    if (!response.ok) throw new Error('Failed to set bracket placement');
  },
};
