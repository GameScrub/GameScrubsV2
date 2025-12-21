import type { BracketType } from './BracketType';

export interface BracketPosition {
  id: number;
  type: BracketType;
  player1: string | null;
  player2: string | null;
  winLocation: string | null;
  loseLocation: string | null;
  markerPosition: number | null;
}
