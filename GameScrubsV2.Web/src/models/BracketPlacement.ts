import type { PlacementStatus } from './PlacementStatus';

export interface BracketPlacement {
  id: number;
  playerName: string;
  bracketPlace: string;
  bracketId: number;
  score: number;
  status: PlacementStatus;
  previousBracketPlace: string | null;
  isTop: boolean;
}
